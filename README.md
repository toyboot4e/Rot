# Rot
A 2D, GUI to-be turn-based roguelike game made with [Nez](https://github.com/prime31/Nez). The goal is to be an RPG-like roguelike game with some static contents.

## Architecture

### Engine - View separation
The internal game state is called as 'Engine'. When you `tick` the game state, it returns `TickReport`, with which you can pull enough context to visualize what happened.

### The moddable engine
The core of the engine is basically a processor of contents. We develop the game by adding contents to the engine.

### Overridable logic
The game logic, such as `MeleeAttack`, is dealt with functions. We call them systems. Systems are applied in the order of `precedence` float value, so we can *override* the game logic by adding another system. This makes the engine more 'moddable'.

### Entity & Component
The game is built with Nez, which provides class-based EC(S).
