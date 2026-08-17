# GDM Coop Mod

A cooperative multiplayer mod for **Star Ocean: The Second Story R**, built using **BepInEx** and **Unity IL2CPP modding tools**.

The goal of this project is to add cooperative gameplay features to Star Ocean 2 R, allowing multiple players to participate together while preserving the original game's mechanics and systems.

## Download

The latest releases of GDM Coop Mod are available here:

**[GDM Coop Mod Releases](https://github.com/GDSeirya/GDMCoopMod/releases)**

## Features

Current features:

- Multiplayer/cooperative control for **2–4 players**
- Localisation based on the language selected in-game
- D-Pad support for switching battle skill slots
- Ability to control multiple characters simultaneously
- Switching between nearby targets and the host's target
- Cooperative camera that tracks multiple playable characters
- Camera support for cutscenes and host-target tracking
- Spellcaster support:
  - Switch between Set Spells using the right stick
  - Cast the selected spell using the north button
- Controller hot-plugging and automatic controller detection
- Support for the game's supported languages

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

1. Install **BepInEx IL2CPP** into your Star Ocean: The Second Story R installation.
2. Download the latest **GDMCoopMod.dll** from the [Releases](https://github.com/GDSeirya/GDMCoopMod/releases) page.
3. Copy the plugin DLL into:

```text
STAR OCEAN THE SECOND STORY R/
└── BepInEx/
    └── plugins/
        └── GDMCoopMod.dll
```

4. Launch the game.

BepInEx should automatically load the plugin.

> **Note:** If you are building the mod from source, see [Building From Source](#building-from-source).

## Controller & Character Assignment

The mod provides keyboard shortcuts for selecting controllers and assigning them to party characters.

### Controller Selection

Use **F1–F4** to select which controller you want to configure.

| Key | Function |
|---|---|
| **F1** | Select Controller 1 |
| **F2** | Select Controller 2 |
| **F3** | Select Controller 3 |
| **F4** | Select Controller 4 |
| **F5** | Display controller and assignment information |
| **F6** | Unassign the selected controller |
| **F7** | Unassign all controllers |

Once a controller has been selected, use **F1–F4** again to assign that controller to the corresponding character.

### AI Control

Use the following key to toggle the game's AI control:

| Key | Function |
|---|---|
| **F9** | Toggle AI control |

### Example

To assign **Controller 2** to **Character 3**:

1. Press **F2** to select Controller 2.
2. Press **F3** to assign Controller 2 to Character 3.

To remove Controller 2 from its assigned character:

1. Press **F2** to select Controller 2.
2. Press **F6** to unassign it.

To remove all controller assignments:

- Press **F7**.

## Building From Source

Clone the repository:

```bash
git clone https://github.com/GDSeirya/GDMCoopMod.git
```

Open the solution in Visual Studio 2022.

Build the project using:

```text
Build → Build Solution
```

The compiled DLL will be located in:

```text
bin/
└── Debug/
    └── net8.0/
        └── GDMCoopMod.dll
```

Copy the resulting DLL into the BepInEx `plugins` folder to test the mod.

## Dependencies

### Runtime Dependencies

The following are required to run the mod:

- **Star Ocean: The Second Story R (PC version)**
  - A legitimate copy of the game is required.
- **BepInEx IL2CPP**
  - Required mod loader.
  - Provides plugin loading, IL2CPP interop, and Harmony patching.

## Development Dependencies

The following are required to build the mod from source.

### Required Software

- **Visual Studio 2022**
  - Workload: `.NET desktop development`
- **.NET 8 SDK**
- **Git**

### BepInEx Development Assemblies

The project references assemblies provided by BepInEx:

```text
BepInEx/core/
├── BepInEx.Core.dll
├── BepInEx.Unity.IL2CPP.dll
├── 0Harmony.dll
└── Il2CppInterop.Runtime.dll
```

### Game IL2CPP Interop Assemblies

After installing BepInEx and launching the game once, the required IL2CPP interop assemblies will be generated:

```text
BepInEx/interop/
├── Il2Cppmscorlib.dll
├── UnityEngine.CoreModule.dll
├── UnityEngine.IMGUIModule.dll
├── UnityEngine.UI.dll
├── UnityEngine.PhysicsModule.dll
├── UnityEngine.TextRenderingModule.dll
├── UnityEngine.InputLegacyModule.dll
├── Unity.InputSystem.dll
└── Assembly-CSharp.dll
```

These files are generated from the game installation and are **not included in this repository**.

Developers must generate their own copies.

## Setting Up a Development Environment

1. Install BepInEx IL2CPP into the game directory.
2. Launch the game once to generate the IL2CPP interop assemblies.
3. Clone this repository.
4. Update the project references to point to your local BepInEx and interop assemblies.
5. Open the solution in Visual Studio 2022.
6. Build the solution.

## Modding Notes

This project targets the IL2CPP version of Unity used by **Star Ocean: The Second Story R**.

Because the game uses IL2CPP:

- Game assemblies are generated through BepInEx's IL2CPP interop tools.
- Decompiled assemblies are used for reference during development.
- Runtime patches and modifications are implemented through BepInEx and Harmony.

Generated game files and IL2CPP interop assemblies are intentionally not included in this repository.

## Contributing

Contributions, suggestions, bug reports, and testing reports are welcome.

Before submitting changes:

- Keep code organized and documented.
- Avoid committing generated files.
- Test changes in-game before submitting a pull request.
- Include relevant information when reporting bugs, such as logs, reproduction steps, and the game/mod version.

## License

This project is licensed under the **MIT License**.

See [LICENSE](LICENSE) for details.

## Disclaimer

This project is a fan-made modification and is not affiliated with, endorsed by, or sponsored by **Square Enix** or **Gemdrops**.

You must own a legitimate copy of **Star Ocean: The Second Story R** to use this mod.

---

# GDM Coop Mod

**スターオーシャン セカンドストーリーR** に協力プレイ機能を追加するための Mod です。  
この Mod は **BepInEx** と **Unity IL2CPP モッディングツール** を使用して構築されています。

本プロジェクトの目的は、スターオーシャン 2 R に協力プレイ機能を追加し、複数のプレイヤーが同時に参加できるようにしつつ、元のゲームのシステムやメカニクスを維持することです。

## ダウンロード

GDM Coop Mod の最新リリースはこちらからダウンロードできます。

**[GDM Coop Mod リリースページ](https://github.com/GDSeirya/GDMCoopMod/releases)**

## 機能

現在の機能:

- **2～4人**でのマルチプレイヤー／協力プレイ
- ゲーム内で設定されている言語に応じたローカライズ
- Dパッドによるバトルスキルスロットの切り替え
- 複数のキャラクターを同時に操作
- 近くのターゲットとホストが選択しているターゲットの切り替え
- 複数の操作キャラクターを追従する協力プレイ用カメラ
- カットシーンおよびホストのターゲットに対応したカメラ制御
- 術師への対応:
  - 右スティックでセット呪紋を切り替え
  - 北ボタンで選択中の呪紋を使用
- コントローラーの接続・切断の検出とホットプラグ対応
- ゲームでサポートされている言語への対応

## 必要環境

### プレイヤー向け

- 正規版 **スターオーシャン セカンドストーリーR**
- [BepInEx](https://github.com/BepInEx/BepInEx) IL2CPP の導入
- Windows PC 版のゲーム

### 開発者向け

- Visual Studio 2022
- .NET 8 SDK
- BepInEx IL2CPP 開発環境
- ゲームから生成された IL2CPP インターフェースアセンブリ

## インストール方法

1. スターオーシャン セカンドストーリーR のゲームフォルダに **BepInEx IL2CPP** を導入します。
2. [リリースページ](https://github.com/GDSeirya/GDMCoopMod/releases) から最新の **GDMCoopMod.dll** をダウンロードします。
3. プラグイン DLL を以下の場所にコピーします:

```text
STAR OCEAN THE SECOND STORY R/
└── BepInEx/
    └── plugins/
        └── GDMCoopMod.dll
```

4. ゲームを起動します。

BepInEx が自動的にプラグインを読み込みます。

> **注意:** ソースコードからビルドする場合は、[ソースコードからビルドする](#ソースコードからビルドする) を参照してください。

## コントローラーとキャラクターの割り当て

この Mod では、コントローラーを選択し、パーティキャラクターに割り当てるためのキーボードショートカットを使用できます。

### コントローラーの選択

**F1～F4** を使用して設定するコントローラーを選択します。

| キー | 機能 |
|---|---|
| **F1** | コントローラー1を選択 |
| **F2** | コントローラー2を選択 |
| **F3** | コントローラー3を選択 |
| **F4** | コントローラー4を選択 |
| **F5** | コントローラーと割り当て情報を表示 |
| **F6** | 選択中のコントローラーの割り当てを解除 |
| **F7** | すべてのコントローラーの割り当てを解除 |

コントローラーを選択した状態で **F1～F4** を押すと、そのコントローラーを対応するキャラクターに割り当てます。

### AI 制御

以下のキーでゲーム内の AI 制御を切り替えられます:

| キー | 機能 |
|---|---|
| **F9** | AI 制御の切り替え |

### 使用例

**コントローラー2** を **キャラクター3** に割り当てる場合:

1. **F2** を押してコントローラー2を選択します。
2. **F3** を押してコントローラー2をキャラクター3に割り当てます。

コントローラー2の割り当てを解除する場合:

1. **F2** を押してコントローラー2を選択します。
2. **F6** を押して割り当てを解除します。

すべてのコントローラーの割り当てを解除する場合:

- **F7** を押します。

## ソースコードからビルドする

リポジトリをクローンします:

```bash
git clone https://github.com/GDSeirya/GDMCoopMod.git
```

Visual Studio 2022 でソリューションを開きます。

以下からプロジェクトをビルドします:

```text
Build → Build Solution
```

ビルドされた DLL は以下に生成されます:

```text
bin/
└── Debug/
    └── net8.0/
        └── GDMCoopMod.dll
```

生成された DLL を BepInEx の `plugins` フォルダにコピーして、ゲーム内でテストしてください。

## 依存関係

### 実行時依存関係

Mod の実行には以下が必要です:

- **スターオーシャン セカンドストーリーR（PC版）**
  - 正規版のゲームが必要です。
- **BepInEx IL2CPP**
  - 必須の Mod ローダーです。
  - プラグインの読み込み、IL2CPP インターフェース、Harmony パッチングを提供します。

## 開発依存関係

ソースコードから Mod をビルドするには、以下が必要です。

### 必須ソフトウェア

- **Visual Studio 2022**
  - ワークロード: `.NET デスクトップ開発`
- **.NET 8 SDK**
- **Git**

### BepInEx 開発アセンブリ

プロジェクトは以下の BepInEx アセンブリを参照します:

```text
BepInEx/core/
├── BepInEx.Core.dll
├── BepInEx.Unity.IL2CPP.dll
├── 0Harmony.dll
└── Il2CppInterop.Runtime.dll
```

### ゲーム IL2CPP インターフェースアセンブリ

BepInEx を導入し、ゲームを一度起動すると、必要な IL2CPP インターフェースアセンブリが生成されます:

```text
BepInEx/interop/
├── Il2Cppmscorlib.dll
├── UnityEngine.CoreModule.dll
├── UnityEngine.IMGUIModule.dll
├── UnityEngine.UI.dll
├── UnityEngine.PhysicsModule.dll
├── UnityEngine.TextRenderingModule.dll
├── UnityEngine.InputLegacyModule.dll
├── Unity.InputSystem.dll
└── Assembly-CSharp.dll
```

これらはゲームから生成されるため、**本リポジトリには含まれていません**。

開発者は自身の環境で生成する必要があります。

## 開発環境のセットアップ

1. ゲームディレクトリに BepInEx IL2CPP を導入します。
2. ゲームを一度起動して IL2CPP インターフェースアセンブリを生成します。
3. このリポジトリをクローンします。
4. プロジェクトの参照先を、ローカルの BepInEx および interop アセンブリに設定します。
5. Visual Studio 2022 でソリューションを開きます。
6. ソリューションをビルドします。

## モッディングノート

本プロジェクトは、**スターオーシャン セカンドストーリーR** が使用する Unity IL2CPP を対象としています。

ゲームが IL2CPP を使用しているため:

- ゲームアセンブリは BepInEx の IL2CPP インターフェースツールによって生成されます。
- 開発時にはデコンパイルされたアセンブリを参照します。
- ランタイムのパッチや変更は BepInEx と Harmony を通じて行われます。

生成されたゲームファイルや IL2CPP インターフェースアセンブリは、意図的にリポジトリへ含めていません。

## コントリビューション

改善提案、バグ報告、テスト報告、プルリクエストを歓迎します。

変更を提出する前に:

- コードを整理し、必要に応じてコメントを追加してください。
- 生成ファイルをコミットしないでください。
- プルリクエストを提出する前に、変更をゲーム内でテストしてください。
- バグ報告には、可能であればログ、再現手順、ゲームおよび Mod のバージョンを含めてください。

## ライセンス

本プロジェクトは **MIT License** の下で提供されています。

詳細については [LICENSE](LICENSE) を参照してください。

## 免責事項

本プロジェクトはファンによる非公式 Mod であり、**スクウェア・エニックス**または**ジェムドロップ**による承認、支援、スポンサーシップを受けたものではありません。

Mod を使用するには、正規版の **スターオーシャン セカンドストーリーR** が必要です。
