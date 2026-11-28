using DiceNDungeon.Inputs;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DiceNDungeon.Core
{
    /// <summary>
    /// Scene changes and quitting. Always resets time scale and gameplay input first,
    /// so leaving a paused or game-over scene never leaves the game frozen.
    /// </summary>
    public static class GameFlow
    {
        public static void LoadScene(string sceneName)
        {
            ResetState();
            SceneManager.LoadScene(sceneName);
        }

        public static void ReloadCurrentScene() => LoadScene(SceneManager.GetActiveScene().name);

        public static void Quit()
        {
            GameSettings.Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public static void ResetState()
        {
            Time.timeScale = 1f;
            InputMaps.SetGameplayEnabled(true);
        }
    }
}
