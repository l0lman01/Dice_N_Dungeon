using System.Collections;
using DiceNDungeon.Core;
using DiceNDungeon.Inputs;
using UnityEngine;
using UnityEngine.UIElements;

namespace DiceNDungeon.UI
{
    /// <summary>
    /// "Press F Studio" opening screen: the logo fades in, then the game waits until the player presses F.
    /// No timer, no other key: only F starts the game.
    /// </summary>
    [RequireComponent(typeof(UIDocument), typeof(MenuInputReader))]
    public class SplashController : MonoBehaviour
    {
        [Tooltip("Scene loaded after F is pressed (must be in Build Profiles scene list).")]
        [SerializeField] private string nextSceneName = "MainMenu"; // scene to open after the splash

        [Tooltip("Seconds before the 'Press F to start' prompt appears and F is accepted.")]
        [SerializeField] private float promptDelay = 1.2f; // seconds

        [Tooltip("Seconds between two blinks of the prompt.")]
        [SerializeField] private float blinkInterval = 0.6f; // seconds

        [Tooltip("Seconds of fade-out after F is pressed, before the next scene loads.")]
        [SerializeField] private float exitDuration = 0.6f; // seconds

        private MenuInputReader input;
        private VisualElement root;
        private VisualElement logo;
        private VisualElement studioLogo;
        private Label prompt;
        private bool acceptingInput;
        private bool leaving;

        private void Awake() => input = GetComponent<MenuInputReader>();

        private void OnEnable() => input.PressFPressed += OnPressF;

        private void OnDisable() => input.PressFPressed -= OnPressF;

        private void Start()
        {
            GameFlow.ResetState();
            InputMaps.SetCursorLocked(false);

            root = GetComponent<UIDocument>().rootVisualElement.Q("splashRoot");
            logo = root.Q("logo");
            studioLogo = root.Q("studioLogo");
            prompt = root.Q<Label>("prompt");

            // Remove the hidden class one frame later so the USS transition plays.
            root.schedule.Execute(() => logo.RemoveFromClassList("splash__logo--hidden")).ExecuteLater(50);
            root.schedule.Execute(ShowPrompt).ExecuteLater((long)(promptDelay * 1000f));
        }

        private void ShowPrompt()
        {
            acceptingInput = true;
            prompt.RemoveFromClassList("splash__prompt--hidden");
            root.schedule.Execute(() => prompt.ToggleInClassList("splash__prompt--dim"))
                .Every((long)(blinkInterval * 1000f));
        }

        private void OnPressF()
        {
            if (!acceptingInput || leaving) return;
            leaving = true;
            studioLogo.AddToClassList("splash__studio-logo--pressed");
            prompt.text = "Respect paid.";
            root.AddToClassList("splash--fade-out");
            StartCoroutine(LoadNextScene());
        }

        private IEnumerator LoadNextScene()
        {
            yield return new WaitForSecondsRealtime(exitDuration);
            GameFlow.LoadScene(nextSceneName);
        }
    }
}
