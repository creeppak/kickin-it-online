# Skyline - Realtime Multiplayer Pong Game (WIP)

This repository is a demonstration of a scalable multiplayer game architecture using Unity & Photon Fusion 2.

![Metagame Screenshot](Markdown/main-menu.png)
![New arena WIP](Markdown/homeland.gif)

Check out this short demo video to see it in action: [Demo Video](https://youtu.be/APnbyL-8Pvo)

## Overview

This project is a multiplayer game built on a scalable architecture that reimagines the classic pong game (but is
actually a Crash Bash clone).
In this game:
- One user creates a session.
- Another user connects to the session using a unique session code.
- Player compete in a pong game to determine the winner.

## Tech Stack

The project leverages the following technologies:
- **URP (Universal Render Pipeline)**
- **Photon Fusion 2**
- **VContainer**
- **R3 for Unity** (previously UniRx)
- **UniTask**
- **Stateless**
- **Odin Inspector**
- **FEEL**

## Getting Started

### Starting Point

For an overview of the game setup and architecture, please refer to the following source files:

| Level       | Source File                                  |
|-------------|----------------------------------------------|
| Application | `Presentation/App/AppScope.cs`               |
| Meta-Game   | `Presentation/Metagame/MetaGameScope.cs`     |
| Game        | `Presentation/Game/GamePresentationScope.cs` |
| Simulation  | `Simulation/Game/GameSimulationScope.cs`     |

_You can find the source files in the `Assets/Sources` folder._

### Prerequisites

- **Unity:** Ensure you have the appropriate version of Unity installed (2022.3.60f1).
- **Odin Inspector:** To compile and run the project locally, you must import your own copy of the Odin Inspector asset package (this is a paid asset).
- **FEEL**: To compile and run the project locally, you must import your own copy of the FEEL asset package (this is a paid asset).

## Gameplay

The core gameplay is a modern take on the classic pong game:
- **Session Creation:** One player initiates a game session.
- **Session Join:** A second player can join the session by entering the session code provided.
- **Player Movement:** Each player controls its own cart to deflect the ball back to the opponent.
- **Player Health:** When player misses the ball, he loses a health point.
- **Win/Lose Condition:** When player score reaches 0, he loses the game.

## Controls

Players can control their paddle using the following keys:
- **A Key:** Move paddle to the left.
- **D Key:** Move paddle to the right.
- **Space Key:** To deflect the ball.

## Future Work

Since the demo is a work-in-progress, expect further developments including:
- Finalizing the core game mechanics.
- Enhanced gameplay features.
- More robust network handling.
- Additional documentation and improvements in user experience.

Contributions and feedback are welcome as I continue to refine and expand this project.