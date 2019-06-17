# Rot
> You cannot compile this repository; it doens't contain image files.

A 2D, GUI, in-development roguelike game made with [Nez](https://github.com/prime31/Nez). The goal is an RPG-like roguelike game with some static contents.

## Architecture

### Engine - View separation
We split the game into "Engine" and "UI". Engine is the internal game state, UI is other (input and view). When we `tick` the game state, it returns `RlTickReport`, with which UI can pull enough context to visualize what happened.

### Engine = processor of contents
We develop the game by adding contents to the engine.

#### Mutation via events
We mutate the game by pushing `RlEvent` to the engine. `RlEvent` is not only primitive events, such as `TakeDamage`, but also **actions** of entities. This is natural because action events are a kind of mutation; they result in some primitive events, e.g. `MeleeAttack` → `Hit` → `TakeDamage`.

#### Layers of logics
When we add a new feature to the game, we may want to add some logic to existing action event handlings. For example, if an entity is able to become "dissy", their walking directions become random. We need add the logic to `Walk` event handling.

Since we add more extension to the engine later, we don't write an action handling logic at one place. Instead, we collect each logic via `RlEventHub`. Logic subscribes specific `RlEvent`s through the hub, with some `precedence`. Then `RlEventHub` dispatches each logic to the events in the order of the precedences.
