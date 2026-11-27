using System;
using DiceNDungeon.Core;
using DiceNDungeon.Player;
using UnityEngine;

namespace DiceNDungeon.Abilities
{
    /// <summary>
    /// Always-on effect while its face is up: applies stat modifiers on equip, removes them on unequip.
    /// Example asset: "Swift" = MoveSpeed, PercentAdd, 0.3 (+30 % speed).
    /// Subclass it for passives that need more than stat changes.
    /// </summary>
    [CreateAssetMenu(menuName = "Dice n Dungeon/Abilities/Passive (Stat Modifiers)", fileName = "Passive")]
    public class PassiveAbility : Ability
    {
        [Serializable]
        private class StatModifierEntry
        {
            [Tooltip("Which player stat is changed.")]
            [SerializeField] private StatType stat = StatType.MoveSpeed; // targeted stat

            [Tooltip("How the stat is changed.")]
            [SerializeField] private StatModifier modifier = new(StatModifierType.PercentAdd, 0.3f); // the change applied

            public StatType Stat => stat;
            public StatModifier Modifier => modifier;
        }

        [Tooltip("Stat changes applied while this ability is equipped.")]
        [SerializeField] private StatModifierEntry[] modifiers = Array.Empty<StatModifierEntry>(); // list of stat changes

        public override void OnEquip(PlayerContext context)
        {
            if (context.Stats == null) return;
            foreach (var entry in modifiers)
            {
                context.Stats.Get(entry.Stat)?.AddModifier(entry.Modifier, this);
            }
        }

        public override void OnUnequip(PlayerContext context)
        {
            if (context.Stats == null) return;
            foreach (StatType type in Enum.GetValues(typeof(StatType)))
            {
                context.Stats.Get(type)?.RemoveAllFromSource(this);
            }
        }
    }
}
