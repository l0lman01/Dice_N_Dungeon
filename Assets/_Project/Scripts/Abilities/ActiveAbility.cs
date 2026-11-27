using DiceNDungeon.Player;
using UnityEngine;

namespace DiceNDungeon.Abilities
{
    /// <summary>A skill the player triggers with the ability button while its face is up.</summary>
    public abstract class ActiveAbility : Ability
    {
        [Tooltip("Seconds before the ability can be used again.")]
        [SerializeField] private float cooldown = 1f; // seconds

        public float Cooldown => cooldown;

        /// <summary>Runs the skill. Cooldown is handled by <see cref="PlayerAbilities"/>.</summary>
        public abstract void Activate(PlayerContext context);
    }
}
