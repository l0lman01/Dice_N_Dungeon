using DiceNDungeon.Core;
using NUnit.Framework;
using UnityEngine;

namespace DiceNDungeon.Tests
{
    public class OrientationMathTests
    {
        private static void AssertApprox(Vector3 expected, Vector3 actual)
        {
            Assert.That(Vector3.Distance(expected, actual), Is.LessThan(1e-4f), $"expected {expected}, got {actual}");
        }

        [Test]
        public void FlatForward_IgnoresCameraPitch()
        {
            // Camera looking down at 45° toward +Z.
            Vector3 cameraForward = new Vector3(0f, -1f, 1f).normalized;
            AssertApprox(Vector3.forward, OrientationMath.FlatForward(cameraForward, Vector3.right));
        }

        [Test]
        public void FlatForward_StraightDown_ReturnsFallback()
        {
            AssertApprox(Vector3.right, OrientationMath.FlatForward(Vector3.down, Vector3.right));
        }

        [Test]
        public void InputToWorld_ForwardAndStrafe_AreRelativeToForward()
        {
            Vector3 forward = Vector3.right; // character faces +X
            AssertApprox(Vector3.right, OrientationMath.InputToWorld(new Vector2(0f, 1f), forward));
            AssertApprox(Vector3.back, OrientationMath.InputToWorld(new Vector2(1f, 0f), forward)); // D = strafe right
        }

        [Test]
        public void InputToWorld_Diagonal_IsNotFaster()
        {
            Vector3 world = OrientationMath.InputToWorld(new Vector2(1f, 1f), Vector3.forward);
            Assert.That(world.magnitude, Is.LessThanOrEqualTo(1f + 1e-5f));
        }
    }
}
