using System;
using NUnit.Framework;

namespace Yamblatro.Core.Tests
{
    public class YambScoringTests
    {
        private static readonly ScoreCategory[] AllCategories =
            (ScoreCategory[])Enum.GetValues(typeof(ScoreCategory));

        // Scores a roll that must form the combination.
        private static int Score(ScoreCategory category, params int[] dice)
        {
            bool formsCombination = YambScoring.TryCalculateScore(category, dice, out int score);
            Assert.IsTrue(formsCombination,
                category + " should be scorable for " + string.Join(",", dice));
            return score;
        }

        // Checks a valid roll that must NOT form the combination.
        private static void AssertNoCombination(ScoreCategory category, params int[] dice)
        {
            bool formsCombination = YambScoring.TryCalculateScore(category, dice, out int score);
            Assert.IsFalse(formsCombination,
                category + " should not be scorable for " + string.Join(",", dice));
        }

        // ---- Ones to Sixes ----

        [TestCase(ScoreCategory.Ones, new[] { 1, 1, 3, 5, 6 }, 2)]
        [TestCase(ScoreCategory.Twos, new[] { 2, 2, 2, 5, 6 }, 6)]
        [TestCase(ScoreCategory.Threes, new[] { 3, 3, 1, 2, 6 }, 6)]
        [TestCase(ScoreCategory.Fours, new[] { 4, 4, 4, 4, 1 }, 16)]
        [TestCase(ScoreCategory.Fives, new[] { 5, 1, 2, 3, 4 }, 5)]
        [TestCase(ScoreCategory.Sixes, new[] { 6, 6, 6, 6, 6 }, 30)]
        public void UpperSection_SumsOnlyMatchingDice(ScoreCategory category, int[] dice, int expected)
        {
            Assert.AreEqual(expected, Score(category, dice));
        }

        [Test]
        public void UpperSection_NoMatchingDice_IsAValidZero()
        {
            Assert.AreEqual(0, Score(ScoreCategory.Sixes, 1, 2, 3, 4, 5));
        }

        [Test]
        public void UpperSection_MoreThanFiveDice_SumsAllMatchingDice()
        {
            Assert.AreEqual(3, Score(ScoreCategory.Ones, 1, 1, 1, 2, 3, 4, 5));
        }

        // ---- Maximum and Minimum ----

        [Test]
        public void Maximum_SumsAllFiveDice()
        {
            Assert.AreEqual(16, Score(ScoreCategory.Maximum, 1, 2, 3, 4, 6));
            Assert.AreEqual(30, Score(ScoreCategory.Maximum, 6, 6, 6, 6, 6));
        }

        [Test]
        public void Minimum_SumsAllFiveDice()
        {
            Assert.AreEqual(16, Score(ScoreCategory.Minimum, 1, 2, 3, 4, 6));
            Assert.AreEqual(5, Score(ScoreCategory.Minimum, 1, 1, 1, 1, 1));
        }

        [Test]
        public void Maximum_MoreThanFiveDice_UsesFiveHighest()
        {
            // 2 + 3 + 4 + 5 + 6; the 1 is dropped.
            Assert.AreEqual(20, Score(ScoreCategory.Maximum, 1, 2, 3, 4, 5, 6));
        }

        [Test]
        public void Minimum_MoreThanFiveDice_UsesFiveLowest()
        {
            // 1 + 2 + 3 + 4 + 5; the 6 is dropped.
            Assert.AreEqual(15, Score(ScoreCategory.Minimum, 1, 2, 3, 4, 5, 6));
        }

        // ---- Two Pairs ----

        [Test]
        public void TwoPairs_SumsOnlyTheFourPairDicePlusBonus()
        {
            Assert.AreEqual(22, Score(ScoreCategory.TwoPairs, 2, 2, 4, 4, 1));
            Assert.AreEqual(22, Score(ScoreCategory.TwoPairs, 4, 1, 2, 4, 2));
            Assert.AreEqual(32, Score(ScoreCategory.TwoPairs, 6, 6, 5, 5, 1));
        }

        [Test]
        public void TwoPairs_MoreThanFiveDice_IgnoresExtraDice()
        {
            Assert.AreEqual(22, Score(ScoreCategory.TwoPairs, 2, 2, 4, 4, 1, 6));
        }

