using System;
using System.Collections.Generic;

namespace Yamblatro.Core
{
    /// <summary>
    /// Pure scoring functions. Nothing here reads or changes scorecard or
    /// player state.
    ///
    /// Invalid input (null dice, fewer than five dice, a die outside 1-6, an
    /// unknown category) throws. A roll that is valid but does not form the
    /// requested combination is not an error: TryCalculateScore returns false.
    /// What to write in the scorecard for that case is an open question
    /// (Decisions.md, question 2), so it is deliberately not decided here.
    /// </summary>
    public static class YambScoring
    {
        // Fixed bonuses added to the dice that form the combination.
        public const int TwoPairsBonus = 10;
        public const int ThreeOfAKindBonus = 20;
        public const int FullHouseBonus = 40;
        public const int PokerBonus = 50;
        public const int YambBonus = 60;

        // Straights score a fixed amount with no dice sum.
        public const int SmallStraightScore = 45;
        public const int BigStraightScore = 50;

        // Per-column bonus for the Ones-Sixes subtotal.
        public const int UpperSectionBonusThreshold = 60;
        public const int UpperSectionBonus = 30;

        /// <summary>Fewest dice a roll may contain.</summary>
        public const int MinimumDiceCount = DiceState.StandardDiceCount;

        // Maximum and Minimum always use the five highest / lowest dice.
        private const int MaximumMinimumDiceCount = 5;

        // Small straight is 1-2-3-4-5, big straight is 2-3-4-5-6.
        private const int StraightLength = 5;
        private const int SmallStraightLowestFace = 1;
        private const int BigStraightLowestFace = 2;

        /// <summary>True for Ones through Sixes.</summary>
        public static bool IsUpperSectionCategory(ScoreCategory category)
        {
            return category >= ScoreCategory.Ones && category <= ScoreCategory.Sixes;
        }

        /// <summary>
        /// Calculates the score a roll earns in a category. Returns false (and
        /// sets score to 0, which is meaningless in that case) when the roll
        /// does not form the combination. Returns true with a possibly zero
        /// score otherwise; Ones-Sixes, Maximum and Minimum are always scorable.
        ///
        /// When a roll with extra dice could form a combination in several ways,
        /// the highest-scoring way is used. Straight is only defined for exactly
        /// five dice until the extra-dice rule is decided (GameRules.md, section 9).
        /// </summary>
        public static bool TryCalculateScore(ScoreCategory category, IReadOnlyList<int> dice, out int score)
        {
            int[] counts = CountFaces(dice);
            score = 0;

            if (IsUpperSectionCategory(category))
            {
                // Ones is face 1, Twos is face 2, and so on.
                int face = (int)category + 1;
                score = face * counts[face];
                return true;
            }

            switch (category)
            {
                case ScoreCategory.Maximum:
                    score = SumHighest(counts, MaximumMinimumDiceCount);
                    return true;

                case ScoreCategory.Minimum:
                    score = SumLowest(counts, MaximumMinimumDiceCount);
                    return true;

                case ScoreCategory.TwoPairs:
                    return TryScoreTwoPairs(counts, out score);

                case ScoreCategory.ThreeOfAKind:
                    return TryScoreOfAKind(counts, 3, ThreeOfAKindBonus, out score);

                case ScoreCategory.Straight:
                    if (dice.Count != DiceState.StandardDiceCount)
                    {
                        throw new ArgumentException(
                            "Straight scoring is only defined for exactly five dice.", nameof(dice));
                    }
                    return TryScoreStraight(counts, out score);

                case ScoreCategory.FullHouse:
                    return TryScoreFullHouse(counts, out score);

                case ScoreCategory.Poker:
                    return TryScoreOfAKind(counts, 4, PokerBonus, out score);

                case ScoreCategory.Yamb:
                    return TryScoreOfAKind(counts, 5, YambBonus, out score);

                default:
                    throw new ArgumentOutOfRangeException(nameof(category), category, "Unknown score category.");
            }
        }

        /// <summary>Bonus for one column, given the total of its Ones-Sixes cells.</summary>
        public static int CalculateUpperSectionBonus(int upperSectionSubtotal)
        {
            return upperSectionSubtotal >= UpperSectionBonusThreshold ? UpperSectionBonus : 0;
        }

