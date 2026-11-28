using System;
using DiceNDungeon.Core;
using UnityEngine.UIElements;

namespace DiceNDungeon.UI
{
    /// <summary>
    /// Logic for SettingsPanel.uxml (plain C#, reused by the main menu and the pause menu).
    /// Sliders write straight into <see cref="GameSettings"/>; Back saves and closes.
    /// </summary>
    public class SettingsPanel
    {
        private const string HiddenClass = "is-hidden";

        private readonly VisualElement root;
        private readonly Slider volumeSlider;
        private readonly Slider sensitivitySlider;
        private readonly Action onClosed;

        public bool IsOpen => !root.ClassListContains(HiddenClass);

        /// <param name="root">The instanced SettingsPanel template.</param>
        /// <param name="onClosed">Called after Back (e.g. to show the previous menu again).</param>
        public SettingsPanel(VisualElement root, Action onClosed)
        {
            this.root = root;
            this.onClosed = onClosed;

            volumeSlider = root.Q<Slider>("volumeSlider");
            sensitivitySlider = root.Q<Slider>("sensitivitySlider");
            sensitivitySlider.lowValue = GameSettings.MinSensitivity;
            sensitivitySlider.highValue = GameSettings.MaxSensitivity;

            volumeSlider.RegisterValueChangedCallback(evt => GameSettings.Volume = evt.newValue);
            sensitivitySlider.RegisterValueChangedCallback(evt => GameSettings.MouseSensitivity = evt.newValue);
            root.Q<Button>("settingsBackButton").clicked += Close;
        }

        public void Open()
        {
            volumeSlider.SetValueWithoutNotify(GameSettings.Volume);
            sensitivitySlider.SetValueWithoutNotify(GameSettings.MouseSensitivity);
            root.RemoveFromClassList(HiddenClass);
            volumeSlider.Focus();
        }

        public void Close()
        {
            if (!IsOpen) return;
            GameSettings.Save();
            root.AddToClassList(HiddenClass);
            onClosed?.Invoke();
        }
    }
}
