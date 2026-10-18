using System;
using NUnit.Framework;

namespace Yamblatro.Core.Tests
{
    public class DiceStateTests
    {
        [Test]
        public void NewDice_KeepValuesAndStartUnlocked()
        {
            DiceState dice = new DiceState(new[] { 1, 2, 3, 4, 5 });

            Assert.AreEqual(DiceState.StandardDiceCount, dice.Count);
            Assert.AreEqual(3, dice.GetValue(2));
            for (int i = 0; i < dice.Count; i++)
            {
                Assert.IsFalse(dice.IsLocked(i));
            }
        }

        [Test]
        public void MoreThanFiveDice_AreAllowed()
        {
            DiceState dice = new DiceState(new[] { 1, 2, 3, 4, 5, 6 });

            Assert.AreEqual(6, dice.Count);
        }

        [TestCase(0)]
        [TestCase(7)]
        [TestCase(-3)]
        public void InvalidDieValue_Throws(int badValue)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new DiceState(new[] { 1, 2, badValue, 4, 5 }));

            DiceState dice = new DiceState(new[] { 1, 2, 3, 4, 5 });
            Assert.Throws<ArgumentOutOfRangeException>(() => dice.SetValue(0, badValue));
            Assert.AreEqual(1, dice.GetValue(0));
        }

        [Test]
        public void NullOrEmptyValues_Throw()
        {
            Assert.Throws<ArgumentNullException>(() => new DiceState(null));
            Assert.Throws<ArgumentException>(() => new DiceState(new int[0]));
        }

        [Test]
        public void LockingOneDie_DoesNotAffectTheOthers()
        {
            DiceState dice = new DiceState(new[] { 1, 2, 3, 4, 5 });

            dice.SetLocked(1, true);

            Assert.IsFalse(dice.IsLocked(0));
            Assert.IsTrue(dice.IsLocked(1));
            Assert.IsFalse(dice.IsLocked(2));

            dice.SetLocked(1, false);
            Assert.IsFalse(dice.IsLocked(1));
        }

        [Test]
        public void IndexOutOfRange_Throws()
        {
            DiceState dice = new DiceState(new[] { 1, 2, 3, 4, 5 });

            Assert.Throws<ArgumentOutOfRangeException>(() => dice.GetValue(5));
            Assert.Throws<ArgumentOutOfRangeException>(() => dice.SetLocked(-1, true));
        }

        [Test]
        public void ChangingTheInputArrayAfterwards_DoesNotChangeTheDice()
        {
            int[] input = { 1, 2, 3, 4, 5 };
            DiceState dice = new DiceState(input);

            input[0] = 6;

            Assert.AreEqual(1, dice.GetValue(0));
        }

        [Test]
        public void Values_CanBeScored()
        {
            DiceState dice = new DiceState(new[] { 2, 2, 4, 4, 1 });

            Assert.IsTrue(YambScoring.TryCalculateScore(ScoreCategory.TwoPairs, dice.Values, out int score));
            Assert.AreEqual(22, score);
        }
    }
}
