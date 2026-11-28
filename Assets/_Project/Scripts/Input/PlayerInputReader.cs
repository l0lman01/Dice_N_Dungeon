using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DiceNDungeon.Inputs
{
    /// <summary>
    /// The only class that talks to the Input System. Other components read <see cref="Move"/>
    /// or subscribe to the events, so swapping or disabling input happens in one place.
    /// Uses the project-wide actions (Project Settings > Input System Package).
    /// </summary>
    public class PlayerInputReader : MonoBehaviour
    {
        [Header("Action names (map/action in InputSystem_Actions)")]
        [Tooltip("Vector2 action used to move (x = strafe, y = forward).")]
        [SerializeField] private string moveActionName = "Player/Move"; // path of the move action

        [Tooltip("Button action that triggers the dodge roll.")]
        [SerializeField] private string rollActionName = "Player/Roll"; // path of the roll action

        [Tooltip("Button action that fires the active ability of the current die face.")]
        [SerializeField] private string abilityActionName = "Player/Attack"; // path of the ability action

        [Tooltip("Button action used to interact with the world.")]
        [SerializeField] private string interactActionName = "Player/Interact"; // path of the interact action

        private InputAction moveAction;
        private InputAction rollAction;
        private InputAction abilityAction;
        private InputAction interactAction;

        /// <summary>Current move input, (0,0) when input is disabled.</summary>
        public Vector2 Move => isActiveAndEnabled && moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;

        public event Action RollPressed;
        public event Action AbilityPressed;
        public event Action InteractPressed;

        private void OnEnable()
        {
            InputActionAsset actions = InputSystem.actions;
            if (actions == null)
            {
                Debug.LogError("No project-wide Input Actions assigned (Project Settings > Input System Package).", this);
                return;
            }

            moveAction = actions.FindAction(moveActionName, throwIfNotFound: false);
            rollAction = actions.FindAction(rollActionName, throwIfNotFound: false);
            abilityAction = actions.FindAction(abilityActionName, throwIfNotFound: false);
            interactAction = actions.FindAction(interactActionName, throwIfNotFound: false);

            if (moveAction == null) Debug.LogWarning($"Input action '{moveActionName}' not found.", this);
            Subscribe(rollAction, OnRoll, rollActionName);
            Subscribe(abilityAction, OnAbility, abilityActionName);
            Subscribe(interactAction, OnInteract, interactActionName);
        }

        private void OnDisable()
        {
            if (rollAction != null) rollAction.performed -= OnRoll;
            if (abilityAction != null) abilityAction.performed -= OnAbility;
            if (interactAction != null) interactAction.performed -= OnInteract;
        }

        private void Subscribe(InputAction action, Action<InputAction.CallbackContext> callback, string actionName)
        {
            if (action == null)
            {
                Debug.LogWarning($"Input action '{actionName}' not found.", this);
                return;
            }
            action.performed += callback;
        }

        private void OnRoll(InputAction.CallbackContext _) => RollPressed?.Invoke();
        private void OnAbility(InputAction.CallbackContext _) => AbilityPressed?.Invoke();
        private void OnInteract(InputAction.CallbackContext _) => InteractPressed?.Invoke();
    }
}
