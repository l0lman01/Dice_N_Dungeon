using DiceNDungeon.Abilities;
using UnityEngine;

namespace DiceNDungeon.Dice
{
    /// <summary>One face of a die: its value, look, and the ability it grants while it is on top.</summary>
    [CreateAssetMenu(menuName = "Dice n Dungeon/Dice/Die Face", fileName = "DieFace")]
    public class DieFaceData : ScriptableObject
    {
        [Tooltip("Number shown on the face (1-6 for a d6).")]
        [SerializeField] private int value = 1; // pip value of the face

        [Tooltip("Tint applied to the die mesh while this face is up.")]
        [SerializeField] private Color color = Color.white; // feedback color for the player

        [Tooltip("Ability equipped while this face is up (passive or active). Can be empty.")]
        [SerializeField] private Ability ability; // granted ability, null = no effect

        public int Value => value;
        public Color Color => color;
        public Ability Ability => ability;

        public override string ToString() => ability != null ? $"{value} ({ability.DisplayName})" : value.ToString();
    }
}
