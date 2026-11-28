using DiceNDungeon.Combat;
using DiceNDungeon.Player;
using UnityEngine;

namespace DiceNDungeon.Abilities
{
    /// <summary>Example active ability: damages every <see cref="IDamageable"/> around the player.</summary>
    [CreateAssetMenu(menuName = "Dice n Dungeon/Abilities/Active - Shockwave", fileName = "Shockwave")]
    public class ShockwaveAbility : ActiveAbility
    {
        [Tooltip("Radius of the shockwave in meters.")]
        [SerializeField] private float radius = 3f; // meters

        [Tooltip("Damage multiplier applied to the player's Damage stat.")]
        [SerializeField] private float damageMultiplier = 2f; // final damage = Damage stat x this

        [Tooltip("Layers that can be hit.")]
        [SerializeField] private LayerMask hitLayers = ~0; // physics layers checked

        public override void Activate(PlayerContext context)
        {
            Vector3 center = context.Transform.position;
            float damage = (context.Stats != null ? context.Stats.Damage.Value : 1f) * damageMultiplier;

            foreach (Collider hit in Physics.OverlapSphere(center, radius, hitLayers, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.root == context.Transform.root) continue; // don't hit ourselves
                if (hit.TryGetComponent(out IDamageable target))
                {
                    target.TakeDamage(new DamageInfo(damage, context.Transform.gameObject, hit.ClosestPoint(center)));
                }
            }
            Debug.Log($"{DisplayName}: shockwave for {damage} damage.", this);
        }
    }
}
