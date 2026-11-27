using DiceNDungeon.Core;
using UnityEngine;

namespace DiceNDungeon.Player
{
    /// <summary>
    /// Turns the player to face the camera's forward direction projected on the ground (pitch ignored).
    /// Runs before <see cref="PlayerMotor"/> so movement uses this frame's orientation.
    /// </summary>
    [DefaultExecutionOrder(-50)]
    public class PlayerOrientation : MonoBehaviour
    {
        [Tooltip("Camera whose forward the player follows. Empty = Camera.main (driven by the Cinemachine Brain).")]
        [SerializeField] private Transform cameraTransform; // reference camera

        [Tooltip("Turn speed in degrees per second. 0 = snap instantly to the camera direction.")]
        [SerializeField] private float turnSpeed = 0f; // deg/s, 0 = instant

        public Vector3 Forward => transform.forward;
        public Vector3 Right => transform.right;

        private void Awake()
        {
            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            if (cameraTransform == null) return;

            Vector3 targetForward = OrientationMath.FlatForward(cameraTransform.forward, transform.forward);
            Quaternion target = Quaternion.LookRotation(targetForward, Vector3.up);
            transform.rotation = turnSpeed <= 0f
                ? target
                : Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
        }
    }
}
