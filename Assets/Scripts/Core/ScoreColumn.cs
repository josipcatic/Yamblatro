namespace Yamblatro.Core
{
    /// <summary>
    /// The four scorecard columns. The numeric value of each member is its
    /// column index (0-3).
    /// </summary>
    public enum ScoreColumn
    {
        /// <summary>Filled from Ones to Yamb; only the next row is legal.</summary>
        Downward = 0,

        /// <summary>Filled from Yamb to Ones; only the next row is legal.</summary>
        Upward = 1,

        /// <summary>Any unfilled row may be chosen.</summary>
        Free = 2,

        /// <summary>
        /// The row is called after the first throw and must then be filled that
        /// turn. The commitment is turn logic and is not stored on the scorecard.
        /// </summary>
        Call = 3
    }
}
