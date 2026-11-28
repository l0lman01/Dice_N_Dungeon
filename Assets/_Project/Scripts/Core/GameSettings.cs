using System;
using UnityEngine;

namespace DiceNDungeon.Core
{
    /// <summary>
    /// Player options saved in PlayerPrefs: master volume and mouse sensitivity.
    /// Read anywhere with <see cref="Volume"/> / <see cref="MouseSensitivity"/>; listen to <see cref="Changed"/>.
    /// </summary>
    public static class GameSettings
    {
        public const float MinSensitivity = 0.1f;
        public const float MaxSensitivity = 3f;

        private const string VolumeKey = "settings.volume";
        private const string SensitivityKey = "settings.mouseSensitivity";

        /// <summary>Raised after any setting changes.</summary>
        public static event Action Changed;

        /// <summary>Master volume, 0 (mute) to 1.</summary>
        public static float Volume
        {
            get => PlayerPrefs.GetFloat(VolumeKey, 0.8f);
            set
            {
                PlayerPrefs.SetFloat(VolumeKey, Mathf.Clamp01(value));
                Apply();
            }
        }

        /// <summary>Multiplier applied to camera orbit speed, 1 = default.</summary>
        public static float MouseSensitivity
        {
            get => PlayerPrefs.GetFloat(SensitivityKey, 1f);
            set
            {
                PlayerPrefs.SetFloat(SensitivityKey, Mathf.Clamp(value, MinSensitivity, MaxSensitivity));
                Apply();
            }
        }

        public static void Save() => PlayerPrefs.Save();

        /// <summary>Applies settings that have a global effect (volume) and notifies listeners.</summary>
        public static void Apply()
        {
            AudioListener.volume = Volume;
            Changed?.Invoke();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void ApplyOnStartup() => AudioListener.volume = Volume;
    }
}
