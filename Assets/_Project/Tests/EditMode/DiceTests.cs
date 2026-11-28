using DiceNDungeon.Dice;
using NUnit.Framework;
using UnityEngine;

namespace DiceNDungeon.Tests
{
    public class DiceTests
    {
        [Test]
        public void DieRoller_SameSeed_GivesSameSequence()
        {
            var a = new DieRoller(42);
            var b = new DieRoller(42);
            for (int i = 0; i < 20; i++)
            {
                Assert.AreEqual(a.RollIndex(6), b.RollIndex(6));
            }
        }

        [Test]
        public void DieRoller_StaysInRange_AndHitsEveryFace()
        {
            var roller = new DieRoller(1);
            var seen = new bool[6];
            for (int i = 0; i < 600; i++)
            {
                int index = roller.RollIndex(6);
                Assert.That(index, Is.InRange(0, 5));
                seen[index] = true;
            }
            CollectionAssert.DoesNotContain(seen, false);
        }

        [Test]
        public void RotationForFaceUp_PutsThatFaceOnTop([NUnit.Framework.Range(0, 5)] int faceIndex)
        {
            Quaternion rotation = DieOrientation.RotationForFaceUp(faceIndex);

            Assert.AreEqual(faceIndex, DieOrientation.FaceUp(rotation));
            Vector3 up = rotation * DieOrientation.FaceAxes[faceIndex];
            Assert.That(Vector3.Dot(up, Vector3.up), Is.GreaterThan(0.999f));
        }

        [Test]
        public void OppositeFaces_AreOppositeAxes()
        {
            // index i and 5 - i are values v and 7 - v, like a real d6.
            for (int i = 0; i < 3; i++)
            {
                Assert.AreEqual(-DieOrientation.FaceAxes[i], DieOrientation.FaceAxes[5 - i]);
            }
        }
    }
}
