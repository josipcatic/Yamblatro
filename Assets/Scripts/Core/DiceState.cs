using System;
using System.Collections.Generic;

namespace Yamblatro.Core
{
    /// <summary>
    /// The current dice values and which dice are locked. The number of dice is
    /// not fixed, so extra dice can be supported later. This class does not
    /// roll dice; values are supplied by the caller.
    /// </summary>
    public class DiceState
    {
        /// <summary>Number of dice in the initial game.</summary>
        public const int StandardDiceCount = 5;

        public const int MinFaceValue = 1;
        public const int MaxFaceValue = 6;

        private readonly int[] values;
        private readonly bool[] locked;
        private readonly IReadOnlyList<int> readOnlyValues;

        /// <summary>Creates dice with the given values, all unlocked.</summary>
        public DiceState(IReadOnlyList<int> initialValues)
        {
            if (initialValues == null)
            {
                throw new ArgumentNullException(nameof(initialValues));
            }
            if (initialValues.Count == 0)
            {
                throw new ArgumentException("At least one die is required.", nameof(initialValues));
            }

            values = new int[initialValues.Count];
            locked = new bool[initialValues.Count];
            for (int i = 0; i < values.Length; i++)
            {
                if (!IsValidFaceValue(initialValues[i]))
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(initialValues), initialValues[i], "Die values must be between 1 and 6.");
                }
                values[i] = initialValues[i];
            }
            readOnlyValues = Array.AsReadOnly(values);
        }

        public int Count
        {
            get { return values.Length; }
        }

        /// <summary>Read-only view of the dice values, usable with <see cref="YambScoring"/>.</summary>
        public IReadOnlyList<int> Values
        {
            get { return readOnlyValues; }
        }

        public static bool IsValidFaceValue(int value)
        {
            return value >= MinFaceValue && value <= MaxFaceValue;
        }

        public int GetValue(int index)
        {
            CheckIndex(index);
            return values[index];
        }

        public void SetValue(int index, int value)
        {
            CheckIndex(index);
            if (!IsValidFaceValue(value))
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, "Die values must be between 1 and 6.");
            }
            values[index] = value;
        }

        public bool IsLocked(int index)
        {
            CheckIndex(index);
            return locked[index];
        }

        public void SetLocked(int index, bool isLocked)
        {
            CheckIndex(index);
            locked[index] = isLocked;
        }

        private void CheckIndex(int index)
        {
            if (index < 0 || index >= values.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "There is no die at this index.");
            }
        }
    }
}
