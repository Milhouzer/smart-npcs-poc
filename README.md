# Smart NPCs in a Multiplayer Unity Game

This project integrates intelligent, dynamic NPC (Non-Player Character) behaviors into a multiplayer Unity game by leveraging a generative AI model via an API. The AI-driven NPCs react to various contextual inputs from the game environment, delivering more immersive and engaging interactions.

## Table of Contents

1. [Project Overview](#project-overview)
   -  1.1 [General idea](#general-idea)
   -  1.2 [Gameplay Integration](#gameplay-integration)
3. [Architecture](#architecture)
4. [Setup](#setup)
5. [API Integration](#api-integration)
6. [Unity Integration](#unity-integration)
7. [How to Use](#how-to-use)
   -  7.1 [Game](#game)
   -  7.2 [API](#api)
8. [Future Improvements](#future-improvements)
9. [Contributing](#contributing)
10. [License](#license)

---

## Project Overview

### General Idea

The **Smart NPCs** project main objective is to allow for the creation of NPCs that dynamically respond to their surroundings and interactions within a multiplayer Unity game. The reactions of these NPCs are powered by an external generative AI model, which processes the context provided by the game server and generates appropriate behaviors, dialogues, and actions for the NPCs.

The AI model interprets the game context, such as the player's actions, world state, and nearby events, allowing NPCs to respond in ways that make the game feel more alive and interactive.

### Gameplay Integration

See [Game Documentation](./Game/README.md) for more details about the gameplay.

---

## Architecture

The architecture of the Smart NPCs system consists of several key components:

- **Unity Multiplayer Game**: The game server sends relevant environmental data, player actions, and other context to the AI API.
- **Generative AI Model**: Hosted externally, this model processes the contextual information and returns appropriate NPC behaviors, including dialogue or actions.
- **API Gateway**: Facilitates the communication between the Unity game and the generative AI, handling requests and responses efficiently.
- **Context System**: Collects and structures the necessary data from the game world, such as NPC surroundings, events, player proximity, and current game state.
- **NPC Controller**: A system within Unity that updates each NPC's behavior based on the response from the AI model.

---

## Setup

### 1. Prerequisites

#### Game
- **Unity 2022.3+** with the **Multiplayer Netcode** package.

#### API [TBD]

- **GoLang** (for handling server-side API communication). **[TDB]**
- Access to an external **Generative AI API** (e.g., OpenAI, GPT-3/4, or a custom model).
- **Docker** (optional) for containerized API hosting. **[TDB]**

### 2. Cloning the Repository

```bash
git clone https://github.com/your-username/smart-npcs.git
cd smart-npcs
```

### 3. API Key Setup

To connect with the external generative AI model, you need to configure the API key:

- Create a `.env` file in the project root with the following content:

```
AI_API_KEY=<your-api-key>
AI_API_URL=<your-api-endpoint>
```

### 4. Unity Setup

- Open the Unity project located in the `UnityProject` folder.
- Ensure all necessary packages (Multiplayer Netcode, Unity Input System) are installed.

---

## API Integration

The server communicates with the generative AI model through a RESTful API. The API is responsible for sending the game context and receiving the corresponding NPC response.

### API Request Example

A typical request to the AI model includes:

```json
{
  "npc_id": "npc_001",
  "location": "village_square",
  "player_action": "greet",
  "nearby_objects": [
    {"type": "tree", "state": "healthy", "distance":"2,75"},
    {"type": "fire", "state": "burning", "distance":"1.23"}
  ],
  "time_of_day": "morning",
  "weather": "sunny"
}
```

### API Response Example

The AI responds with the NPC's behavior and dialogue:

```json
{
  "npc_id": "npc_001",
  "dialogue": "Good morning, traveler! Quite a sunny day, isn't it?",
  "action": "wave"
}
```

---

## Unity Integration

### 1. Context System

In the Unity game, the context system gathers all relevant data about the environment, NPCs, and players. This data is then structured and sent via the API to the external AI.

### 2. NPC Controller

The NPC controller receives AI-generated responses and applies them to the NPCs in the game. This includes:

- Updating NPC dialogue boxes with AI-generated text.
- Executing actions (e.g., animations like waving, pointing, or walking away).
- Updating NPC behavior based on player interaction and world state.

---

## How to Use

### Game

See [Game Documentation](./Game/README.md).

### API

See [API Documentation](./API/README.md).

---

## Future Improvements

- **Advanced Contextual Understanding**: Enhance the AI’s understanding of deeper gameplay mechanics, such as NPC backstories or complex player decisions.
- **Real-Time Adaptation**: Improve how NPCs adapt to rapid changes in the environment or player behavior.
- **Performance Optimization**: Introduce caching mechanisms for common NPC responses to reduce API calls and improve performance.

---

## Contributing

Contributions are welcome! Feel free to submit a pull request or create an issue if you find a bug or have a suggestion for improvements.

1. Fork the repository.
2. Create a feature branch (`git checkout -b feature-branch`).
3. Commit your changes (`git commit -m 'Add some feature'`).
4. Push to the branch (`git push origin feature-branch`).
5. Open a pull request.

---

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more details.

---
