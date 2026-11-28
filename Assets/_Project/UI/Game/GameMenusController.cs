using DiceNDungeon.Combat;
using DiceNDungeon.Core;
using DiceNDungeon.Inputs;
using DiceNDungeon.Player;
using UnityEngine;
using UnityEngine.UIElements;

namespace DiceNDungeon.UI
{
    /// <summary>
    /// Pause menu (Esc / Start), settings from pause, and game over screen when the player dies.
    /// Pausing freezes time, disables gameplay input (camera included) and frees the cursor.
    /// </summary>
    [RequireComponent(typeof(UIDocument), typeof(MenuInputReader))]
    public class GameMenusController : MonoBehaviour
    {
        [Tooltip("Scene loaded by the 'Main menu' buttons.")]
        [SerializeField] private string mainMenuSceneName = "MainMenu"; // main menu scene name

        [Tooltip("Player health watched for game over. Empty = found automatically in the scene.")]
        [SerializeField] private Health playerHealth; // death triggers the game over screen

        [Tooltip("Lock and hide the mouse cursor during gameplay.")]
        [SerializeField] private bool lockCursorInGame = true; // cursor state while playing

        private MenuInputReader input;
        private VisualElement pauseMenu;
        private VisualElement gameOver;
        private SettingsPanel settings;

        public bool IsPaused { get; private set; }
        public bool IsGameOver { get; private set; }

        private void Awake()
        {
            input = GetComponent<MenuInputReader>();
            if (playerHealth == null)
            {
                PlayerDie player = FindFirstObjectByType<PlayerDie>();
                if (player != null) playerHealth = player.GetComponent<Health>();
            }
        }

        private void OnEnable()
        {
            input.PausePressed += OnPausePressed;
            if (playerHealth != null) playerHealth.Died += OnPlayerDied;
        }

        private void OnDisable()
        {
            input.PausePressed -= OnPausePressed;
            if (playerHealth != null) playerHealth.Died -= OnPlayerDied;
        }

        private void Start()
        {
            GameFlow.ResetState();
            InputMaps.SetCursorLocked(lockCursorInGame);

            // Query in Start: UIDocument builds its visual tree in its own OnEnable.
            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            pauseMenu = root.Q("pauseMenu");
            gameOver = root.Q("gameOver");
            settings = new SettingsPanel(root.Q("settingsPanel"), () => Show(pauseMenu, true));

            root.Q<Button>("resumeButton").clicked += () => SetPaused(false);
            root.Q<Button>("pauseSettingsButton").clicked += OpenSettings;
            root.Q<Button>("pauseMainMenuButton").clicked += () => GameFlow.LoadScene(mainMenuSceneName);
            root.Q<Button>("pauseQuitButton").clicked += GameFlow.Quit;
            root.Q<Button>("retryButton").clicked += GameFlow.ReloadCurrentScene;
            root.Q<Button>("gameOverMainMenuButton").clicked += () => GameFlow.LoadScene(mainMenuSceneName);
        }

        private void OnPausePressed()
        {
            if (IsGameOver || settings == null) return;
            if (settings.IsOpen) settings.Close(); // Esc in settings = Back
            else SetPaused(!IsPaused);
        }

        public void SetPaused(bool paused)
        {
            IsPaused = paused;
            Time.timeScale = paused ? 0f : 1f;
            InputMaps.SetGameplayEnabled(!paused);
            InputMaps.SetCursorLocked(!paused && lockCursorInGame);
            Show(pauseMenu, paused);
            if (paused) pauseMenu.Q<Button>("resumeButton").Focus();
        }

        private void OpenSettings()
        {
            Show(pauseMenu, false);
            settings.Open();
        }

        private void OnPlayerDied()
        {
            if (settings.IsOpen) settings.Close();
            SetPaused(false);
            IsGameOver = true;
            Time.timeScale = 0f;
            InputMaps.SetGameplayEnabled(false);
            InputMaps.SetCursorLocked(false);
            Show(gameOver, true);
            gameOver.Q<Button>("retryButton").Focus();
        }

        private static void Show(VisualElement element, bool visible) => element.EnableInClassList("is-hidden", !visible);
    }
}
