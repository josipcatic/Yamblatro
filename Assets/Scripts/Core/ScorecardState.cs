using System;

namespace Yamblatro.Core
{
    /// <summary>
    /// One player's scorecard: 4 columns x 14 rows. A cell is either empty or
    /// holds a recorded score (which may be zero).
    ///
    /// This class stores data and answers questions about it. Choosing which
    /// cell a player may write to this turn, including the Call column's
    /// commitment, belongs to the turn rules, which should use CanSelect.
    /// </summary>
    public class ScorecardState
    {
        public const int ColumnCount = 4;
        public const int CategoryCount = 14;

        // null means the cell is empty; 0 means it was filled with a zero score.
        private readonly int?[,] cells = new int?[ColumnCount, CategoryCount];

        public bool IsFilled(ScoreColumn column, ScoreCategory category)
        {
            ValidateCell(column, category);
            return cells[(int)column, (int)category].HasValue;
        }

        /// <summary>
        /// Returns true and the recorded score if the cell is filled. Returns
        /// false for an empty cell (score is then 0 and must be ignored).
        /// </summary>
        public bool TryGetScore(ScoreColumn column, ScoreCategory category, out int score)
        {
            ValidateCell(column, category);
            int? recorded = cells[(int)column, (int)category];
            score = recorded.GetValueOrDefault();
            return recorded.HasValue;
        }

        /// <summary>
        /// Writes a score into an empty cell. Throws if the cell is already
        /// filled or the score is negative. It does not check column order;
        /// callers should check CanSelect first.
        /// </summary>
        public void Record(ScoreColumn column, ScoreCategory category, int score)
        {
            ValidateCell(column, category);
            if (score < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(score), score, "A recorded score cannot be negative.");
            }
            if (cells[(int)column, (int)category].HasValue)
            {
                throw new InvalidOperationException(
                    "The " + column + " / " + category + " cell is already filled.");
            }
            cells[(int)column, (int)category] = score;
        }

        /// <summary>
        /// True if the cell is empty and the column's order allows it now.
        /// Downward: every earlier row is filled. Upward: every later row is
        /// filled. Free and Call: any empty cell.
        /// </summary>
        public bool CanSelect(ScoreColumn column, ScoreCategory category)
        {
            if (IsFilled(column, category))
            {
                return false;
            }

            int columnIndex = (int)column;
            int categoryIndex = (int)category;

            switch (column)
            {
                case ScoreColumn.Downward:
                    for (int row = 0; row < categoryIndex; row++)
                    {
                        if (!cells[columnIndex, row].HasValue)
                        {
                            return false;
                        }
                    }
                    return true;

                case ScoreColumn.Upward:
                    for (int row = CategoryCount - 1; row > categoryIndex; row--)
                    {
                        if (!cells[columnIndex, row].HasValue)
                        {
                            return false;
                        }
                    }
                    return true;

                default:
                    return true;
            }
        }

        /// <summary>
        /// Upper-section bonus for one column: 30 if its Ones-Sixes cells add up
        /// to 60 or more, otherwise 0. Empty cells count as 0. This is never
        /// stored in a cell.
        /// </summary>
        public int GetUpperSectionBonus(ScoreColumn column)
        {
            ValidateColumn(column);
            int subtotal = 0;
            for (int row = (int)ScoreCategory.Ones; row <= (int)ScoreCategory.Sixes; row++)
            {
                subtotal += cells[(int)column, row].GetValueOrDefault();
            }
            return YambScoring.CalculateUpperSectionBonus(subtotal);
        }

        /// <summary>
        /// Maximum/Minimum multiplier for one column: (Maximum - Minimum) * Ones
        /// when all three cells are filled, otherwise 0. A filled zero counts as
        /// filled. This is never stored in a cell.
        /// </summary>
        public int GetMaxMinContribution(ScoreColumn column)
        {
            ValidateColumn(column);
            int? ones = cells[(int)column, (int)ScoreCategory.Ones];
            int? maximum = cells[(int)column, (int)ScoreCategory.Maximum];
            int? minimum = cells[(int)column, (int)ScoreCategory.Minimum];

            if (!ones.HasValue || !maximum.HasValue || !minimum.HasValue)
            {
                return 0;
            }
            return YambScoring.CalculateMaxMinContribution(ones.Value, maximum.Value, minimum.Value);
        }

        /// <summary>One column's filled cells plus its upper-section bonus and Max/Min contribution.</summary>
        public int GetColumnTotal(ScoreColumn column)
        {
            ValidateColumn(column);
            int total = 0;
            for (int row = 0; row < CategoryCount; row++)
            {
                total += cells[(int)column, row].GetValueOrDefault();
            }
            return total + GetUpperSectionBonus(column) + GetMaxMinContribution(column);
        }

        /// <summary>The scorecard's current total across all four columns.</summary>
        public int GetTotal()
        {
            int total = 0;
            for (int column = 0; column < ColumnCount; column++)
            {
                total += GetColumnTotal((ScoreColumn)column);
            }
            return total;
        }

        private static void ValidateColumn(ScoreColumn column)
        {
            if ((int)column < 0 || (int)column >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException(nameof(column), column, "Unknown score column.");
            }
        }

        private static void ValidateCell(ScoreColumn column, ScoreCategory category)
        {
            ValidateColumn(column);
            if ((int)category < 0 || (int)category >= CategoryCount)
            {
                throw new ArgumentOutOfRangeException(nameof(category), category, "Unknown score category.");
            }
        }
    }
}
