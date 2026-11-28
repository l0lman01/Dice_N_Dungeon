using DiceNDungeon.Core;
using NUnit.Framework;

namespace DiceNDungeon.Tests
{
    public class StatTests
    {
        [Test]
        public void Value_WithoutModifiers_IsBaseValue()
        {
            var stat = new Stat(6f);
            Assert.AreEqual(6f, stat.Value, 1e-5f);
        }

        [Test]
        public void Value_AppliesFlatThenPercentAddThenPercentMult()
        {
            var stat = new Stat(10f);
            stat.AddModifier(new StatModifier(StatModifierType.Flat, 2f), "a");        // 12
            stat.AddModifier(new StatModifier(StatModifierType.PercentAdd, 0.25f), "a"); // summed: x1.5
            stat.AddModifier(new StatModifier(StatModifierType.PercentAdd, 0.25f), "b");
            stat.AddModifier(new StatModifier(StatModifierType.PercentMult, 1f), "b");   // x2

            Assert.AreEqual(12f * 1.5f * 2f, stat.Value, 1e-4f);
        }

        [Test]
        public void RemoveAllFromSource_RemovesOnlyThatSource()
        {
            var stat = new Stat(10f);
            object swift = new object();
            object other = new object();
            stat.AddModifier(new StatModifier(StatModifierType.PercentAdd, 0.3f), swift);
            stat.AddModifier(new StatModifier(StatModifierType.Flat, 1f), other);

            int removed = stat.RemoveAllFromSource(swift);

            Assert.AreEqual(1, removed);
            Assert.AreEqual(11f, stat.Value, 1e-5f);
        }

        [Test]
        public void Changed_IsRaisedOnAddAndRemove()
        {
            var stat = new Stat(1f);
            int calls = 0;
            stat.Changed += _ => calls++;

            stat.AddModifier(new StatModifier(StatModifierType.Flat, 1f), this);
            stat.RemoveAllFromSource(this);

            Assert.AreEqual(2, calls);
        }
    }
}
