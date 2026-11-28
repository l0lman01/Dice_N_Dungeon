using DiceNDungeon.Abilities;
using DiceNDungeon.Dice;
using DiceNDungeon.Inputs;
using UnityEngine;

namespace DiceNDungeon.Player
{
    /// <summary>
    /// Equips the ability of the face currently on top and fires active abilities
    /// when the ability button is pressed (respecting their cooldown).
    /// </summary>
    [RequireComponent(typeof(PlayerDie))]
    public class PlayerAbilities : MonoBehaviour
    {
        [Tooltip("Input source. Empty = looks for one on this GameObject.")]
        [SerializeField] private PlayerInputReader input; // listens to AbilityPressed

        [Tooltip("Log equips and activations in the Console.")]
        [SerializeField] private bool debugLogs = true; // console feedback while prototyping

        private PlayerDie die;
        private PlayerContext context;
        private float nextActiveTime;

        public Ability Equipped { get; private set; }
        public float CooldownRemaining => Mathf.Max(0f, nextActiveTime - Time.time);

        private void Awake()
        {
            die = GetComponent<PlayerDie>();
            if (input == null) input = GetComponent<PlayerInputReader>();
            context = new PlayerContext(this);
        }

        private void OnEnable()
        {
            die.FaceRolled += OnFaceRolled;
            if (input != null) input.AbilityPressed += TryActivate;
        }

        private void OnDisable()
        {
            die.FaceRolled -= OnFaceRolled;
            if (input != null) input.AbilityPressed -= TryActivate;
            Equip(null);
        }

        private void OnFaceRolled(DieFaceData face)
        {
            if (debugLogs) Debug.Log($"Die landed on {(face != null ? face.ToString() : "an empty face")}.", this);
            Equip(face != null ? face.Ability : null);
        }

        private void Equip(Ability ability)
        {
            if (Equipped == ability) return;
            if (Equipped != null) Equipped.OnUnequip(context);
            Equipped = ability;
            if (Equipped != null) Equipped.OnEquip(context);
        }

        public void TryActivate()
        {
            if (Equipped is not ActiveAbility active || Time.time < nextActiveTime) return;

            active.Activate(context);
            nextActiveTime = Time.time + active.Cooldown;
            if (debugLogs) Debug.Log($"Activated {active.DisplayName}.", this);
        }
    }
}
