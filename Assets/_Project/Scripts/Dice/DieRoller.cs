using System;

namespace DiceNDungeon.Dice
{
    /// <summary>
    /// Picks a random face. Pure C# with a seedable RNG, so tests (and replays) are deterministic.
    /// </summary>
    public class DieRoller
    {
        private readonly Random random;

        /// <param name="seed">Same seed = same sequence of rolls. Null = time-based random.</param>
        public DieRoller(int? seed = null)
        {
            random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        /// <summary>Returns a face index in [0, faceCount).</summary>
        public int RollIndex(int faceCount)
        {
            if (faceCount <= 0) throw new ArgumentOutOfRangeException(nameof(faceCount), "A die needs at least one face.");
            return random.Next(faceCount);
        }
    }
}
