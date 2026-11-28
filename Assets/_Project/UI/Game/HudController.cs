using DiceNDungeon.Abilities;
using DiceNDungeon.Combat;
using DiceNDungeon.Dice;
using DiceNDungeon.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace DiceNDungeon.UI
{
    /// <summary>In-game HUD: current die face and its ability, HP bar, roll and ability cooldowns.</summary>
    [RequireComponent(typeof(UIDocument))]
    public class HudController : MonoBehaviour
    {
        [Tooltip("Player die. Empty = found automatically in the scene.")]
        [SerializeField] private PlayerDie playerDie; // source of the current face

        [Tooltip("Player roll. Empty = found automatically in the scene.")]
        [SerializeField] private PlayerRoll playerRoll; // source of the roll cooldown

        [Tooltip("Player abilities. Empty = found automatically in the scene.")]
        [SerializeField] private PlayerAbilities playerAbilities; // source of the ability cooldown

        [Tooltip("Player health. Empty = found automatically in the scene.")]
        [SerializeField] private Health playerHealth; // source of the HP bar

        [Tooltip("Milliseconds the die icon stays enlarged after a new face lands.")]
        [SerializeField] private int popDurationMs = 150; // ms

        private VisualElement faceDie;
        private Label faceValue;
        private Label abilityName;
        private Label abilityType;
        private Label abilityDescription;
        private ProgressBar healthBar;
        private ProgressBar rollBar;
        private ProgressBar abilityBar;

        private void Awake()
        {
            if (playerDie == null) playerDie = FindFirstObjectByType<PlayerDie>();
            if (playerRoll == null) playerRoll = FindFirstObjectByType<PlayerRoll>();
            if (playerAbilities == null) playerAbilities = FindFirstObjectByType<PlayerAbilities>();
            if (playerHealth == null && playerDie != null) playerHealth = playerDie.GetComponent<Health>();
        }

        private void OnEnable()
        {
            if (playerDie != null) playerDie.FaceRolled += ShowFace;
        }

        private void OnDisable()
        {
            if (playerDie != null) playerDie.FaceRolled -= ShowFace;
        }

        private void Start()
        {
            // Query in Start: UIDocument builds its visual tree in its own OnEnable.
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            faceDie = root.Q("faceDie");
            faceValue = root.Q<Label>("faceValue");
            abilityName = root.Q<Label>("abilityName");
            abilityType = root.Q<Label>("abilityType");
            abilityDescription = root.Q<Label>("abilityDescription");
            healthBar = root.Q<ProgressBar>("healthBar");
            rollBar = root.Q<ProgressBar>("rollBar");
            abilityBar = root.Q<ProgressBar>("abilityBar");

            if (playerDie == null) Debug.LogWarning("HUD: no PlayerDie found in the scene.", this);
            else if (playerDie.CurrentFace != null) ShowFace(playerDie.CurrentFace);
        }

        private void ShowFace(DieFaceData face)
        {
            if (face == null || faceValue == null) return; // UI not built yet: Start shows the current face

            faceValue.text = face.Value.ToString();
            // Face color comes from data (DieFaceData), so it can't live in USS.
            faceDie.style.backgroundColor = face.Color;

            Ability ability = face.Ability;
            abilityName.text = ability != null ? ability.DisplayName : "No ability";
            abilityType.text = ability switch
            {
                ActiveAbility => "Active - press Attack",
                PassiveAbility => "Passive",
                null => "Roll again!",
                _ => "Ability",
            };
            abilityDescription.text = ability != null ? ability.Description : string.Empty;

            faceDie.AddToClassList("hud__die--pop");
            faceDie.schedule.Execute(() => faceDie.RemoveFromClassList("hud__die--pop")).ExecuteLater(popDurationMs);
        }

        private void Update()
        {
            if (playerHealth != null)
            {
                healthBar.value = playerHealth.Max > 0f ? playerHealth.Current / playerHealth.Max : 0f;
                healthBar.title = $"HP {Mathf.CeilToInt(playerHealth.Current)} / {Mathf.CeilToInt(playerHealth.Max)}";
            }

            if (playerRoll != null)
            {
                rollBar.value = Ready(playerRoll.CooldownRemaining, playerRoll.Cooldown);
                rollBar.title = playerRoll.CooldownRemaining > 0f ? "Roll" : "Roll ready";
            }

            if (playerAbilities != null)
            {
                bool hasActive = playerAbilities.Equipped is ActiveAbility;
                abilityBar.EnableInClassList("is-hidden", !hasActive);
                if (playerAbilities.Equipped is ActiveAbility active)
                {
                    abilityBar.value = Ready(playerAbilities.CooldownRemaining, active.Cooldown);
                    abilityBar.title = playerAbilities.CooldownRemaining > 0f ? active.DisplayName : $"{active.DisplayName} ready";
                }
            }
        }

        /// <summary>0 = just used, 1 = ready.</summary>
        private static float Ready(float remaining, float total) => total > 0f ? 1f - Mathf.Clamp01(remaining / total) : 1f;
    }
}
