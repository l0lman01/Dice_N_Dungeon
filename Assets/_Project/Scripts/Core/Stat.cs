using System;
using System.Collections.Generic;
using UnityEngine;

namespace DiceNDungeon.Core
{
    /// <summary>How a <see cref="StatModifier"/> changes a stat.</summary>
    public enum StatModifierType
    {
        Flat,        // added to the base value: base + value
        PercentAdd,  // summed with other PercentAdd, then applied once: x (1 + sum)
        PercentMult  // applied one by one: x (1 + value)
    }

    /// <summary>One change to a stat, e.g. "+30 % move speed". Serializable so abilities can define them in the Inspector.</summary>
    [Serializable]
    public class StatModifier
    {
        [Tooltip("How the value is applied to the stat.")]
        [SerializeField] private StatModifierType type = StatModifierType.PercentAdd; // flat or percentage modifier

        [Tooltip("Amount. Flat: units of the stat. Percent: 0.3 = +30 %, -0.2 = -20 %.")]
        [SerializeField] private float value = 0.3f; // modifier amount, meaning depends on type

        public StatModifierType Type => type;
        public float Value => value;

        public StatModifier() { } // needed by Unity serialization

        public StatModifier(StatModifierType type, float value)
        {
            this.type = type;
            this.value = value;
        }
    }

    /// <summary>
    /// A numeric stat with a base value and a list of modifiers.
    /// Pure C#: no MonoBehaviour, so it can be unit tested.
    /// Final value = (base + flat) x (1 + sum of PercentAdd) x product of (1 + PercentMult).
    /// </summary>
    public class Stat
    {
        private readonly List<(StatModifier modifier, object source)> modifiers = new();
        private float baseValue;

        /// <summary>Raised when the final value may have changed.</summary>
        public event Action<Stat> Changed;

        public Stat(float baseValue)
        {
            this.baseValue = baseValue;
        }

        public float BaseValue
        {
            get => baseValue;
            set
            {
                baseValue = value;
                Changed?.Invoke(this);
            }
        }

        public float Value
        {
            get
            {
                float flat = 0f;
                float percentAdd = 0f;
                float percentMult = 1f;
                foreach (var (modifier, _) in modifiers)
                {
                    switch (modifier.Type)
                    {
                        case StatModifierType.Flat: flat += modifier.Value; break;
                        case StatModifierType.PercentAdd: percentAdd += modifier.Value; break;
                        case StatModifierType.PercentMult: percentMult *= 1f + modifier.Value; break;
                    }
                }
                return (baseValue + flat) * (1f + percentAdd) * percentMult;
            }
        }

        public int ModifierCount => modifiers.Count;

        /// <param name="source">Who added it (usually an ability). Used to remove all its modifiers at once.</param>
        public void AddModifier(StatModifier modifier, object source)
        {
            if (modifier == null) return;
            modifiers.Add((modifier, source));
            Changed?.Invoke(this);
        }

        /// <summary>Removes every modifier added by <paramref name="source"/>. Returns how many were removed.</summary>
        public int RemoveAllFromSource(object source)
        {
            int removed = modifiers.RemoveAll(entry => Equals(entry.source, source));
            if (removed > 0) Changed?.Invoke(this);
            return removed;
        }
    }
}
