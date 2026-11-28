using DiceNDungeon.Core;
using UnityEngine;

namespace DiceNDungeon.Player
{
    /// <summary>Stats that abilities can modify. Add a new entry here to add a new stat.</summary>
    public enum StatType
    {
        MoveSpeed,
        Damage,
    }

    /// <summary>Holds the player's runtime stats. Base values are set in the Inspector; abilities add modifiers.</summary>
    public class PlayerStats : MonoBehaviour
    {
        [Tooltip("Base ground speed in meters per second, before modifiers.")]
        [SerializeField] private float baseMoveSpeed = 6f; // m/s

        [Tooltip("Base damage dealt by abilities, before modifiers.")]
        [SerializeField] private float baseDamage = 1f; // damage points

        public Stat MoveSpeed { get; private set; }
        public Stat Damage { get; private set; }

        private void Awake()
        {
            MoveSpeed = new Stat(baseMoveSpeed);
            Damage = new Stat(baseDamage);
        }

        public Stat Get(StatType type) => type switch
        {
            StatType.MoveSpeed => MoveSpeed,
            StatType.Damage => Damage,
            _ => null,
        };
    }
}
