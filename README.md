# PietyCrit

A small gameplay mod for **Star Ocean: The Second Story R**, built using **BepInEx** and **Unity IL2CPP modding tools**.

In the vanilla game, each level-up of the **Piety** skill grants stat points to randomly chosen stats. This mod changes that so every Piety level-up grants **CRT** (critical rate) instead, while leaving the rest of the game's mechanics and systems untouched.

## Download

The latest releases of PietyCrit are available here:

**[PietyCrit Releases](https://github.com/GDSeirya/Star-Ocean-2-Second-Story-R-Piety-Patch/releases)**

## Features

Current features:

- Piety level-ups always grant **CRT** instead of random stats
- The number of points gained is the same as in the vanilla game
- No other skill is affected

## Requirements

### For Players

- A legitimate copy of **Star Ocean: The Second Story R**
- [BepInEx](https://github.com/BepInEx/BepInEx) IL2CPP installed
- Windows PC version of the game
- Optional: [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager) (IL2CPP build) for changing settings in-game

### For Developers

- Visual Studio 2022 (or the `dotnet` CLI)
- .NET SDK
- BepInEx IL2CPP development environment
- IL2CPP interop assemblies generated from the game

## Installation

1. Install **BepInEx IL2CPP** into your Star Ocean: The Second Story R installation.
2. Download the latest **PietyCrit.dll** from the [Releases](https://github.com/GDSeirya/Star-Ocean-2-Second-Story-R-Piety-Patch/releases) page.
3. Copy the plugin DLL into:

```text
STAR OCEAN THE SECOND STORY R/
└── BepInEx/
    └── plugins/
        └── PietyCrit.dll
```

4. Launch the game.

BepInEx should automatically load the plugin.

> **Note:** If you are building the mod from source, see [Building From Source](#building-from-source).

## Configuration

A config file is created on first launch at:

```text
BepInEx/config/local.so2r.pietycrit.cfg
```

| Section | Setting | Default | Description |
|---|---|---|---|
| General | `Enabled` | `true` | When enabled, Piety level-ups grant CRT. Changes apply immediately. |

If you have the **BepInEx Configuration Manager** installed, the setting also appears in its in-game menu. The mod works the same without it.

## Building From Source

Clone the repository:

```bash
git clone https://github.com/GDSeirya/Star-Ocean-2-Second-Story-R-Piety-Patch.git
```

Open `PietyCrit.csproj` and set `GameDir` to your game installation folder.

Build the project using Visual Studio (**Build → Build Solution**) or the command line:

```bash
dotnet build -c Release
```

The compiled DLL will be located in:

```text
bin/
└── Release/
    └── net8.0/
        └── PietyCrit.dll
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

- **Visual Studio 2022** (workload: `.NET desktop development`) or the `dotnet` CLI
- **.NET SDK**
- **Git**

### BepInEx Development Assemblies

The project references the `BepInEx.Unity.IL2CPP` NuGet package, along with the following assembly from your BepInEx installation:

```text
BepInEx/core/
└── Il2CppInterop.Runtime.dll
```

### Game IL2CPP Interop Assemblies

After installing BepInEx and launching the game once, the required IL2CPP interop assemblies will be generated:

```text
BepInEx/interop/
├── Il2Cppmscorlib.dll
├── UnityEngine.CoreModule.dll
└── Assembly-CSharp.dll
```

These files are generated from the game installation and are **not included in this repository**.

Developers must generate their own copies.

## Setting Up a Development Environment

1. Install BepInEx IL2CPP into the game directory.
2. Launch the game once to generate the IL2CPP interop assemblies.
3. Clone this repository.
4. Update `GameDir` in `PietyCrit.csproj` to point to your game folder.
5. Open the project in Visual Studio 2022, or use the `dotnet` CLI.
6. Build the project.

## Modding Notes

This project targets the IL2CPP version of Unity used by **Star Ocean: The Second Story R**.

Because the game uses IL2CPP:

- Game assemblies are generated through BepInEx's IL2CPP interop tools.
- Decompiled assemblies are used for reference during development.
- Runtime patches and modifications are implemented through BepInEx and Harmony.

How the patch works:

- The Piety branch of `Game.CharacterParameter.SkillLevelupEffect` picks a random `CharacterStatus` for each point and calls `CharacterParameter.IncreaseBaseStatus(status, value)`.
- In the current game build, that is the only caller of `IncreaseBaseStatus`.
- The plugin applies a Harmony prefix to `IncreaseBaseStatus` that forces `status` to `CharacterStatus.CRITICAL`.
- `SkillLevelupEffect` itself is deliberately **not** patched. It takes a `ref GameManager.IncreaseResourcesParameter` struct containing reference-type fields, and patching it causes `NullReferenceException` errors from Il2CppInterop at startup.

A future game update could rename or inline `IncreaseBaseStatus`. If CRT stops changing, check the BepInEx log.

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

# PietyCrit

**スターオーシャン セカンドストーリーR** 向けの小規模なゲームプレイ Mod です。  
この Mod は **BepInEx** と **Unity IL2CPP モッディングツール** を使用して構築されています。

通常のゲームでは、スキル「**信仰心**（Piety）」のレベルアップ時に、ランダムに選ばれたステータスが上昇します。この Mod は、信仰心のレベルアップで上昇するステータスをすべて **CRT**（クリティカル率）に変更します。それ以外のゲームのシステムやメカニクスには手を加えません。

## ダウンロード

PietyCrit の最新リリースはこちらからダウンロードできます。

**[PietyCrit リリースページ](https://github.com/GDSeirya/Star-Ocean-2-Second-Story-R-Piety-Patch/releases)**

## 機能

現在の機能:

- 信仰心のレベルアップで、ランダムなステータスの代わりに常に **CRT** が上昇
- 上昇量は通常のゲームと同じ
- 他のスキルには影響しません
- ゲームを再起動せずに即時反映される、オプションのオン／オフ設定
- オプションで **BepInEx Configuration Manager** に対応（必須ではありません）

## 必要環境

### プレイヤー向け

- 正規版 **スターオーシャン セカンドストーリーR**
- [BepInEx](https://github.com/BepInEx/BepInEx) IL2CPP の導入
- Windows PC 版のゲーム
- オプション: ゲーム内で設定を変更するための [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager)（IL2CPP 版）

### 開発者向け

- Visual Studio 2022（または `dotnet` CLI）
- .NET SDK
- BepInEx IL2CPP 開発環境
- ゲームから生成された IL2CPP インターフェースアセンブリ

## インストール方法

1. スターオーシャン セカンドストーリーR のゲームフォルダに **BepInEx IL2CPP** を導入します。
2. [リリースページ](https://github.com/GDSeirya/Star-Ocean-2-Second-Story-R-Piety-Patch/releases) から最新の **PietyCrit.dll** をダウンロードします。
3. プラグイン DLL を以下の場所にコピーします:

```text
STAR OCEAN THE SECOND STORY R/
└── BepInEx/
    └── plugins/
        └── PietyCrit.dll
```

4. ゲームを起動します。

BepInEx が自動的にプラグインを読み込みます。

> **注意:** ソースコードからビルドする場合は、[ソースコードからビルドする](#ソースコードからビルドする) を参照してください。

## 設定

初回起動時に、以下の場所に設定ファイルが作成されます:

```text
BepInEx/config/local.so2r.pietycrit.cfg
```

| セクション | 設定 | デフォルト | 説明 |
|---|---|---|---|
| General | `Enabled` | `true` | 有効にすると、信仰心のレベルアップで CRT が上昇します。変更は即時に反映されます。 |

**BepInEx Configuration Manager** を導入している場合は、ゲーム内メニューにもこの設定が表示されます。導入していなくても、Mod は同じように動作します。

## ソースコードからビルドする

リポジトリをクローンします:

```bash
git clone https://github.com/GDSeirya/Star-Ocean-2-Second-Story-R-Piety-Patch.git
```

`PietyCrit.csproj` を開き、`GameDir` をゲームのインストールフォルダに設定します。

Visual Studio（**Build → Build Solution**）またはコマンドラインでビルドします:

```bash
dotnet build -c Release
```

ビルドされた DLL は以下に生成されます:

```text
bin/
└── Release/
    └── net8.0/
        └── PietyCrit.dll
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

- **Visual Studio 2022**（ワークロード: `.NET デスクトップ開発`）または `dotnet` CLI
- **.NET SDK**
- **Git**

### BepInEx 開発アセンブリ

プロジェクトは `BepInEx.Unity.IL2CPP` NuGet パッケージと、BepInEx のインストール先にある以下のアセンブリを参照します:

```text
BepInEx/core/
└── Il2CppInterop.Runtime.dll
```

### ゲーム IL2CPP インターフェースアセンブリ

BepInEx を導入し、ゲームを一度起動すると、必要な IL2CPP インターフェースアセンブリが生成されます:

```text
BepInEx/interop/
├── Il2Cppmscorlib.dll
├── UnityEngine.CoreModule.dll
└── Assembly-CSharp.dll
```

これらはゲームから生成されるため、**本リポジトリには含まれていません**。

開発者は自身の環境で生成する必要があります。

## 開発環境のセットアップ

1. ゲームディレクトリに BepInEx IL2CPP を導入します。
2. ゲームを一度起動して IL2CPP インターフェースアセンブリを生成します。
3. このリポジトリをクローンします。
4. `PietyCrit.csproj` の `GameDir` を、ゲームフォルダに合わせて更新します。
5. Visual Studio 2022 でプロジェクトを開くか、`dotnet` CLI を使用します。
6. プロジェクトをビルドします。

## モッディングノート

本プロジェクトは、**スターオーシャン セカンドストーリーR** が使用する Unity IL2CPP を対象としています。

ゲームが IL2CPP を使用しているため:

- ゲームアセンブリは BepInEx の IL2CPP インターフェースツールによって生成されます。
- 開発時にはデコンパイルされたアセンブリを参照します。
- ランタイムのパッチや変更は BepInEx と Harmony を通じて行われます。

パッチの仕組み:

- `Game.CharacterParameter.SkillLevelupEffect` の信仰心の処理は、ポイントごとにランダムな `CharacterStatus` を選び、`CharacterParameter.IncreaseBaseStatus(status, value)` を呼び出します。
- 現在のゲームビルドでは、これが `IncreaseBaseStatus` の唯一の呼び出し元です。
- プラグインは `IncreaseBaseStatus` に Harmony のプレフィックスを適用し、`status` を `CharacterStatus.CRITICAL` に強制します。
- `SkillLevelupEffect` 自体は意図的にパッチ**していません**。このメソッドは参照型フィールドを含む `ref GameManager.IncreaseResourcesParameter` 構造体を受け取るため、パッチすると起動時に Il2CppInterop から `NullReferenceException` が発生します。

今後のゲームアップデートで `IncreaseBaseStatus` の名前が変わったりインライン化されたりする可能性があります。CRT が変化しなくなった場合は、BepInEx のログを確認してください。

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
