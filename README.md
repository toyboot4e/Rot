# Rot
A 2D, GUI to-be turn-based roguelike game made with [Nez](https://github.com/prime31/Nez). The goal is to make an RPG-like roguelike game with some static contents.

## Architecture

### Engine - View separation
We split the game into "Engine" and "UI". The internal game state is called as Engine. When we `tick` the game state, it returns `TickReport`, with which UI can pull enough context to visualize what happened.

### Engine = processor of contents
We develop the game by adding contents to the engine.

#### Mutation via events
We mutate the game by pushing `RlEvent` to the engine. `RlEvent` is not only primitive events, such as `TakeDamage`, but also **actions** of entities. Action events are a kind of mutation; they result in some primitive events.

#### Layers of logics
When we add a new feature to the game, we may want to add some logic to existing action event handlings. For example, if an entity become "dissy", their walking directions become random.

We don't write an action handling logic at one place. Instead, we collect each logic via `RlEventHub`. Logic subscribes specific `RlEvent`s through the hub, with some `precedene`. Then `RlEventHub` dispatches each logic to the events in the order of preecdence.

Logics may convert events to another; e.g. `MeleeAttack` → `Hit` → `TakeDamage`.
