using DiceNDungeon.Core;
using DiceNDungeon.Inputs;
using UnityEngine;
using UnityEngine.UIElements;

namespace DiceNDungeon.UI
{
    /// <summary>Main menu: Play, Settings, Quit.</summary>
    [RequireComponent(typeof(UIDocument))]
    public class MainMenuController : MonoBehaviour
    {
        [Tooltip("Gameplay scene loaded by Play (must be in Build Profiles scene list).")]
        [SerializeField] private string gameSceneName = "Daniel"; // scene started by the Play button

        private VisualElement menuButtons;
        private SettingsPanel settings;

        private void Start()
        {
            GameFlow.ResetState();
            InputMaps.SetCursorLocked(false);

            VisualElement root = GetComponent<UIDocument>().rootVisualElement;
            menuButtons = root.Q("menuButtons");
            settings = new SettingsPanel(root.Q("settingsPanel"), () => menuButtons.RemoveFromClassList("is-hidden"));

            Button play = root.Q<Button>("playButton");
            play.clicked += () => GameFlow.LoadScene(gameSceneName);
            root.Q<Button>("settingsButton").clicked += OpenSettings;
            root.Q<Button>("quitButton").clicked += GameFlow.Quit;
            play.Focus();
        }

        private void OpenSettings()
        {
            menuButtons.AddToClassList("is-hidden");
            settings.Open();
        }
    }
}
