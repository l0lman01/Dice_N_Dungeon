using System;
using DiceNDungeon.Dice;
using UnityEngine;

namespace DiceNDungeon.Player
{
    /// <summary>
    /// The player's die: after each roll it picks a face, shows it on top of the die visual,
    /// and raises <see cref="FaceRolled"/> so abilities and UI can react.
    /// </summary>
    public class PlayerDie : MonoBehaviour
    {
        [Header("Data")]
        [Tooltip("The die (6 faces) the player is using.")]
        [SerializeField] private DieDefinition die; // current loadout

        [Tooltip("Index of the face on top when the game starts (0 = value 1).")]
        [SerializeField] private int startFaceIndex = 0; // starting face

        [Tooltip("Random seed for rolls. 0 = different every run.")]
        [SerializeField] private int seed = 0; // fixed seed for testing, 0 = random

        [Header("References")]
        [Tooltip("Roll component whose end triggers a die roll. Empty = looks on this GameObject.")]
        [SerializeField] private PlayerRoll roll; // source of RollEnded

        [Tooltip("Child holding the die mesh, rotated so the rolled face is on top.")]
        [SerializeField] private Transform dieVisual; // mesh to orient

        [Tooltip("Renderer tinted with the face color. Optional.")]
        [SerializeField] private Renderer dieRenderer; // mesh renderer for color feedback

        [Tooltip("Event asset raised on every roll (HUD, SFX). Optional.")]
        [SerializeField] private DieFaceEventChannel faceRolledChannel; // global event for other systems

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor"); // URP Lit color property
        private DieRoller roller;
        private MaterialPropertyBlock propertyBlock;

        public int CurrentFaceIndex { get; private set; }
        public DieFaceData CurrentFace => die != null ? die.GetFace(CurrentFaceIndex) : null;

        public event Action<DieFaceData> FaceRolled;

        private void Awake()
        {
            roller = new DieRoller(seed != 0 ? seed : null);
            propertyBlock = new MaterialPropertyBlock();
            if (roll == null) roll = GetComponent<PlayerRoll>();
        }

        private void OnEnable()
        {
            if (roll != null) roll.RollEnded += Roll;
        }

        private void OnDisable()
        {
            if (roll != null) roll.RollEnded -= Roll;
        }

        private void Start()
        {
            // Start (not Awake) so PlayerAbilities is subscribed before the first face is announced.
            if (die != null) SetFace(Mathf.Clamp(startFaceIndex, 0, die.FaceCount - 1));
        }

        /// <summary>Rolls the die and applies the result.</summary>
        public void Roll()
        {
            if (die == null || die.FaceCount == 0)
            {
                Debug.LogWarning("PlayerDie has no DieDefinition assigned.", this);
                return;
            }
            SetFace(roller.RollIndex(die.FaceCount));
        }

        /// <summary>Forces a face (debug, scripted events, abilities that "set" the die).</summary>
        public void SetFace(int faceIndex)
        {
            CurrentFaceIndex = faceIndex;

            if (dieVisual != null) dieVisual.localRotation = DieOrientation.RotationForFaceUp(faceIndex);

            DieFaceData face = CurrentFace;
            if (dieRenderer != null && face != null)
            {
                dieRenderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetColor(BaseColorId, face.Color);
                dieRenderer.SetPropertyBlock(propertyBlock);
            }

            FaceRolled?.Invoke(face);
            if (faceRolledChannel != null) faceRolledChannel.Raise(face);
        }
    }
}
