# Simulation and Balancing Experiment Design

## Research objective

Use automated matches to evaluate modifier strength and selection
behavior, then tune modifier parameters and prices while checking that
the result generalizes to an AI strategy not used during tuning.

The simulator should use the same game engine and scoring rules as the
client. A separate simplified implementation of the rules would
undermine the validity of the results.

## Phases

### Phase 1 --- Baseline engine

-   Run complete matches with no shop or modifiers.
-   Validate scoring, turn transitions and match termination.
-   Add a simple legal-action AI.
-   Record seeds and final scores.

### Phase 2 --- Modifier framework and shop

-   Implement a shop between rounds.
-   Give each player an independent offer generated under the same
    offer-generation rules.
-   Let players buy an offered modifier or save their money.
-   Carry unspent money between shop visits unless later rules specify
    otherwise.
-   Support permanent, round-limited and use-limited modifiers only
    where required by the actual modifier set.
-   Log offers, purchases, prices, triggers and remaining uses.

### Phase 3 --- AI strategies

Implement at least two meaningfully different AI approaches, for
example:

-   **Greedy AI:** chooses an action based on immediate expected score
    or a simple heuristic.
-   **Monte Carlo AI:** estimates candidate actions by sampling possible
    future dice outcomes.

Difficulty should not be represented only by changing a random
percentage if the research depends on different decision quality. Record
each agent's strategy and parameters.

### Phase 4 --- Evaluation

For each modifier and configuration, measure:

-   Win rate.
-   Average final score and score difference.
-   Purchase or selection frequency.
-   Average value associated with owning or activating the modifier.
-   Results by AI strategy and skill level.
-   Economy indicators, including money accumulated and unspent.
-   Frequency of modifier triggers and whether an effect reaches its cap
    or runs out.

Define the exact meaning of "modifier contribution" before running final
experiments. A modifier's raw average score is not the same as its
causal effect on winning; controlled comparisons are needed.

### Phase 5 --- Automatic tuning

Begin with a small, interpretable tuner that adjusts numerical effects
and prices. Keep rarity and duration fixed initially so that the
experiment does not change too many dimensions at once.

A possible initial objective is to reduce extreme modifier imbalance
while preserving meaningful choices. Do not assume that every modifier
should have identical win rate or purchase frequency: situational and
defensive effects may be valuable in specific matchups.

Compare baseline and tuned configurations using the same sets of seeds
where feasible. Use separate tuning and evaluation data.

### Phase 6 --- Held-out validation

Evaluate the tuned configuration with an AI strategy that was not used
during tuning. This tests whether the balancing result generalizes
beyond the behaviors the tuner saw.

The held-out agent should differ meaningfully in its decision method,
not merely use a different parameter value for the same policy.

## Experimental controls

-   Record a unique experiment ID.
-   Save the full modifier configuration, prices, rarity settings, AI
    types and parameters.
-   Record random seeds.
-   Use paired seeds for comparisons when the setup allows a fair
    comparison.
-   Keep baseline results immutable.
-   Record code version or Git commit for each result set.
-   Separate raw results from processed summaries and figures.
-   Repeat experiments sufficiently to estimate uncertainty; report
    sample sizes and confidence intervals where appropriate.

## Example output schema

A per-match record may include:

-   `experiment_id`
-   `config_id`
-   `seed`
-   `player_1_agent`
-   `player_2_agent`
-   `player_1_score`
-   `player_2_score`
-   `winner`
-   `match_length`
-   `player_1_money_remaining`
-   `player_2_money_remaining`

A per-purchase record may include:

-   `experiment_id`
-   `match_id`
-   `round`
-   `player_id`
-   `modifier_id`
-   `price`
-   `was_purchased`

A per-trigger record may include:

-   `experiment_id`
-   `match_id`
-   `round`
-   `player_id`
-   `modifier_id`
-   `trigger_type`
-   `effect_value`

Use structured records or CSV for exported data. Do not rely on console
logs as the only research output.

## Balancing interpretation

Win rate alone is insufficient. A modifier may be strong only for one AI
strategy, expensive enough to be fair, or valuable mainly as a counter
to another modifier.

Evaluate outcomes by matchup and strategy, and examine both modifier use
and match outcomes. Document the chosen balance criterion before
optimizing. Avoid changing the criterion after seeing the final results
without reporting that change.
