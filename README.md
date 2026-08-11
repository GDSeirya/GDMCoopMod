# GDM Coop Mod

A cooperative multiplayer mod for **Star Ocean: The Second Story R**, built using **BepInEx** and **Unity IL2CPP modding tools**.

The goal of this project is to add cooperative gameplay features to Star Ocean 2 R, allowing multiple players to participate together while preserving the original game's mechanics and systems.

## Features

Current features:
- BepInEx plugin framework setup
- Runtime testing tools
- Multiplayer/cooperative player control

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
2. Download or build the latest GDM Coop Mod DLL.
3. Copy the plugin DLL into:

```
STAR OCEAN THE SECOND STORY R/
└── BepInEx/
    └── plugins/
        └── GDMCoopMod.dll
```

4. Launch the game.

BepInEx should load the plugin automatically.

## Controller & Character Assignment

The mod provides keyboard shortcuts for assigning controllers to party characters.

### Controller Assignment

To assign a controller to a character:

1. Press **F1–F4** to select the character you want to assign.
2. The selected controller will be assigned to that character.

| Key | Function |
|---|---|
| **F1** | Assign selected controller to Character 1 / Select Controller 1 |
| **F2** | Assign selected controller to Character 2 / Select Controller 2 |
| **F3** | Assign selected controller to Character 3 / Select Controller 3 |
| **F4** | Assign selected controller to Character 4 / Select Controller 4 |
| **F5** | Display controller/assignment information |
| **F6** | Unassign the selected controller |
| **F7** | Unassign all controllers |

> **Note:** Pressing **F2** selects Controller 2. Once a controller is selected, pressing **F1–F4** assigns it to the corresponding character.

### AI Control

The following keys control whether the game's AI is enabled for the characters:

| Key | Function |
|---|---|
| **F9** | Toggle AI |

### Example

To assign **Controller 2** to **Character 3**:

1. Press **F2** to select Controller 2.
2. Press **F3** to assign Controller 2 to Character 3.

To remove Controller 2 from its assigned character:

1. Press **F2** to select Controller 2.
2. Press **F6** to unassign it.

To reset all controller assignments:

- Press **F7**.

## Building From Source

Clone the repository:

```bash
git clone https://github.com/<username>/GDMCoopMod.git
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
        └── GDMCoopMod.dll
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

------------------------------------------------------------

# GDM Coop Mod

**スターオーシャン セカンドストーリーR** に協力プレイ機能を追加するための Mod です。  
この Mod は **BepInEx** と **Unity IL2CPP モッディングツール** を使用して構築されています。

本プロジェクトの目的は、スターオーシャン 2 R に協力プレイ機能を追加し、複数のプレイヤーが同時に参加できるようにしつつ、元のゲームのシステムやメカニクスを維持することです。

## 機能

現在の機能:
- BepInEx プラグインフレームワークのセットアップ
- ランタイムテストツール
- マルチプレイ／協力プレイ用キャラクター操作

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

### 開発ビルド

1. Star Ocean 2 R のインストール先に BepInEx IL2CPP を導入します。
2. 最新の GDM Coop Mod DLL をダウンロードまたはビルドします。
3. プラグイン DLL を以下の場所にコピーします:

```text
STAR OCEAN THE SECOND STORY R/
└── BepInEx/
    └── plugins/
        └── GDMCoopMod.dll
```

4. ゲームを起動します。

BepInEx が自動的にプラグインを読み込みます。

## コントローラーとキャラクター割り当て

この Mod は、パーティキャラクターにコントローラーを割り当てるためのキーボードショートカットを提供します。

### コントローラー割り当て

キャラクターにコントローラーを割り当てるには:

1. **F1–F4** を押して割り当てたいキャラクターを選択します。
2. 選択されたコントローラーがそのキャラクターに割り当てられます。

| キー | 機能 |
|---|---|
| **F1** | キャラクター1にコントローラーを割り当て / コントローラー1を選択 |
| **F2** | キャラクター2にコントローラーを割り当て / コントローラー2を選択 |
| **F3** | キャラクター3にコントローラーを割り当て / コントローラー3を選択 |
| **F4** | キャラクター4にコントローラーを割り当て / コントローラー4を選択 |
| **F5** | コントローラー／割り当て情報を表示 |
| **F6** | 選択中のコントローラーの割り当て解除 |
| **F7** | 全コントローラーの割り当て解除 |

> **注意:**  
> 例として **F2** を押すとコントローラー2が選択されます。  
> コントローラーが選択された状態で **F1–F4** を押すと、対応するキャラクターに割り当てられます。

### AI 制御

以下のキーでキャラクターの AI をオン／オフできます:

| キー | 機能 |
|---|---|
| **F9** | AI の切り替え |

### 例

**コントローラー2** を **キャラクター3** に割り当てる場合:

1. **F2** を押してコントローラー2を選択。
2. **F3** を押してキャラクター3に割り当て。

コントローラー2の割り当てを解除するには:

1. **F2** を押してコントローラー2を選択。
2. **F6** を押して割り当て解除。

すべての割り当てをリセットするには:

- **F7** を押します。

## ソースコードからビルドする

リポジトリをクローンします:

```bash
git clone https://github.com/<username>/GDMCoopMod.git
```

Visual Studio でソリューションを開きます。

プロジェクトをビルドします:

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

DLL を BepInEx の plugins フォルダにコピーしてテストします。

## 依存関係

### 実行時依存関係

Mod を実行するために必要なもの:

- **スターオーシャン セカンドストーリーR（PC版）**
  - 正規版が必要です。
- **BepInEx IL2CPP**
  - 必須の Mod ローダー。
  - プラグイン読み込み、IL2CPP インターフェース、Harmony パッチングを提供。

## 開発依存関係

ソースコードから Mod をビルドするために必要なもの:

### 必須ソフトウェア

- **Visual Studio 2022**
  - 必須ワークロード: `.NET デスクトップ開発`
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

BepInEx を導入し、ゲームを一度起動すると IL2CPP インターフェースアセンブリが生成されます:

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

1. ゲームディレクトリに BepInEx IL2CPP を導入。
2. ゲームを一度起動してインターフェースアセンブリを生成。
3. このリポジトリをクローン。
4. プロジェクト参照をローカルの BepInEx と interop アセンブリに更新。
5. ソリューションをビルド。

## モッディングノート

本プロジェクトは、スターオーシャン セカンドストーリーR が使用する Unity IL2CPP を対象としています。

ゲームが IL2CPP を使用しているため:
- ゲームアセンブリは BepInEx の interop ツールで生成されます。
- 開発時にはデコンパイルされたアセンブリを参照します。
- ランタイムのパッチや変更は BepInEx と Harmony を通じて行われます。

生成されたゲームファイルや interop アセンブリは意図的にリポジトリに含めていません。

## コントリビューション

改善提案、テスト報告、プルリクエストを歓迎します。

提出前の注意:
- コードは整理し、コメントを付けてください。
- 生成ファイルはコミットしないでください。
- 変更は提出前にゲーム内でテストしてください。

## ライセンス

本プロジェクトは MIT ライセンスの下で提供されています。

詳細は [LICENSE](LICENSE) を参照してください。

## 免責事項

本プロジェクトはファンによる非公式 Mod であり、スクウェア・エニックスまたはジェムドロップによる承認・支援を受けたものではありません。

Mod を使用するには、正規版のスターオーシャン セカンドストーリーRが必要です。