        /// <summary>
        /// Final-score contribution of one column's Ones, Maximum and Minimum cells.
        /// The caller must only use this when all three cells are filled.
        /// </summary>
        public static int CalculateMaxMinContribution(int onesScore, int maximumScore, int minimumScore)
        {
            return (maximumScore - minimumScore) * onesScore;
        }

        // Validates the dice and returns how many dice show each face (index 1-6).
        private static int[] CountFaces(IReadOnlyList<int> dice)
        {
            if (dice == null)
            {
                throw new ArgumentNullException(nameof(dice));
            }
            if (dice.Count < MinimumDiceCount)
            {
                throw new ArgumentException(
                    "At least " + MinimumDiceCount + " dice are required.", nameof(dice));
            }

            int[] counts = new int[DiceState.MaxFaceValue + 1];
            for (int i = 0; i < dice.Count; i++)
            {
                int value = dice[i];
                if (!DiceState.IsValidFaceValue(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(dice), value, "Die values must be between 1 and 6.");
                }
                counts[value]++;
            }
            return counts;
        }

        private static int SumHighest(int[] counts, int diceToSum)
        {
            int sum = 0;
            int remaining = diceToSum;
            for (int face = DiceState.MaxFaceValue; face >= DiceState.MinFaceValue && remaining > 0; face--)
            {
                int taken = Math.Min(counts[face], remaining);
                sum += taken * face;
                remaining -= taken;
            }
            return sum;
        }

        private static int SumLowest(int[] counts, int diceToSum)
        {
            int sum = 0;
            int remaining = diceToSum;
            for (int face = DiceState.MinFaceValue; face <= DiceState.MaxFaceValue && remaining > 0; face++)
            {
                int taken = Math.Min(counts[face], remaining);
                sum += taken * face;
                remaining -= taken;
            }
            return sum;
        }

        // Highest face shown at least minCount times, skipping excludedFace
        // (pass 0 to skip nothing). Returns 0 if there is none.
        private static int HighestFaceWithAtLeast(int[] counts, int minCount, int excludedFace)
        {
            for (int face = DiceState.MaxFaceValue; face >= DiceState.MinFaceValue; face--)
            {
                if (face != excludedFace && counts[face] >= minCount)
                {
                    return face;
                }
            }
            return 0;
        }

        // Three of a Kind, Poker and Yamb: sum of the matching dice plus a bonus.
        private static bool TryScoreOfAKind(int[] counts, int matchingDice, int bonus, out int score)
        {
            score = 0;
            int face = HighestFaceWithAtLeast(counts, matchingDice, 0);
            if (face == 0)
            {
                return false;
            }
            score = face * matchingDice + bonus;
            return true;
        }

        // Two different faces, each shown at least twice. A single face shown
        // four times does not count (Decisions.md, question 3, is still open).
        private static bool TryScoreTwoPairs(int[] counts, out int score)
        {
            score = 0;
            int firstFace = HighestFaceWithAtLeast(counts, 2, 0);
            if (firstFace == 0)
            {
                return false;
            }
            int secondFace = HighestFaceWithAtLeast(counts, 2, firstFace);
            if (secondFace == 0)
            {
                return false;
            }
            score = 2 * (firstFace + secondFace) + TwoPairsBonus;
            return true;
        }

        // Three dice of one face plus two dice of a different face.
        private static bool TryScoreFullHouse(int[] counts, out int score)
        {
            score = 0;
            int tripleFace = HighestFaceWithAtLeast(counts, 3, 0);
            if (tripleFace == 0)
            {
                return false;
            }
            int pairFace = HighestFaceWithAtLeast(counts, 2, tripleFace);
            if (pairFace == 0)
            {
                return false;
            }
            score = 3 * tripleFace + 2 * pairFace + FullHouseBonus;
            return true;
        }

        private static bool TryScoreStraight(int[] counts, out int score)
        {
            if (HasRun(counts, BigStraightLowestFace))
            {
                score = BigStraightScore;
                return true;
            }
            if (HasRun(counts, SmallStraightLowestFace))
            {
                score = SmallStraightScore;
                return true;
            }
            score = 0;
            return false;
        }

        // True if every face from lowestFace up to lowestFace + StraightLength - 1 appears.
        private static bool HasRun(int[] counts, int lowestFace)
        {
            for (int face = lowestFace; face < lowestFace + StraightLength; face++)
            {
                if (counts[face] == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
