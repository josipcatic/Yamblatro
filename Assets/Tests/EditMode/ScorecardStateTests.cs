using System;
using NUnit.Framework;

namespace Yamblatro.Core.Tests
{
    public class ScorecardStateTests
    {
        private ScorecardState card;

        [SetUp]
        public void SetUp()
        {
            card = new ScorecardState();
        }

        // Fills Ones-Sixes in one column so they add up to the given subtotal.
        private void FillUpperSection(ScoreColumn column, int subtotal)
        {
            card.Record(column, ScoreCategory.Ones, 3);
            card.Record(column, ScoreCategory.Twos, 6);
            card.Record(column, ScoreCategory.Threes, 9);
            card.Record(column, ScoreCategory.Fours, 12);
            card.Record(column, ScoreCategory.Fives, 15);
            card.Record(column, ScoreCategory.Sixes, subtotal - 45);
        }

        [Test]
        public void Dimensions_MatchTheEnums()
        {
            Assert.AreEqual(ScorecardState.ColumnCount, Enum.GetValues(typeof(ScoreColumn)).Length);
            Assert.AreEqual(ScorecardState.CategoryCount, Enum.GetValues(typeof(ScoreCategory)).Length);
        }

        // ---- Empty versus zero ----

        [Test]
        public void NewCell_IsEmpty()
        {
            Assert.IsFalse(card.IsFilled(ScoreColumn.Free, ScoreCategory.Yamb));
            Assert.IsFalse(card.TryGetScore(ScoreColumn.Free, ScoreCategory.Yamb, out int score));
        }

        [Test]
        public void RecordedZero_IsFilledAndDifferentFromEmpty()
        {
            card.Record(ScoreColumn.Free, ScoreCategory.Yamb, 0);

            Assert.IsTrue(card.IsFilled(ScoreColumn.Free, ScoreCategory.Yamb));
            Assert.IsTrue(card.TryGetScore(ScoreColumn.Free, ScoreCategory.Yamb, out int score));
            Assert.AreEqual(0, score);
        }

        [Test]
        public void RecordedPositiveScore_CanBeRead()
        {
            card.Record(ScoreColumn.Downward, ScoreCategory.Ones, 4);

            Assert.IsTrue(card.TryGetScore(ScoreColumn.Downward, ScoreCategory.Ones, out int score));
            Assert.AreEqual(4, score);
        }

        [Test]
        public void Cells_AreIndependentAcrossColumns()
        {
            card.Record(ScoreColumn.Free, ScoreCategory.Ones, 3);

            Assert.IsFalse(card.IsFilled(ScoreColumn.Downward, ScoreCategory.Ones));
            Assert.IsFalse(card.IsFilled(ScoreColumn.Upward, ScoreCategory.Ones));
            Assert.IsFalse(card.IsFilled(ScoreColumn.Call, ScoreCategory.Ones));
        }

        // ---- No overwriting ----

        [Test]
        public void Record_FilledPositiveCell_ThrowsAndKeepsOriginalScore()
        {
            card.Record(ScoreColumn.Free, ScoreCategory.Fours, 12);

            Assert.Throws<InvalidOperationException>(
                () => card.Record(ScoreColumn.Free, ScoreCategory.Fours, 8));

            card.TryGetScore(ScoreColumn.Free, ScoreCategory.Fours, out int score);
            Assert.AreEqual(12, score);
        }

        [Test]
        public void Record_FilledZeroCell_ThrowsAndKeepsZero()
        {
            card.Record(ScoreColumn.Free, ScoreCategory.Fours, 0);

            Assert.Throws<InvalidOperationException>(
                () => card.Record(ScoreColumn.Free, ScoreCategory.Fours, 8));

            card.TryGetScore(ScoreColumn.Free, ScoreCategory.Fours, out int score);
            Assert.AreEqual(0, score);
        }

