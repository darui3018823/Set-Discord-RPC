# Discord Rich Presence Manager

This project automatically updates **Discord Rich Presence** when specific applications are running.

![GitHub License](https://img.shields.io/github/license/darui3018823/Set-Discord-RPC)
![GitHub Repo Size](https://img.shields.io/github/repo-size/darui3018823/Set-Discord-RPC)

---

## 🚀 Features
✅ **Automatically updates Discord Rich Presence per application**  
✅ **Supports multiple applications (parallel processing)**  
✅ **Closes Rich Presence when the application exits**  
✅ **Customizable settings via `processes.json`**  
✅ **Lightweight & fast execution**  

---

## 📌 Screenshot
### **Running Console**
<img src="https://raw.githubusercontent.com/darui3018823/Set-Discord-RPC/main/assets/screenshot.png" width="800">

---

## 🎯 Installation
### **1. Clone the repository**

```sh
git clone https://github.com/darui3018823/Set-Discord-RPC.git
cd Set-Discord-RPC
```
### **2. Install .NET SDK**
This application runs on **.NET 9.0.**<br>
Download and install the latest version from [Microsoft .NET](https://dotnet.microsoft.com/ja-jp/download).

### 3. **Restore dependencies**
```sh
dotnet restore
```

### **4. Build the project**
```sh
dotnet build
```

### **5. Run**
```sh
cd MainApp/bin/Debug/net9.0
./MainApp.exe
```

## **⚙ Configuration**
You can configure supported applications via `processes.json`.
```json
{
    "AppleMusic.exe": {
        "ClientId": "012345678901234567",
        "State": "Status Message",
        "Details": "Listen on Apple Music",
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
| Key             | Description                |
|-----------------|----------------------------|
| `ClientId`      | Discord Application ID     |
| `State`         |Status message displayed under the main details|
| `Details`       | Presence Description       |
| `LargeImage`    | Large icon key             |
| `LargeImageText`| Tooltip for the large icon |
| `SmallImage`    | Key for the small image icon|
| `SmallImageText`| Tooltip text for the small image|
| `Buttons`       | List ofbuttons (up to 2)  |
| `(Buttons)Label`| The text to display on the button|
| `(Buttons)Url`  | Button transition destination|
| `PartyId`       | Optional party ID for multiplayer sessions|
| `PartySize`     | `[current, max]` number of players in a party|
| ` Priority`     | Determines which presence gets updated first if multiple are running|

## **🛠 Project Structure**
```sh
Set-Discord-RPC/
├── MainApp/        # Monitors processes and launches SubApp
├── SubprocessApp/  # Updates Discord Rich Presence
├── processes.json  # Configuration file
├── README.md       # This file
└── .gitignore      # Git ignore list
```

## **📢 Future Updates**
- Support custom icons
- More optimized code

## **🔗 Related Links**
- GitHub Repo: [darui3018823/Set-Discord-RPC](https://github.com/darui3018823/Set-Discord-RPC)
- .NET Download: [.NET Official Site](https://dotnet.microsoft.com/ja-jp/download)
- Discord API: [Discord Developer Portal](https://discord.com/developers/applications)

## **📄 License**
This project is licensed under the [GNU General Public License v3.0](https://github.com/darui3018823/Set-Discord-RPC/blob/main/LICENSE)
The key conditions of this license are as follows:

### Permissions:
- **Commercial use**: You can use this software for commercial purposes.
- **Modification**: You are allowed to modify the source code.
- **Distribution**: You can distribute this software.
- **Patent use**: Any relevant patent rights are granted.
- **Private use**: Personal use is allowed.

### Limitations:
- **Liability**: No liability is provided for damages caused by using this software.
- **Warranty**: No warranty is provided for the software.

### Conditions:
- **License and copyright notice**: You must retain the copyright and license notices.
- **State changes**: Modifications must be explicitly stated.
- **Disclose source**: If you distribute a modified version, the source code must also be made available under the same GPLv3 license.
- **Same license**: Any derivative works must be distributed under the same GPLv3 license.

## **🎉 Contributing**
Contributions are welcome! If you find a bug or have a feature request, please open an issue on [GitHub Issues](https://github.com/darui3018823/Set-Discord-RPC/issues). 🚀🔥