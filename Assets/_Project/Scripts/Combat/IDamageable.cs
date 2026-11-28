using UnityEngine;

namespace DiceNDungeon.Combat
{
    /// <summary>Anything that can take damage (player, enemies, breakables).</summary>
    public interface IDamageable
    {
        void TakeDamage(DamageInfo info);
    }

    /// <summary>Implemented by components that can make their GameObject temporarily immune (e.g. the dodge roll).</summary>
    public interface IInvulnerabilitySource
    {
        bool IsInvulnerable { get; }
    }

    /// <summary>Data describing one hit.</summary>
    public readonly struct DamageInfo
    {
        public readonly float Amount;        // damage points
        public readonly GameObject Source;   // who dealt it (can be null)
        public readonly Vector3 HitPoint;    // world position of the hit

        public DamageInfo(float amount, GameObject source, Vector3 hitPoint)
        {
            Amount = amount;
            Source = source;
            HitPoint = hitPoint;
        }
    }
}
