# Architecture and Core State Model

## Goals

The initial architecture should make the rules easy to test and let the
same engine serve the Unity client, AI agents and headless simulation
runner.

Avoid building systems that are not needed yet. In particular,
networking, modifiers, shops and sophisticated AI should not complicate
the first rules-engine milestone.

## Assembly boundaries

Create these assembly definitions under `Assets/_Project/Scripts/`:

  -----------------------------------------------------------------------
  Assembly                Responsibility          References
  ----------------------- ----------------------- -----------------------
  `Yamb.Core`             State, rules, scoring,  None
                          turn actions and core   
                          game transitions        

  `Yamb.Simulation`       Match runner, AI        `Yamb.Core`
                          agents, simulation      
                          metrics                 

  `Yamb.Research`         Experiment              `Yamb.Core`,
                          configurations,         `Yamb.Simulation`
                          balancing algorithms    
                          and result export       

  `Yamb.Client`           Unity UI, input and     `Yamb.Core`
                          presentation            

  `Yamb.Networking`       Network message         `Yamb.Core`
                          handling and            
                          multiplayer transport   
                          integration             
  -----------------------------------------------------------------------

The dependency direction should remain:

``` text
Simulation -> Core
Research   -> Core + Simulation
Client     -> Core
Networking -> Core
Core       -> no project-specific assemblies
```

The Core assembly should not depend on `UnityEngine` presentation types.
Prefer standard C# types and deterministic, injectable randomness.

## Minimum state model

### GameState

Represents the match as a whole.

Minimum fields:

-   Players
-   Active player index
-   Current turn state
-   Match status (in progress or complete)

Add a match ID or seed only when needed by the runner and
reproducibility work.

### PlayerState

Represents one player.

Minimum fields:

-   Stable player ID
-   Scorecard state
-   Money balance (for the later shop)
-   Owned modifier instances (for later modifier support)

Do not implement shop behavior in the initial core milestone merely
because the state model anticipates money and modifiers. If that data is
not needed for the first playable core, it can be added when the shop
work begins.

### ScorecardState

Represents 4 columns × 14 categories.

Requirements:

-   Store whether each cell is filled.
-   Store the score for filled cells, including zero.
-   Determine the next legal cell for the Downward and Upward columns.
-   Determine whether a cell is available in the Free and Call columns.
-   Calculate column totals and the final score, including bonuses.

Do not use zero to represent an empty cell. Use an explicit filled flag
or a nullable score representation.

### TurnState

Minimum fields:

-   Current dice values
-   Locked/unlocked status for each die
-   Number of throws completed
-   Called category, if any
-   Whether the turn is complete

The initial implementation uses five dice, but represent them as a
collection rather than five separately hardcoded variables. This keeps
the code manageable if additional dice are introduced later.

### ModifierInstance (later milestone)

When modifiers are implemented, an owned instance should identify:

-   Modifier definition ID
-   Owner
-   Remaining duration or uses, if applicable
-   Any instance-specific choice or parameter

Keep modifier definitions separate from modifier instances owned by
players.

## Core services

### YambScoring

Pure scoring functions that receive dice and return category scores.
They should not change player state or UI.

### YambRules

Validates whether an action is legal given the current state. Examples
include rolling, locking dice, calling a category and choosing a
scorecard cell.

### GameEngine / TurnController

Applies valid actions, updates the state and ends turns. It should not
contain UI logic.

Choose one clear entry point for applying game actions; avoid
duplicating rule validation in UI scripts and AI agents.

### Simulation runner

Creates a game, asks AI agents for legal actions and advances the engine
until the match finishes. It should run without rendering and export the
final outcome and relevant metrics.

## Randomness and reproducibility

Random dice generation should be injected into the core or supplied
through a small interface, rather than being hidden inside scoring or
state classes. This enables deterministic tests and seeded simulations.

A test should be able to supply known dice outcomes. A simulation run
should record its seed and configuration.

## Testing

Use Unity Edit Mode tests for pure core logic.

Recommended initial test groups:

-   `YambScoringTests`
-   `ScorecardStateTests`
-   `TurnRulesTests`
-   `GameEngineTests`
-   `MatchRunnerTests` (after the runner exists)

Test the core state and transitions without needing to load a scene or
instantiate UI objects.

## Initial folder structure

``` text
Assets/
  _Project/
    Scripts/
      Core/
      Simulation/
      Research/
      Client/
      Networking/
    Tests/
      EditMode/
      PlayMode/
    Prefabs/
    Scenes/
    ScriptableObjects/

Documentation/
  README.md
  GameRules.md
  Architecture.md
  ExperimentDesign.md
  Decisions.md
  DevelopmentPlan.md

Research/
  Experiments/
  Results/
  Figures/
  Notes/
```

Only create folders when they are needed. The diagram is a target
organization, not a requirement to create empty folders for every future
feature immediately.
