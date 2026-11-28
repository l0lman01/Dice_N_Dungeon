using DiceNDungeon.Core;
using DiceNDungeon.Inputs;
using UnityEngine;

namespace DiceNDungeon.Player
{
    /// <summary>
    /// Moves the CharacterController relative to the player's orientation (W = forward, A/D = strafe)
    /// and applies gravity. Other systems (roll, abilities) can temporarily take over horizontal
    /// movement with <see cref="SetOverride"/>.
    /// </summary>
    [RequireComponent(typeof(CharacterController), typeof(PlayerOrientation), typeof(PlayerStats))]
    public class PlayerMotor : MonoBehaviour
    {
        [Tooltip("Input source. Empty = looks for one on this GameObject.")]
        [SerializeField] private PlayerInputReader input; // where move input comes from

        [Tooltip("Gravity acceleration in m/s² (negative = down).")]
        [SerializeField] private float gravity = -9.81f; // m/s²

        [Tooltip("Small downward speed kept while grounded so the controller sticks to slopes and steps.")]
        [SerializeField] private float groundedStickSpeed = -2f; // m/s

        private CharacterController controller;
        private PlayerOrientation orientation;
        private PlayerStats stats;
        private float verticalVelocity;
        private Vector3? horizontalOverride;

        /// <summary>Velocity applied during the last frame (m/s).</summary>
        public Vector3 Velocity { get; private set; }
        public bool IsGrounded => controller.isGrounded;
        public bool HasOverride => horizontalOverride.HasValue;

        /// <summary>World direction the player is asking to move in (length 0..1).</summary>
        public Vector3 InputDirection => OrientationMath.InputToWorld(input != null ? input.Move : Vector2.zero, orientation.Forward);

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            orientation = GetComponent<PlayerOrientation>();
            stats = GetComponent<PlayerStats>();
            if (input == null) input = GetComponent<PlayerInputReader>();
        }

        /// <summary>Replaces input-driven horizontal velocity (m/s) until <see cref="ClearOverride"/>.</summary>
        public void SetOverride(Vector3 horizontalVelocity) => horizontalOverride = Vector3.ProjectOnPlane(horizontalVelocity, Vector3.up);

        public void ClearOverride() => horizontalOverride = null;

        private void Update()
        {
            Vector3 horizontal = horizontalOverride ?? InputDirection * stats.MoveSpeed.Value;

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = groundedStickSpeed;
            }
            verticalVelocity += gravity * Time.deltaTime;

            Velocity = horizontal + Vector3.up * verticalVelocity;
            controller.Move(Velocity * Time.deltaTime);
        }
    }
}
