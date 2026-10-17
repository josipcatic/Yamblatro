# Decisions, Assumptions and Open Questions

This file is the decision log for rules and research design. Update it
when a rule is confirmed or changed.

## Confirmed decisions

### Scorecard

-   [x] Four columns and fourteen rows.
-   [x] Row order: Ones, Twos, Threes, Fours, Fives, Sixes, Maximum,
    Minimum, Two Pairs, Three of a Kind, Straight, Full House, Poker,
    Yamb.
-   [x] Downward column is filled from Ones to Yamb.
-   [x] Upward column is filled from Yamb to Ones.
-   [x] Free column allows any unfilled row.
-   [x] Call column allows a category to be called after the first
    throw; the called row must be filled even if the score is zero.
-   [x] Initial dice count is five.
-   [x] Up to three throws per turn; the player may stop earlier.
-   [x] The player can lock selected dice and reroll the others.
-   [x] A recorded zero is different from an empty cell.

### Scoring

-   [x] Ones--Sixes score only dice matching the category's face value.
-   [x] Maximum is the sum of the five highest dice.
-   [x] Minimum is the sum of the five lowest dice.
-   [x] Two Pairs = sum of the four dice forming two distinct pairs +
    10.
-   [x] Three of a Kind = sum of the three matching dice + 20.
-   [x] Straight = 45 for small straight and 50 for big straight, with
    no additional sum.
-   [x] Full House = sum of all five dice + 40.
-   [x] Poker = sum of the four matching dice + 50.
-   [x] Yamb = sum of the five matching dice + 60.
-   [x] Add 30 to a column if its Ones--Sixes subtotal is at least 60.
-   [x] When Ones, Maximum and Minimum are all filled, the final score
    includes `(Maximum - Minimum) * Ones` for that column.
-   [x] The Maximum/Minimum multiplier is a final-score calculation, not
    part of the score written into either cell.
-   [x] Future extra dice should improve combination chances without
    automatically contributing to every combination's score.

### Architecture and research

-   [x] Core rules should be independent of Unity presentation.
-   [x] The same engine should power the client and headless simulation.
-   [x] Scoring and turn rules should be tested before building the UI.
-   [x] Simulation data should include reproducible configurations and
    seeds.
-   [x] Start with simple tuning of numerical effects and prices; do not
    build an unnecessarily complex optimizer first.

## Open questions to resolve before implementing the related rules

1.  **Straight definitions:** Which five-die sequence is the small
    straight and which is the big straight? Are the accepted sequences,
    for example, `1-2-3-4-5` and `2-3-4-5-6`, or does your local ruleset
    distinguish them differently?
2.  **Invalid combinations:** In a non-called category, if the roll does
    not meet the combination requirement, may the player still write
    zero? The initial implementation should not guess if your table
    rules differ.
3.  **Two Pairs and four-of-a-kind:** Can four matching dice count as
    two pairs, or must the two pairs have different face values?
4.  **Call timing and target:** Can a player call only one category per
    turn? Is calling permitted only when that row is still available in
    the Call column? Can a call be made immediately after the first
    throw, before any reroll?
5.  **Call column order:** Confirm that the Call column itself has no
    fixed row order; the called category determines the row.
6.  **Final-score display:** Should the upper-section bonus and
    Maximum/Minimum multiplier be shown as separate bonus lines, even
    though they contribute to the final total?
7.  **Match end:** Confirm that a match ends when every player has
    filled all 56 cells. If you intend a different multiplayer match
    length, record it here.
8.  **Player count:** Confirm the initial number of players for the
    first complete runner (two is a useful starting point).

## Later design questions --- not required for the initial rules engine

-   What constitutes a round for shop timing?
-   Does every player get the same number of turns per round?
-   Which modifiers are permanent, round-limited or use-limited?
-   What is the starting amount of money, and how is money earned?
-   What exactly is the modifier-balancing objective?
-   Which AI strategy will be held out from the tuning process?
-   Is online multiplayer feasible within the schedule, or should local
    multiplayer be the reliable fallback?

## Change procedure

When changing a confirmed rule:

1.  Update `GameRules.md`.
2.  Update this decision log.
3.  Add or update tests that express the new rule.
4.  Record a short reason and date in the change history below.

## Change history

-   Initial specification created: 2026-10-17.
