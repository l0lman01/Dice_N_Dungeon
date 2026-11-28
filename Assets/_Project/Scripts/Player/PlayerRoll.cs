using System;
using System.Collections;
using DiceNDungeon.Combat;
using DiceNDungeon.Inputs;
using UnityEngine;

namespace DiceNDungeon.Player
{
    /// <summary>
    /// Dodge roll: a short burst of movement with invulnerability frames while the die visual tumbles.
    /// When the roll ends, <see cref="RollEnded"/> fires and <see cref="PlayerDie"/> picks the new face.
    /// </summary>
    [RequireComponent(typeof(PlayerMotor))]
    public class PlayerRoll : MonoBehaviour, IInvulnerabilitySource
    {
        [Header("References")]
        [Tooltip("Input source. Empty = looks for one on this GameObject.")]
        [SerializeField] private PlayerInputReader input; // listens to RollPressed

        [Tooltip("Child holding the die mesh; it tumbles during the roll. The root/collider never rotates.")]
        [SerializeField] private Transform dieVisual; // mesh rotated for feedback

        [Header("Movement")]
        [Tooltip("Distance covered by one roll, in meters (with a flat speed curve).")]
        [SerializeField] private float rollDistance = 4f; // meters

        [Tooltip("Duration of the roll in seconds.")]
        [SerializeField] private float rollDuration = 0.35f; // seconds

        [Tooltip("Speed over the roll: X = normalized time (0-1), Y = multiplier of the average speed.")]
        [SerializeField] private AnimationCurve speedCurve = AnimationCurve.Constant(0f, 1f, 1f); // speed profile

        [Tooltip("Seconds after a roll ends before the next one is allowed.")]
        [SerializeField] private float cooldown = 0.5f; // seconds

        [Tooltip("If true, the player can only roll while grounded.")]
        [SerializeField] private bool requireGrounded = true; // blocks air rolls

        [Header("Invulnerability")]
        [Tooltip("Part of the roll (normalized time 0-1) during which damage is ignored.")]
        [SerializeField] private Vector2 invulnerabilityWindow = new(0f, 0.8f); // i-frames start/end

        [Header("Visual")]
        [Tooltip("Degrees the die visual tumbles forward during the roll.")]
        [SerializeField] private float tumbleDegrees = 360f; // visual rotation amount

        private PlayerMotor motor;
        private float nextRollTime;

        public bool IsRolling { get; private set; }
        public bool IsInvulnerable { get; private set; }
        public float CooldownRemaining => Mathf.Max(0f, nextRollTime - Time.time);
        public float Cooldown => cooldown;

        public event Action RollStarted;
        public event Action RollEnded;

        private void Awake()
        {
            motor = GetComponent<PlayerMotor>();
            if (input == null) input = GetComponent<PlayerInputReader>();
        }

        private void OnEnable()
        {
            if (input != null) input.RollPressed += TryRoll;
        }

        private void OnDisable()
        {
            if (input != null) input.RollPressed -= TryRoll;
            if (IsRolling) EndRoll();
        }

        public bool CanRoll => !IsRolling && Time.time >= nextRollTime && (!requireGrounded || motor.IsGrounded);

        public void TryRoll()
        {
            if (CanRoll) StartCoroutine(RollRoutine());
        }

        private IEnumerator RollRoutine()
        {
            IsRolling = true;
            RollStarted?.Invoke();

            // Roll toward the input direction, or straight ahead if there is no input.
            Vector3 direction = motor.InputDirection;
            if (direction.sqrMagnitude < 0.01f) direction = transform.forward;
            direction.Normalize();

            float averageSpeed = rollDistance / Mathf.Max(0.01f, rollDuration);
            Vector3 tumbleAxis = Vector3.Cross(Vector3.up, direction);
            Quaternion visualStart = dieVisual != null ? dieVisual.rotation : Quaternion.identity;

            float elapsed = 0f;
            while (elapsed < rollDuration)
            {
                float t = elapsed / rollDuration;
                motor.SetOverride(direction * (averageSpeed * speedCurve.Evaluate(t)));
                IsInvulnerable = t >= invulnerabilityWindow.x && t <= invulnerabilityWindow.y;
                if (dieVisual != null) dieVisual.rotation = Quaternion.AngleAxis(tumbleDegrees * t, tumbleAxis) * visualStart;

                elapsed += Time.deltaTime;
                yield return null;
            }

            EndRoll();
        }

        private void EndRoll()
        {
            motor.ClearOverride();
            IsInvulnerable = false;
            IsRolling = false;
            nextRollTime = Time.time + cooldown;
            RollEnded?.Invoke();
        }
    }
}
