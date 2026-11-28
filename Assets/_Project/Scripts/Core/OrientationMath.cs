using UnityEngine;

namespace DiceNDungeon.Core
{
    /// <summary>Pure math helpers for the third-person controls (unit tested).</summary>
    public static class OrientationMath
    {
        /// <summary>
        /// Projects a direction (e.g. the camera forward) on the ground plane and normalizes it,
        /// so pitch is ignored. Returns <paramref name="fallback"/> when the direction is (almost) vertical.
        /// </summary>
        public static Vector3 FlatForward(Vector3 direction, Vector3 fallback)
        {
            Vector3 flat = Vector3.ProjectOnPlane(direction, Vector3.up);
            return flat.sqrMagnitude < 0.0001f ? fallback : flat.normalized;
        }

        /// <summary>
        /// Converts a 2D move input (x = strafe, y = forward) into a world direction relative to
        /// <paramref name="forward"/>. Magnitude is clamped to 1 so diagonals are not faster.
        /// </summary>
        public static Vector3 InputToWorld(Vector2 input, Vector3 forward)
        {
            Vector3 flatForward = FlatForward(forward, Vector3.forward);
            Vector3 right = Vector3.Cross(Vector3.up, flatForward);
            Vector3 world = flatForward * input.y + right * input.x;
            return Vector3.ClampMagnitude(world, 1f);
        }
    }
}
