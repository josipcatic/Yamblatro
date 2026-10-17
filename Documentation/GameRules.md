# Yamb Game Rules --- Initial Specification

**Status:** Initial implementation specification. Rules marked as
unresolved in `Decisions.md` must be confirmed before the related code
is finalized.

## 1. Game setup

-   A player has a scorecard with **4 columns and 14 rows**, for 56
    cells total.
-   The initial game uses **5 six-sided dice**.
-   Each player fills one scorecard cell per turn.
-   A filled cell cannot be used again.
-   A cell can contain a score of `0`; this is different from an empty
    cell.
-   A match is complete when all required cells on every player's
    scorecard are filled.

## 2. Scorecard rows

The displayed order is:

1.  Ones
2.  Twos
3.  Threes
4.  Fours
5.  Fives
6.  Sixes
7.  Maximum
8.  Minimum
9.  Two Pairs
10. Three of a Kind
11. Straight
12. Full House
13. Poker
14. Yamb

## 3. Scorecard columns

### Column 1 --- Downward

Fill rows in order, from **Ones through Yamb**. The next row is the only
legal choice in this column.

### Column 2 --- Upward

Fill rows in reverse order, from **Yamb through Ones**. The next row is
the only legal choice in this column.

### Column 3 --- Free order

Any unfilled row can be selected.

### Column 4 --- Call

A player may call a category **after the first throw**. Once called,
that category is committed for the turn: the player must fill that row
in the Call column, even if the resulting score is zero.

A call does not mean the player can choose a different category at the
end of the turn.

## 4. Turn and dice rules

1.  The active player starts a turn by rolling all available dice.
2.  The player may lock any dice they want to keep.
3.  The player may reroll unlocked dice.
4.  A turn permits at most **three throws total**. The first roll counts
    as throw one.
5.  The player may stop after one or two throws; using all three is not
    mandatory.
6.  A call may be made only after the first throw and before the turn's
    final scorecard entry.
7.  After the final throw, the player chooses a legal scorecard cell,
    unless the call already committed the target cell.
8.  The score is written to the selected cell.
9.  The turn ends and control passes to the next player.

The game engine must reject actions that violate the current turn state,
such as a fourth throw, locking dice that do not exist, or changing a
called category.

## 5. Category scoring

The score entered into a cell is calculated from the dice that form the
relevant category. Extra dice must not automatically contribute to a
combination's score.

### Ones through Sixes

Sum only the dice showing the category's face value.

Example: for Ones, a roll of `1, 1, 3, 5, 6` scores `2`.

### Maximum

Sum the **five highest dice** in the roll.

### Minimum

Sum the **five lowest dice** in the roll.

In the initial five-dice game, Maximum and Minimum both use all five
dice. If additional dice are introduced later, only the selected five
count.

### Two Pairs

Requires two distinct pairs. Add the values of the four dice that form
the pairs, then add **10**. Do not include the fifth die in the base
score.

Example: `2, 2, 4, 4, 1` scores `2 + 2 + 4 + 4 + 10 = 22`.

A four-of-a-kind does not automatically count as two pairs unless that
rule is explicitly approved.

### Three of a Kind

Requires three dice with the same face value. Sum those three dice and
add **20**. Other dice do not contribute.

### Straight

-   Small straight: **45 points**.
-   Big straight: **50 points**.
-   No additional sum or bonus is added.

The exact sequences that count as small and big straights must be
confirmed in `Decisions.md` before finalizing the scoring tests.

### Full House

Requires a three-of-a-kind plus a separate pair, using all five dice.
Sum all five dice and add **40**.

### Poker

Requires four dice with the same face value. Sum only those four
matching dice and add **50**. Any remaining die is ignored.

Example: `4, 4, 4, 4, 5` scores `4 + 4 + 4 + 4 + 50 = 66`.

### Yamb

Requires five dice with the same face value. Sum those five matching
dice and add **60**.

If more than five dice are available in a future version, only the five
dice forming the Yamb count toward its score.

## 6. Upper-section bonus

For each column, add **30 points** if the total of its Ones, Twos,
Threes, Fours, Fives and Sixes cells is **60 or more**.

The bonus is evaluated per column, not across the whole scorecard.

## 7. Maximum/Minimum final-score multiplier

Maximum and Minimum cells store their own category scores. They are
**not** multiplied by Ones when the cell is first recorded.

When calculating a column's final score, if that column's Ones, Maximum
and Minimum cells are all filled, add this contribution:

`(Maximum score - Minimum score) * Ones score`

Use the values stored in those three cells. Do not recalculate the
multiplier using the dice from an earlier turn.

Example: Ones = 4, Maximum = 25, Minimum = 8. The additional final-score
contribution is `(25 - 8) * 4 = 68`.

If any of those three cells is empty, do not apply this contribution
yet. A filled cell containing zero is not empty.

## 8. Empty cells, zero scores and invalid combinations

-   Empty means the cell has not been used.
-   Zero means the cell has been used and its recorded score is zero.
-   A called category must be filled even when the player did not roll
    the required combination.
-   The engine must not treat a zero score as an unused cell.
-   The precise handling of a category that does not meet its
    combination requirements, when it was not called, must follow the
    decisions recorded in `Decisions.md`.

## 9. Future additional dice

Additional dice are a possible later upgrade, not part of the initial
implementation.

The intended design principle is that extra dice increase the chance of
rolling a desired combination, but do not automatically increase the
score by allowing every die to contribute. Category scoring selects the
dice that form the category:

-   Ones--Sixes: only matching face values count.
-   Maximum: the five highest dice count.
-   Minimum: the five lowest dice count.
-   Two Pairs: the four dice in the two pairs count.
-   Three of a Kind: the three matching dice count.
-   Straight: fixed score.
-   Full House: all five dice required for the combination count.
-   Poker: four matching dice count.
-   Yamb: five matching dice count.

The effect of additional dice on the exact definition of a straight and
on other edge cases must be decided before the upgrade is implemented.