        [Test]
        public void Record_NegativeScore_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => card.Record(ScoreColumn.Free, ScoreCategory.Fours, -1));
            Assert.IsFalse(card.IsFilled(ScoreColumn.Free, ScoreCategory.Fours));
        }

        [Test]
        public void UnknownColumnOrCategory_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => card.IsFilled((ScoreColumn)9, ScoreCategory.Ones));
            Assert.Throws<ArgumentOutOfRangeException>(
                () => card.Record(ScoreColumn.Free, (ScoreCategory)99, 1));
        }

        // ---- Which cells can be selected ----

        [Test]
        public void CanSelect_Downward_OnlyTheNextRowFromTheTop()
        {
            Assert.IsTrue(card.CanSelect(ScoreColumn.Downward, ScoreCategory.Ones));
            Assert.IsFalse(card.CanSelect(ScoreColumn.Downward, ScoreCategory.Twos));

            card.Record(ScoreColumn.Downward, ScoreCategory.Ones, 3);

            Assert.IsFalse(card.CanSelect(ScoreColumn.Downward, ScoreCategory.Ones));
            Assert.IsTrue(card.CanSelect(ScoreColumn.Downward, ScoreCategory.Twos));
            Assert.IsFalse(card.CanSelect(ScoreColumn.Downward, ScoreCategory.Threes));
        }

        [Test]
        public void CanSelect_Upward_OnlyTheNextRowFromTheBottom()
        {
            Assert.IsTrue(card.CanSelect(ScoreColumn.Upward, ScoreCategory.Yamb));
            Assert.IsFalse(card.CanSelect(ScoreColumn.Upward, ScoreCategory.Poker));

            card.Record(ScoreColumn.Upward, ScoreCategory.Yamb, 0);

            Assert.IsFalse(card.CanSelect(ScoreColumn.Upward, ScoreCategory.Yamb));
            Assert.IsTrue(card.CanSelect(ScoreColumn.Upward, ScoreCategory.Poker));
            Assert.IsFalse(card.CanSelect(ScoreColumn.Upward, ScoreCategory.FullHouse));
        }

        [Test]
        public void CanSelect_Free_AnyUnfilledRow()
        {
            Assert.IsTrue(card.CanSelect(ScoreColumn.Free, ScoreCategory.Straight));

            card.Record(ScoreColumn.Free, ScoreCategory.Straight, 45);

            Assert.IsFalse(card.CanSelect(ScoreColumn.Free, ScoreCategory.Straight));
            Assert.IsTrue(card.CanSelect(ScoreColumn.Free, ScoreCategory.Yamb));
        }

        [Test]
        public void CanSelect_Call_AnyUnfilledRow()
        {
            Assert.IsTrue(card.CanSelect(ScoreColumn.Call, ScoreCategory.Minimum));

            card.Record(ScoreColumn.Call, ScoreCategory.Minimum, 10);

            Assert.IsFalse(card.CanSelect(ScoreColumn.Call, ScoreCategory.Minimum));
        }

        // ---- Totals ----

        [Test]
        public void EmptyCard_TotalIsZero()
        {
            Assert.AreEqual(0, card.GetTotal());
        }

        [Test]
        public void Total_SumsFilledCells()
        {
            card.Record(ScoreColumn.Free, ScoreCategory.Ones, 3);
            card.Record(ScoreColumn.Free, ScoreCategory.Twos, 6);
            card.Record(ScoreColumn.Free, ScoreCategory.Yamb, 0);

            Assert.AreEqual(9, card.GetTotal());
        }

        // ---- Upper-section bonus ----

        [TestCase(59, 59)]
        [TestCase(60, 90)]
        [TestCase(75, 105)]
        public void UpperSectionBonus_AtAndAroundSixty(int subtotal, int expectedColumnTotal)
        {
            FillUpperSection(ScoreColumn.Free, subtotal);

            Assert.AreEqual(expectedColumnTotal, card.GetColumnTotal(ScoreColumn.Free));
        }

        [Test]
        public void UpperSectionBonus_IsNotWrittenIntoACell()
        {
            FillUpperSection(ScoreColumn.Free, 60);

            card.TryGetScore(ScoreColumn.Free, ScoreCategory.Sixes, out int sixes);
            Assert.AreEqual(15, sixes);
            Assert.AreEqual(30, card.GetUpperSectionBonus(ScoreColumn.Free));
        }

        // ---- Maximum/Minimum multiplier ----

        [Test]
        public void MaxMinMultiplier_AllThreeFilled_IsApplied()
        {
            card.Record(ScoreColumn.Free, ScoreCategory.Ones, 4);
            card.Record(ScoreColumn.Free, ScoreCategory.Maximum, 25);
            card.Record(ScoreColumn.Free, ScoreCategory.Minimum, 8);

            Assert.AreEqual(68, card.GetMaxMinContribution(ScoreColumn.Free));
            Assert.AreEqual(4 + 25 + 8 + 68, card.GetColumnTotal(ScoreColumn.Free));
        }

        [Test]
        public void MaxMinMultiplier_DoesNotChangeTheMaximumOrMinimumCells()
        {
            card.Record(ScoreColumn.Free, ScoreCategory.Ones, 4);
            card.Record(ScoreColumn.Free, ScoreCategory.Maximum, 25);
            card.Record(ScoreColumn.Free, ScoreCategory.Minimum, 8);

            card.TryGetScore(ScoreColumn.Free, ScoreCategory.Maximum, out int maximum);
            card.TryGetScore(ScoreColumn.Free, ScoreCategory.Minimum, out int minimum);
            Assert.AreEqual(25, maximum);
            Assert.AreEqual(8, minimum);
        }

        [TestCase(ScoreCategory.Ones)]
        [TestCase(ScoreCategory.Maximum)]
        [TestCase(ScoreCategory.Minimum)]
        public void MaxMinMultiplier_OneRequiredCellEmpty_IsNotApplied(ScoreCategory emptyCategory)
        {
            ScoreCategory[] required = { ScoreCategory.Ones, ScoreCategory.Maximum, ScoreCategory.Minimum };
            int[] scores = { 4, 25, 8 };
            int expectedTotal = 0;
            for (int i = 0; i < required.Length; i++)
            {
                if (required[i] != emptyCategory)
                {
                    card.Record(ScoreColumn.Free, required[i], scores[i]);
                    expectedTotal += scores[i];
                }
            }

            Assert.AreEqual(0, card.GetMaxMinContribution(ScoreColumn.Free));
            Assert.AreEqual(expectedTotal, card.GetColumnTotal(ScoreColumn.Free));
        }

        [Test]
        public void MaxMinMultiplier_FilledZeroOnes_GivesZeroContribution()
        {
            card.Record(ScoreColumn.Free, ScoreCategory.Ones, 0);
            card.Record(ScoreColumn.Free, ScoreCategory.Maximum, 25);
            card.Record(ScoreColumn.Free, ScoreCategory.Minimum, 8);

            Assert.AreEqual(0, card.GetMaxMinContribution(ScoreColumn.Free));
            Assert.AreEqual(25 + 8, card.GetColumnTotal(ScoreColumn.Free));
        }

        // ---- Several columns ----

        [Test]
        public void BonusAndMultiplier_AreCalculatedPerColumn()
        {
            // Downward: upper section of exactly 60, so it earns the bonus.
            FillUpperSection(ScoreColumn.Downward, 60);

            // Upward: upper section of 59, no bonus.
            FillUpperSection(ScoreColumn.Upward, 59);

            // Free: Ones, Maximum and Minimum filled, so the multiplier applies.
            card.Record(ScoreColumn.Free, ScoreCategory.Ones, 4);
            card.Record(ScoreColumn.Free, ScoreCategory.Maximum, 25);
            card.Record(ScoreColumn.Free, ScoreCategory.Minimum, 8);

            // Call: left empty.

            Assert.AreEqual(60 + 30, card.GetColumnTotal(ScoreColumn.Downward));
            Assert.AreEqual(59, card.GetColumnTotal(ScoreColumn.Upward));
            Assert.AreEqual(4 + 25 + 8 + 68, card.GetColumnTotal(ScoreColumn.Free));
            Assert.AreEqual(0, card.GetColumnTotal(ScoreColumn.Call));

            Assert.AreEqual(30, card.GetUpperSectionBonus(ScoreColumn.Downward));
            Assert.AreEqual(0, card.GetUpperSectionBonus(ScoreColumn.Upward));
            Assert.AreEqual(0, card.GetMaxMinContribution(ScoreColumn.Downward));
            Assert.AreEqual(68, card.GetMaxMinContribution(ScoreColumn.Free));

            Assert.AreEqual(90 + 59 + 105, card.GetTotal());
        }
    }
}