        [Test]
        public void TwoPairs_InvalidCombinations()
        {
            AssertNoCombination(ScoreCategory.TwoPairs, 1, 2, 3, 4, 5);
            AssertNoCombination(ScoreCategory.TwoPairs, 1, 1, 2, 3, 4);
            AssertNoCombination(ScoreCategory.TwoPairs, 3, 3, 3, 1, 2);
        }

        // ---- Three of a Kind ----

        [Test]
        public void ThreeOfAKind_SumsOnlyTheThreeMatchingDicePlusBonus()
        {
            Assert.AreEqual(29, Score(ScoreCategory.ThreeOfAKind, 3, 3, 3, 1, 2));
            Assert.AreEqual(38, Score(ScoreCategory.ThreeOfAKind, 2, 6, 6, 1, 6));
        }

        [Test]
        public void ThreeOfAKind_MoreThanFiveDice_IgnoresExtraDice()
        {
            Assert.AreEqual(29, Score(ScoreCategory.ThreeOfAKind, 3, 3, 3, 1, 2, 6));
        }

        [Test]
        public void ThreeOfAKind_InvalidCombinations()
        {
            AssertNoCombination(ScoreCategory.ThreeOfAKind, 1, 2, 3, 4, 5);
            AssertNoCombination(ScoreCategory.ThreeOfAKind, 3, 3, 1, 1, 2);
        }

        // ---- Straight ----

        [Test]
        public void Straight_SmallIsOneToFive()
        {
            Assert.AreEqual(45, Score(ScoreCategory.Straight, 1, 2, 3, 4, 5));
            Assert.AreEqual(45, Score(ScoreCategory.Straight, 5, 3, 1, 2, 4));
        }

        [Test]
        public void Straight_BigIsTwoToSix()
        {
            Assert.AreEqual(50, Score(ScoreCategory.Straight, 2, 3, 4, 5, 6));
            Assert.AreEqual(50, Score(ScoreCategory.Straight, 6, 2, 5, 3, 4));
        }

        [Test]
        public void Straight_InvalidCombinations()
        {
            AssertNoCombination(ScoreCategory.Straight, 1, 2, 3, 4, 6);
            AssertNoCombination(ScoreCategory.Straight, 1, 3, 4, 5, 6);
            AssertNoCombination(ScoreCategory.Straight, 1, 1, 2, 3, 4);
            AssertNoCombination(ScoreCategory.Straight, 2, 2, 3, 4, 5);
        }

        [Test]
        public void Straight_MoreThanFiveDice_IsRejectedUntilTheRuleIsDecided()
        {
            Assert.Throws<ArgumentException>(
                () => YambScoring.TryCalculateScore(ScoreCategory.Straight, new[] { 1, 2, 3, 4, 5, 6 }, out int score));
        }

        // ---- Full House ----

        [Test]
        public void FullHouse_SumsAllFiveDicePlusBonus()
        {
            Assert.AreEqual(56, Score(ScoreCategory.FullHouse, 2, 2, 2, 5, 5));
            Assert.AreEqual(56, Score(ScoreCategory.FullHouse, 5, 2, 5, 2, 2));
            Assert.AreEqual(60, Score(ScoreCategory.FullHouse, 6, 6, 6, 1, 1));
        }

        [Test]
        public void FullHouse_MoreThanFiveDice_IgnoresExtraDice()
        {
            Assert.AreEqual(56, Score(ScoreCategory.FullHouse, 2, 2, 2, 5, 5, 1));
        }

        [Test]
        public void FullHouse_InvalidCombinations()
        {
            AssertNoCombination(ScoreCategory.FullHouse, 1, 2, 3, 4, 5);
            AssertNoCombination(ScoreCategory.FullHouse, 1, 1, 2, 3, 4);
            AssertNoCombination(ScoreCategory.FullHouse, 2, 2, 2, 3, 4);
            AssertNoCombination(ScoreCategory.FullHouse, 2, 2, 3, 3, 4);
        }

        // ---- Poker ----

