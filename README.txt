# Pandemic-Inspired Strategy Board Game (Unity)

## Overview

This project is a digital adaptation inspired by the board game Pandemic, developed using the Unity game engine. The game focuses on cooperative strategy gameplay where players work together to prevent the spread of diseases across a global map. Core features include turn-based player actions, infection spread mechanics, and resource management systems. The project is designed to demonstrate our understanding of object-oriented programming, game logic, and system integration using C# and Unity. Our goal is to create a functional prototype that highlights core gameplay mechanics while maintaining a clean and scalable code structure.

## Installation

To run this project, you will need the following dependencies installed:

* Unity Hub (latest version recommended)
* Unity Editor (version 2022.3 LTS or newer recommended)
* Visual Studio 2022 (or another C# IDE)
* .NET SDK (usually included with Unity/Visual Studio)

### Setup Steps

1. Clone the repository:

   git clone https://github.com/it-sd-capstone/capstone-project-the-developing-dogs

2. Open Unity Hub.

3. Click "Add" and then click "Add project from disk". Select the cloned project folder.

4. Allow Unity to load and import all assets (this may take a few minutes).

5. Open the main scene located in:

   Assets/Scenes/SampleScene.unity

## Testing

This project can be validated using both manual testing and Unity’s Play Mode.

### Manual Testing

1. Open the project in Unity.
2. Press the "Play" button in the Unity Editor.
3. Verify the following:

   * Game loads without errors

### Debugging

* Use `Debug.Log()` messages in the Unity Console to track behavior.
* Ensure no errors appear in the Console window during runtime.

### (Optional Automated Testing)

If unit tests are included:

1. Open Unity Test Runner:

   Window → General → Test Runner

2. Run all tests and verify they pass.


## Usage

To run the project:

1. Open the project in Unity.
2. Load the main scene:

   Assets/Scenes/SampleScene.unity

3. Press the "Play" button in the Unity Editor.

### Gameplay Overview

* Players take turns performing actions such as moving between cities and treating infections.
* Diseases spread automatically based on game rules.
* The objective is to control outbreaks and prevent global loss conditions.

### Build Instructions (Optional)

To build the game:

1. Go to:

   File → Build Settings

2. Select "PC, Mac & Linux Standalone"
3. Click "Build" and choose a destination folder
