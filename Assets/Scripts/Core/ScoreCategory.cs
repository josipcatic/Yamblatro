namespace Yamblatro.Core
{
    /// <summary>
    /// The 14 scorecard rows, in display order. The numeric value of each
    /// member is its row index (0-13), so do not reorder them.
    /// </summary>
    public enum ScoreCategory
    {
        Ones = 0,
        Twos = 1,
        Threes = 2,
        Fours = 3,
        Fives = 4,
        Sixes = 5,
        Maximum = 6,
        Minimum = 7,
        TwoPairs = 8,
        ThreeOfAKind = 9,
        Straight = 10,
        FullHouse = 11,
        Poker = 12,
        Yamb = 13
    }
}
