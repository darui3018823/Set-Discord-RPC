# Discord Rich Presence Manager

このプロジェクトは、特定のアプリケーションが実行されているときに**Discord Rich Presence**を自動的に更新します。

![GitHub License](https://img.shields.io/github/license/darui3018823/Set-Discord-RPC)
![GitHub Repo Size](https://img.shields.io/github/repo-size/darui3018823/Set-Discord-RPC)

---

## 🚀 機能
✅ **アプリケーションごとにDiscord Rich Presenceを自動更新**  
✅ **複数のアプリケーションをサポート（並列処理）**  
✅ **アプリケーション終了時にRich Presenceを閉じる**  
✅ **`processes.json`を介したカスタマイズ可能な設定**  
✅ **軽量で高速な実行**  

---

## 📌 スクリーンショット
### **実行中のコンソール**
<img src="https://raw.githubusercontent.com/darui3018823/Set-Discord-RPC/main/assets/screenshot.png" width="800">

---

## 🎯 インストール
### **1. リポジトリをクローン**
```sh
git clone https://github.com/darui3018823/Set-Discord-RPC.git
cd Set-Discord-RPC
```

### **2. .NET SDKをインストール**
このアプリケーションは **.NET 9.0** で動作します。<br>
最新バージョンを[Microsoft .NET](https://dotnet.microsoft.com/ja-jp/download)からダウンロードしてインストールしてください。

### 3. **依存関係を復元**
```sh
dotnet restore
```

### **4. プロジェクトをビルド**
```sh
dotnet build
```

### **5. 実行**
```sh
cd MainApp/bin/Debug/net9.0
./MainApp.exe
```

## **⚙ JSONファイルの設定**
サポートされているアプリケーションは`processes.json`で追加、変更、削除できます。
<img src="https://raw.githubusercontent.com/darui3018823/Set-Discord-RPC/main/assets/rpcsc.png" width="800">
```json
{
    "AppleMusic.exe": {
        "ClientId": "012345678901234567",
        "State": "ステータスメッセージ",
        "Details": "詳細",
        "LargeImage": "Apple_Music_Logo",
        "LargeImageText": "Apple Music",
        "SmallImage": "Apple_Logo",
        "SmallImageText": "Apple",
        "Buttons": [
            {"Label": "Apple Music Web Player", "Url": "https://music.apple.com/"}
        ],
        "PartyId": null,
        "PartySize": [1, 1],
        "Priority": 1
    }
}
```
| キー             | 説明                        |
|-----------------|----------------------------|
| `ClientId`      | DiscordアプリケーションID  |
| `State`         | メインの詳細の下に表示されるステータスメッセージ |
| `Details`       | プレゼンスの説明           |
| `LargeImage`    | 大きなアイコンのキー       |
| `LargeImageText`| 大きなアイコンのツールチップ |
| `SmallImage`    | 小さなアイコンのキー       |
| `SmallImageText`| 小さなアイコンのツールチップ |
| `Buttons`       | ボタンのリスト（最大2つ）  |
| `(Buttons)Label`| ボタンに表示されるテキスト |
| `(Buttons)Url`  | ボタンの遷移先URL         |
| `PartyId`       | マルチプレイヤーセッション用のオプションのパーティID |
| `PartySize`     | `[現在の人数, 最大人数]` パーティの人数 |
| `Priority`      | 複数のアプリケーションが実行されている場合、どのプレゼンスが最初に更新されるかを決定 |

**現在確認されている問題**<br>
`State`と`Details`キーに問題があります。<br>
どちらか一方のキーのみが押された場合、正常に動作します。<br>
他のキーは正常に動作します。質問がある場合は、GitHub Issuesに投稿してください。

## **🛠 プロジェクト構造**
```sh
Set-Discord-RPC/
├── MainApp/        # プロセスを監視し、SubAppを起動
├── SubprocessApp/  # Discord Rich Presenceを更新
├── processes.json  # 設定ファイル
├── README.md       # このファイル
└── .gitignore      # Gitの無視リスト
```

## **📢 将来のアップデート**
- カスタムアイコンのサポート
- より最適化されたコード

## **🔗 関連リンク**
- GitHubリポジトリ: [darui3018823/Set-Discord-RPC](https://github.com/darui3018823/Set-Discord-RPC)
- .NETダウンロード: [.NET公式サイト](https://dotnet.microsoft.com/ja-jp/download)
- Discord API: [Discord Developer Portal](https://discord.com/developers/applications)

## **📄 ライセンス**
このプロジェクトは[GNU General Public License v3.0](https://github.com/darui3018823/Set-Discord-RPC/blob/main/LICENSE)の下でライセンスされています。
このライセンスの主な条件は以下の通りです：

### 許可事項:
- **商用利用**: このソフトウェアを商用目的で使用できます。
- **改変**: ソースコードを改変することができます。
- **配布**: このソフトウェアを配布することができます。
- **特許の使用**: 関連する特許権が付与されます。
- **私的使用**: 個人的な使用が許可されています。

### 制限事項:
- **責任**: このソフトウェアの使用によって生じた損害については責任を負いません。
- **保証**: ソフトウェアに対する保証は提供されません。

### 条件:
- **ライセンスおよび著作権表示**: 著作権およびライセンス表示を保持する必要があります。
- **変更の明示**: 改変が明示されている必要があります。
- **ソースコードの公開**: 改変版を配布する場合、ソースコードも同じGPLv3ライセンスの下で公開する必要があります。
- **同一ライセンス**: 派生作品は同じGPLv3ライセンスの下で配布する必要があります。

## **🎉 コントリビューション**
コントリビューションは歓迎します！バグを見つけたり、機能リクエストがある場合は、[GitHub Issues](https://github.com/darui3018823/Set-Discord-RPC/issues)に投稿してください。🚀🔥
