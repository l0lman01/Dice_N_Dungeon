using UnityEngine;

namespace DiceNDungeon.Dice
{
    /// <summary>
    /// Maps a face index to the local axis of the die mesh that carries it, and gives the
    /// rotation that puts that face on top. Convention (opposite faces sum to 7, like a real d6):
    /// index 0 = value 1 = +Y, 1 = 2 = +Z, 2 = 3 = +X, 3 = 4 = -X, 4 = 5 = -Z, 5 = 6 = -Y.
    /// If your die model is authored differently, change <see cref="FaceAxes"/> only.
    /// </summary>
    public static class DieOrientation
    {
        public static readonly Vector3[] FaceAxes =
        {
            Vector3.up,
            Vector3.forward,
            Vector3.right,
            Vector3.left,
            Vector3.back,
            Vector3.down,
        };

        /// <summary>Local rotation for the die visual so that face <paramref name="faceIndex"/> points up.</summary>
        public static Quaternion RotationForFaceUp(int faceIndex)
        {
            Vector3 axis = FaceAxes[Mathf.Clamp(faceIndex, 0, FaceAxes.Length - 1)];
            return Quaternion.FromToRotation(axis, Vector3.up);
        }

        /// <summary>Which face index points up for a given rotation (the axis closest to world up).</summary>
        public static int FaceUp(Quaternion rotation)
        {
            int best = 0;
            float bestDot = float.MinValue;
            for (int i = 0; i < FaceAxes.Length; i++)
            {
                float dot = Vector3.Dot(rotation * FaceAxes[i], Vector3.up);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    best = i;
                }
            }
            return best;
        }
    }
}