        [Test]
        public void Poker_SumsOnlyTheFourMatchingDicePlusBonus()
        {
            Assert.AreEqual(66, Score(ScoreCategory.Poker, 4, 4, 4, 4, 5));
            Assert.AreEqual(54, Score(ScoreCategory.Poker, 1, 6, 1, 1, 1));
        }

        [Test]
        public void Poker_MoreThanFiveDice_IgnoresExtraDice()
        {
            Assert.AreEqual(66, Score(ScoreCategory.Poker, 4, 4, 4, 4, 5, 1));
        }

        [Test]
        public void Poker_InvalidCombinations()
        {
            AssertNoCombination(ScoreCategory.Poker, 4, 4, 4, 5, 5);
            AssertNoCombination(ScoreCategory.Poker, 1, 2, 3, 4, 5);
            AssertNoCombination(ScoreCategory.Poker, 2, 2, 2, 3, 6);
        }

        // ---- Yamb ----

        [Test]
        public void Yamb_SumsFiveMatchingDicePlusBonus()
        {
            Assert.AreEqual(90, Score(ScoreCategory.Yamb, 6, 6, 6, 6, 6));
            Assert.AreEqual(65, Score(ScoreCategory.Yamb, 1, 1, 1, 1, 1));
        }

        [Test]
        public void Yamb_MoreThanFiveDice_IgnoresExtraDice()
        {
            Assert.AreEqual(75, Score(ScoreCategory.Yamb, 3, 3, 3, 3, 3, 6));
        }

        [Test]
        public void Yamb_InvalidCombinations()
        {
            AssertNoCombination(ScoreCategory.Yamb, 6, 6, 6, 6, 5);
            AssertNoCombination(ScoreCategory.Yamb, 1, 2, 3, 4, 5);
        }

        // ---- Invalid input is rejected, not scored ----

        [TestCaseSource(nameof(AllCategories))]
        public void DieValueBelowOne_Throws(ScoreCategory category)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => YambScoring.TryCalculateScore(category, new[] { 0, 1, 2, 3, 4 }, out int score));
        }

        [TestCaseSource(nameof(AllCategories))]
        public void DieValueAboveSix_Throws(ScoreCategory category)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => YambScoring.TryCalculateScore(category, new[] { 1, 2, 3, 4, 7 }, out int score));
        }

        [TestCaseSource(nameof(AllCategories))]
        public void NegativeDieValue_Throws(ScoreCategory category)
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => YambScoring.TryCalculateScore(category, new[] { -1, 1, 2, 3, 4 }, out int score));
        }

        [TestCaseSource(nameof(AllCategories))]
        public void FewerThanFiveDice_Throws(ScoreCategory category)
        {
            Assert.Throws<ArgumentException>(
                () => YambScoring.TryCalculateScore(category, new[] { 1, 2, 3, 4 }, out int score));
            Assert.Throws<ArgumentException>(
                () => YambScoring.TryCalculateScore(category, new int[0], out int score));
        }

        [TestCaseSource(nameof(AllCategories))]
        public void NullDice_Throws(ScoreCategory category)
        {
            Assert.Throws<ArgumentNullException>(
                () => YambScoring.TryCalculateScore(category, null, out int score));
        }

        [Test]
        public void UnknownCategory_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => YambScoring.TryCalculateScore((ScoreCategory)99, new[] { 1, 2, 3, 4, 5 }, out int score));
        }

        // ---- Bonus and multiplier formulas ----

        [TestCase(0, 0)]
        [TestCase(59, 0)]
        [TestCase(60, 30)]
        [TestCase(61, 30)]
        [TestCase(100, 30)]
        public void UpperSectionBonus_AppliesFromSixty(int subtotal, int expectedBonus)
        {
            Assert.AreEqual(expectedBonus, YambScoring.CalculateUpperSectionBonus(subtotal));
        }

        [Test]
        public void MaxMinContribution_IsMaximumMinusMinimumTimesOnes()
        {
            Assert.AreEqual(68, YambScoring.CalculateMaxMinContribution(4, 25, 8));
        }

        [Test]
        public void MaxMinContribution_ZeroOnes_IsZero()
        {
            Assert.AreEqual(0, YambScoring.CalculateMaxMinContribution(0, 25, 8));
        }
    }
}
