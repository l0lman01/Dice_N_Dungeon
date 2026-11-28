using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceNDungeon.Inputs
{
    /// <summary>
    /// Reads the "Menu" action map (Pause, PressF). Used by the splash screen and the in-game menus.
    /// Like <see cref="PlayerInputReader"/>, it is the only place menus touch the Input System.
    /// </summary>
    public class MenuInputReader : MonoBehaviour
    {
        [Tooltip("Button action that opens/closes the pause menu.")]
        [SerializeField] private string pauseActionName = "Menu/Pause"; // path of the pause action

        [Tooltip("Button action used by the studio splash screen (the F key).")]
        [SerializeField] private string pressFActionName = "Menu/PressF"; // path of the 'press F' action

        private InputAction pauseAction;
        private InputAction pressFAction;

        public event Action PausePressed;
        public event Action PressFPressed;

        private void OnEnable()
        {
            InputActionAsset actions = InputSystem.actions;
            if (actions == null)
            {
                Debug.LogError("No project-wide Input Actions assigned (Project Settings > Input System Package).", this);
                return;
            }

            pauseAction = actions.FindAction(pauseActionName, throwIfNotFound: false);
            pressFAction = actions.FindAction(pressFActionName, throwIfNotFound: false);

            if (pauseAction != null) { pauseAction.Enable(); pauseAction.performed += OnPause; }
            else Debug.LogWarning($"Input action '{pauseActionName}' not found.", this);

            if (pressFAction != null) { pressFAction.Enable(); pressFAction.performed += OnPressF; }
            else Debug.LogWarning($"Input action '{pressFActionName}' not found.", this);
        }

        private void OnDisable()
        {
            if (pauseAction != null) pauseAction.performed -= OnPause;
            if (pressFAction != null) pressFAction.performed -= OnPressF;
        }

        private void OnPause(InputAction.CallbackContext _) => PausePressed?.Invoke();
        private void OnPressF(InputAction.CallbackContext _) => PressFPressed?.Invoke();
    }
}
