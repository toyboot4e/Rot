# Architecture

## Engine - View separation

We split the game into *Engine* and *UI.* Engine is the internal game state, UI is the other (input and view). When we `tick` the game state, it returns `RlTickReport`, with which UI can pull enough context to visualize what happened.

> The terminology is based on [this blog post](https://journal.stuffwithstuff.com/2014/07/15/a-turn-based-game-loop/), but they don't directly apply to our project names. `Rot.Engine` is almost same as the internal game state. But `Rot.Ui` is a view/input component for the `Rot.Game` application, and it is the UI to the Engine.

## Engine = processor of contents

We develop the game by adding contents to the engine.

### Mutation via events

We mutate the game by pushing `RlEvent` to the engine. `RlEvent` is not only primitive events, such as `GiveDamage`, but also **actions** of entities. This is natural because action events are a kind of mutation; they result in some primitive events, e.g. `MeleeAttack` → `Hit` → `GiveDamage`.

### Layers of logics

When we add a new feature to the game, we may want to add some logic to existing action event handlings. For example, if an entity is able to become "dissy", they may walk in a random direction when they're in that state. We need to add that logic to the `Walk` event handling then. How should we achive it?

Since we add more *extension* to the engine later, we don't want to write an action handling logic at *one place*. Instead, we collect each logic via `RlEventHub`, which dispatches *event handlers* depending on their `precedence`.