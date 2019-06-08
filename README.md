# Rot
A 2D, GUI to-be turn-based roguelike game made with [Nez](https://github.com/prime31/Nez). The goal is to be an RPG-like roguelike game with some static contents.

## Architecture

### Engine - View separation
The internal game state is called as 'Engine'. When we `tick` the game state, it returns `TickReport`, with which we can pull enough context to visualize what happened.

### The moddable engine
We develop the game by adding contents to the engine. The core of the engine is basically a processor of contents.

### Overridable logic
The game logic, such as `MeleeAttack`, is dealt with functions. We call them systems. Systems are applied in the order of `precedence` value, so we can *override* the game logic by adding another system. This makes the engine more 'moddable'.

### Entity & Component
The game is built with Nez, so we use class-based EC(S).

## Not (yet) in concern
I'm not ready to take care about them. For examples,

### Serializable
Maybe using Nez.Persistance.

### Client-server separation
For porting to web.

### Multithreading
I'm not sure if we really need it though.

### (Really) moddable
Adding contents to the moddable engine via DLL or files.
