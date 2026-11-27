using DiceNDungeon.Player;
using UnityEngine;

namespace DiceNDungeon.Abilities
{
    /// <summary>
    /// Base of every die-face ability. Abilities are ScriptableObject assets shared by everyone using them,
    /// so never store per-player state in fields: keep it in the player's components.
    /// </summary>
    public abstract class Ability : ScriptableObject
    {
        [Tooltip("Name shown in the HUD.")]
        [SerializeField] private string displayName = "New Ability"; // UI label

        [Tooltip("Short description shown in the HUD / tooltips.")]
        [SerializeField, TextArea] private string description; // UI text

        [Tooltip("Icon shown in the HUD. Optional.")]
        [SerializeField] private Sprite icon; // UI icon

        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;

        /// <summary>Called when the die lands on a face granting this ability.</summary>
        public virtual void OnEquip(PlayerContext context) { }

        /// <summary>Called when the die lands on another face.</summary>
        public virtual void OnUnequip(PlayerContext context) { }
    }
}
