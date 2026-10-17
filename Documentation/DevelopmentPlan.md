# Development Plan

The order below prioritizes correctness and research feasibility. Dates
are not fixed; each phase should be considered complete only when its
exit checks pass.

## Milestone 1 --- Rules and core model

Tasks:

-   [ ] Resolve scoring and call-related questions in `Decisions.md`.
-   [ ] Create the core state types.
-   [ ] Implement scorecard cell state, including empty versus recorded
    zero.
-   [ ] Implement category scoring as pure functions.
-   [ ] Implement upper-section bonus and final-score multiplier.

Exit checks:

-   All category scoring tests pass.
-   Empty cells and zero-valued cells behave differently.
-   Final-score calculations work for partially and fully filled
    scorecards.

## Milestone 2 --- Complete turn

Tasks:

-   [ ] Implement initial roll.
-   [ ] Implement locking and rerolling selected dice.
-   [ ] Enforce the three-throw limit.
-   [ ] Implement calling and call commitment.
-   [ ] Validate scorecard cell selection for all four columns.
-   [ ] Record the score and advance the active player.

Exit checks:

-   Legal actions succeed.
-   Illegal actions are rejected without corrupting state.
-   A complete turn can be played without loading a Unity scene.

## Milestone 3 --- Complete headless match

Tasks:

-   [ ] Implement match completion rules.
-   [ ] Add a seeded dice random source.
-   [ ] Add a simple legal-action agent.
-   [ ] Run a full match and report scores and winner.

Exit checks:

-   Matches terminate reliably.
-   Every recorded cell is valid and filled at most once.
-   A run can be reproduced from its seed and configuration.

## Milestone 4 --- AI and baseline metrics

Tasks:

-   [ ] Implement at least two meaningfully different AI approaches.
-   [ ] Record scores, winners, seeds and agent configurations.
-   [ ] Run baseline simulations without modifiers.
-   [ ] Export machine-readable results.

Exit checks:

-   Large batches can run without rendering.
-   Simulation outputs can be processed independently of the Unity UI.

## Milestone 5 --- Shop and modifiers

Tasks:

-   [ ] Define round boundaries and shop timing.
-   [ ] Implement independent player offers.
-   [ ] Implement buying and saving money.
-   [ ] Implement modifier ownership, trigger resolution and durations.
-   [ ] Add at least fifteen modifiers.
-   [ ] Log offers, purchases and effect activations.

Exit checks:

-   Modifier effects are deterministic given the same state and random
    inputs.
-   Modifier interactions have tests.
-   Shop events are recorded for research.

## Milestone 6 --- Evaluation and tuning

Tasks:

-   [ ] Define the balance criterion.
-   [ ] Implement controlled modifier comparisons.
-   [ ] Measure win rates, score differences, selection rates and
    strategy dependence.
-   [ ] Implement a first numerical parameter/price tuner.
-   [ ] Preserve baseline and tuning datasets.
-   [ ] Validate with a held-out AI.

Exit checks:

-   The tuning process is reproducible.
-   Results include sample sizes and uncertainty estimates.
-   Held-out validation is reported separately from tuning results.

## Milestone 7 --- Client and multiplayer integration

Build the minimum interface required to play and demonstrate the
project. Prototype networking early enough to make a realistic go/no-go
decision; retain local multiplayer as the fallback.

Avoid spending time on visual polish before the core, tests, headless
runner and research pipeline are dependable.

## Final milestone --- Thesis and delivery

-   [ ] Freeze the experiment configuration.
-   [ ] Run final experiments.
-   [ ] Export tables and figures.
-   [ ] Explain methods, limitations and results.
-   [ ] Test a clean build and document how to reproduce key results.
