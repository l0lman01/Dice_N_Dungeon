using System;
using UnityEngine;

namespace DiceNDungeon.Combat
{
    /// <summary>
    /// Hit points for the player or an enemy. Damage is ignored while any
    /// <see cref="IInvulnerabilitySource"/> on the same GameObject says so (e.g. during a roll).
    /// </summary>
    public class Health : MonoBehaviour, IDamageable
    {
        [Tooltip("Hit points at spawn and maximum hit points.")]
        [SerializeField] private float maxHealth = 10f; // maximum HP

        private IInvulnerabilitySource[] invulnerabilitySources;

        public float Current { get; private set; }
        public float Max => maxHealth;
        public bool IsDead => Current <= 0f;

        public event Action<DamageInfo> Damaged;
        public event Action<float> Healed;
        public event Action Died;

        private void Awake()
        {
            Current = maxHealth;
            invulnerabilitySources = GetComponents<IInvulnerabilitySource>();
        }

        public bool IsInvulnerable
        {
            get
            {
                if (invulnerabilitySources == null) return false;
                foreach (var source in invulnerabilitySources)
                {
                    if (source.IsInvulnerable) return true;
                }
                return false;
            }
        }

        public void TakeDamage(DamageInfo info)
        {
            if (IsDead || IsInvulnerable || info.Amount <= 0f) return;

            Current = Mathf.Max(0f, Current - info.Amount);
            Damaged?.Invoke(info);
            if (IsDead) Died?.Invoke();
        }

        public void Heal(float amount)
        {
            if (IsDead || amount <= 0f) return;
            Current = Mathf.Min(maxHealth, Current + amount);
            Healed?.Invoke(amount);
        }
    }
}
