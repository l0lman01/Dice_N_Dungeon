using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceNDungeon.Inputs
{
    /// <summary>Turns gameplay input on/off (pause, game over, menus) and handles the mouse cursor.</summary>
    public static class InputMaps
    {
        private const string GameplayMap = "Player";

        /// <summary>
        /// Enables or disables the "Player" action map. Disabling it also stops the Cinemachine camera
        /// from reading Look, so the camera freezes while a menu is open.
        /// </summary>
        public static void SetGameplayEnabled(bool enabled)
        {
            InputActionMap map = InputSystem.actions != null ? InputSystem.actions.FindActionMap(GameplayMap) : null;
            if (map == null) return;
            if (enabled) map.Enable();
            else map.Disable();
        }

        /// <summary>Locked + hidden cursor for gameplay, free + visible cursor for menus.</summary>
        public static void SetCursorLocked(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }
    }
}
