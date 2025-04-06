# Online Pong Demo Game (WIP)

This repository is a public showcase of my programming skills and a demonstration of a scalable multiplayer game architecture.

**Please note:** The demo is still a work-in-progress and is not fully functional.

## Overview

This project is a multiplayer game built on a scalable architecture that reimagines the classic pong game. In this game:
- One user creates a session.
- Another user connects to the session using a unique session code.

## Tech Stack

The project leverages the following technologies:
- **URP (Universal Render Pipeline)**
- **Photon Fusion 2**
- **VContainer**
- **R3 for Unity** (previously UniRx)
- **UniTask**
- **Stateless**

## Getting Started

### Starting Point

For an overview of the game setup and architecture, please refer to the following files:
- `Sources/Presentation/Game/GameBoot.cs`
- `Sources/Presentation/Game/GameScope.cs`

### Prerequisites

- **Unity:** Ensure you have the appropriate version of Unity installed.
- **Odin Inspector:** To compile and run the project locally, you must import your own copy of the Odin Inspector asset package (this is a paid asset).

## Gameplay

The core gameplay is a modern take on the classic pong game:
- **Session Creation:** One player initiates a game session.
- **Session Join:** A second player can join the session by entering the session code provided.

## Future Work

Since the demo is a work-in-progress, expect further developments including:
- Finalizing the core game mechanics.
- Enhanced gameplay features.
- More robust network handling.
- Additional documentation and improvements in user experience.

Contributions and feedback are welcome as I continue to refine and expand this project.