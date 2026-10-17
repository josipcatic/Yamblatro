# Yamb --- Multiplayer Game and Simulation Research

## Overview

This project is a multiplayer implementation of Yamb, with a rules
engine designed to support automated match simulation and future
modifier-balancing experiments.

The research priority is **correct, testable game logic and reproducible
simulation data**. Visual polish is secondary to a reliable rules
engine, a headless match runner, and useful measurements.

## Current scope

The initial version focuses on:

-   A four-column, fourteen-category Yamb scorecard.
-   Turn-based play with five dice, up to three throws per turn, and
    dice locking.
-   Downward, upward, free-order, and call columns.
-   Automated tests for scoring, legal actions, turn transitions, and
    edge cases.
-   A complete match runner that works without rendering.
-   A basic AI opponent to prove that matches can run automatically.

Planned later work may include:

-   A shop between rounds.
-   At least fifteen modifiers, including economic, defensive,
    offensive, and personal-scoring effects.
-   Multiple AI strategies and skill levels.
-   Large-scale simulations and statistics.
-   Automatic modifier parameter and price tuning.
-   Optional online multiplayer, provided it does not jeopardize the
    research schedule.

These later features are planned, not part of the initial rules-engine
milestone.

## Project principles

1.  **One source of truth:** the client, AI, and simulator must use the
    same rules engine.
2.  **Rules before presentation:** implement and test game logic before
    investing in UI polish.
3.  **Explicit state:** an empty scorecard cell must be distinguishable
    from a cell containing zero.
4.  **Reproducibility:** simulations should record configuration and
    random seeds.
5.  **Small, testable boundaries:** keep core game rules independent
    from Unity presentation and networking code.
6.  **Avoid premature complexity:** add only the architecture needed for
    current requirements, while avoiding obvious hardcoded limitations.

## Documentation

-   [`Documentation/GameRules.md`](Documentation/GameRules.md) ---
    scorecard, scoring, turn and column rules.
-   [`Documentation/Architecture.md`](Documentation/Architecture.md) ---
    code boundaries and state model.
-   [`Documentation/ExperimentDesign.md`](Documentation/ExperimentDesign.md)
    --- intended simulation and balancing methodology.
-   [`Documentation/Decisions.md`](Documentation/Decisions.md) ---
    confirmed decisions, assumptions and unresolved questions.
-   [`Documentation/DevelopmentPlan.md`](Documentation/DevelopmentPlan.md)
    --- implementation order and milestone checks.

## Initial development order

1.  Finalize and document rules.
2.  Create the minimum core state model.
3.  Implement scoring functions.
4.  Write Edit Mode tests for scoring.
5.  Implement a complete turn and test legal/illegal actions.
6.  Run a complete match without rendering.
7.  Add simple AI agents and simulation metrics.
8.  Add the shop, modifiers, more AI strategies and research
    experiments.

## Unity setup

The project uses Unity and C#. Keep the core logic free of
`MonoBehaviour`, `GameObject`, and other presentation-specific
dependencies wherever practical. Use assembly definitions to enforce
boundaries.

Initial assembly dependency direction:

``` text
Simulation -> Core
Research   -> Core, Simulation
Client     -> Core
Networking -> Core
Core       -> no project-specific assemblies
```

Do not add networking, modifier systems, or complex AI to the core
milestone before the basic game rules and headless match runner work.

## Research data

Store experiment configurations, raw simulation results, processed
summaries and figures separately from game assets. Do not overwrite
baseline results when running tuned configurations. Every experiment
should have a unique identifier and enough metadata to reproduce it.

See `Documentation/ExperimentDesign.md` before implementing the research
runner.
