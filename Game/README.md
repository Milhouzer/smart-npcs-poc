# Game Documentation
### Table of Contents

1. [Introduction](#introduction)
2. [How to Play](#how-to-play)
   - 2.1 [Launch the Server](#launch-server)
   - 2.2 [Launch Clients](#launch-clients)
   - 2.3 [Gameplay Overview](#gameplay-overview)
4. [Controls](#controls)
   - 3.1 [Player Controls](#player-controls)
   - 3.2 [Build Mode Control](#build-mode-controls)
   - 3.3 [UI Interactions](#ui-interactions)
   - 3.4 [Input Key Mapping](#input-key-mapping)
     - 3.4.1 [Generic Controls](#generic-controls)
     - 3.4.2 [Alternatives](#alternatives)
5. [Advanced Gameplay Mechanics](#advanced-gameplay-mechanics)
6. [Debugging and Troubleshooting](#debugging)

## Introduction

Releases are available at [Smart NPCs PoC Releases](https://github.com/Milhouzer/smart-npcs-poc/releases)
It is recommended to download the latest release.

[TBD]

## How to Play

### Launch the Server

The game needs a server in order to run:
- Execute the .exe release of your choice
- Click on the **"start server"** button
  
### Launch Clients 
⚠️ The versions used for the client must match the server one.

Now that the server is running, you can log as many clients as you want:
- Execute the same .exe release used to run the server
- Click on the **"start client"** button
- You can play as a client, you can repeat the operation as many time as you want and see how clients interact together!
  
### Gameplay Overview

Build objects available in the catalog, use tools on them and try to upgrade them!
Sadly there is real UI or any sort of guidage ingame, you'll have to figure it out by yourself and using the controls tables below.
Debug console is still available so you know what is happening.

## Controls

In the game, you can control the player and interact with the build mode through keyboard inputs. Here’s a breakdown of the controls for the **Player**, **Build Mode**, and **Generic Actions**.

### Player Controls

The player has two main actions: moving the camera and entering build mode.

| Action               | Description                       | Default Key Bindings     |
|----------------------|-----------------------------------|--------------------------|
| **Move**             | Move the player camera           | **Arrow Keys** (⬆ ⬇ ⬅ ➡) |
| **Enter Build Mode**  | Switch to build mode              | **B**                    |

When in player mode, you can move using the arrow keys and enter build mode by pressing **B**.

### Build Mode Controls

Once you enter the build mode, the selection catalog opens and several actions are available to manage building and interaction.

| Action               | Description                           | Default Key Bindings     |
|----------------------|---------------------------------------|--------------------------|
| **Exit Build Mode**   | Exit from build mode and return to player control | **B**                    |
| **Rotate Object**     | Rotate the object (while holding **Shift**) | **Mouse Scroll**          |
| **Scale Object**      | Scale the object (while holding **Alt**) | **Mouse Scroll**          |
| **Select Object**     | Select an object to build in the catalog   | **Left Click**            |
| **Cancel Selection**  | Cancel the current action             | **Right Click**           |
| **Reset Build Input** | Reset the current object’s position or state | **Right Click**           |

You can exit build mode at any time by pressing **B** again.

### UI Interactions

UI Inputs will be consumed first. 

### Input Key Mapping

### Generic Controls

These are general-purpose inputs for actions like selecting or interacting with the game’s user interface.

| Action               | Description                           | Default Key Bindings     |
|----------------------|---------------------------------------|--------------------------|
| **Select**           | Select an item or object              | **Left Click**           |
| **Scroll**           | Scroll through options or zoom        | **Mouse Scroll**         |
| **Cancel**           | Cancel the current action             | **Right Click**          |

### Alternatives

The game also supports alternative input methods to facilitate various play styles.

| Action     | Description                           | Default Key Bindings     |
|------------|---------------------------------------|--------------------------|
| **Shift**  | Enable rotation                       | **Shift**                |
| **Alt**    | Enable scaling                        | **Alt**                  |
| **Control**| Reserved for future control actions   | **Ctrl**                 |


## Advanced Gameplay Mechanics

[TBD]

## Debugging and Troubleshooting

Debug information are available through the console. Each client as it's own personnal console, so does the server.
