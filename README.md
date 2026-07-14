# RyM Coop Mod

A cooperative multiplayer mod for **Star Ocean: The Second Story R**, built using **BepInEx** and **Unity IL2CPP modding tools**.

The goal of this project is to add cooperative gameplay features to Star Ocean 2 R, allowing multiple players to participate together while preserving the original game's mechanics and systems.

## Features

🚧 **Work in Progress**

Current features:
- BepInEx plugin framework setup
- In-game debug overlay
- Runtime testing tools

Planned features:
- Multiplayer/cooperative player control
- Additional player character control
- Synchronization of player actions
- Cooperative battle support
- Shared game state handling

## Requirements

### For Players

- A legitimate copy of **Star Ocean: The Second Story R**
- [BepInEx](https://github.com/BepInEx/BepInEx) IL2CPP installed
- Windows PC version of the game

### For Developers

- Visual Studio 2022
- .NET 8 SDK
- BepInEx IL2CPP development environment
- IL2CPP interop assemblies generated from the game

## Installation

### Development Build

1. Install BepInEx IL2CPP into your Star Ocean 2 R installation.
2. Download or build the latest RyM Coop Mod DLL.
3. Copy the plugin DLL into:

```
Star Ocean The Second Story R/
└── BepInEx/
    └── plugins/
        └── RyMCoopMod.dll
```

4. Launch the game.

BepInEx should load the plugin automatically.

## Building From Source

Clone the repository:

```bash
git clone https://github.com/<username>/RyMCoopMod.git
```

Open the solution in Visual Studio.

Build the project:

```
Build → Build Solution
```

The compiled DLL will be located in:

```
bin/
└── Debug/
    └── net8.0/
        └── RyMCoopMod.dll
```

Copy the DLL into the BepInEx plugins folder to test.

## Dependencies

### Runtime Dependencies

The following are required to run the mod:

- **Star Ocean: The Second Story R (PC version)**
  - A legitimate copy of the game is required.
- **BepInEx IL2CPP**
  - Required mod loader.
  - Provides plugin loading, IL2CPP interop, and Harmony patching.

## Development Dependencies

The following are required to build the mod from source:

### Required Software

- **Visual Studio 2022**
  - Workload: `.NET desktop development`
- **.NET 8 SDK**
- **Git**

### BepInEx Development Assemblies

The project references assemblies generated/provided by BepInEx:

```
BepInEx/core/
├── BepInEx.Core.dll
├── BepInEx.Unity.IL2CPP.dll
├── 0Harmony.dll
└── Il2CppInterop.Runtime.dll
```

### Game IL2CPP Interop Assemblies

After installing BepInEx and launching the game once, IL2CPP interop assemblies will be generated:

```
BepInEx/interop/
├── Il2Cppmscorlib.dll
├── UnityEngine.CoreModule.dll
├── UnityEngine.IMGUIModule.dll
└── Assembly-CSharp.dll
```

These files are generated from the game installation and are **not included in this repository**.

Developers must generate their own copies.

## Setting Up a Development Environment

1. Install BepInEx IL2CPP into the game directory.
2. Launch the game once to generate interop assemblies.
3. Clone this repository.
4. Update project references to point to your local BepInEx and interop assemblies.
5. Build the solution.

## Modding Notes

This project targets the IL2CPP version of Unity used by Star Ocean: The Second Story R.

Because the game uses IL2CPP:
- Game assemblies are generated through BepInEx interop tools.
- Decompiled assemblies are used for reference during development.
- Runtime patches and modifications are implemented through BepInEx and Harmony.

Generated game files and interop assemblies are intentionally not included in this repository.

## Contributing

Contributions, suggestions, and testing reports are welcome.

Before submitting changes:
- Keep code organized and documented.
- Avoid committing generated files.
- Test changes in-game before submitting pull requests.

## License

This project is licensed under the MIT License.

See [LICENSE](LICENSE) for details.

## Disclaimer

This project is a fan-made modification and is not affiliated with or endorsed by Square Enix or Gemdrops.

You must own a legitimate copy of Star Ocean: The Second Story R to use this mod.
