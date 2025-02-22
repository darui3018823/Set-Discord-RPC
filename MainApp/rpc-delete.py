import json
import os
import asyncio
from pypresence import Presence

# Windows の `asyncio` のパイプエラー回避
if os.name == "nt":
    asyncio.set_event_loop(asyncio.ProactorEventLoop())

# JSON ファイルのパス（MainApp のルートから取得）
JSON_PATH = os.path.abspath(os.path.join(os.path.dirname(__file__), "../MainApp/processes.json"))

def load_client_ids():
    """ processes.json から Client ID を取得する """
    if not os.path.exists(JSON_PATH):
        print(f"Error: Configuration file not found at {JSON_PATH}")
        return {}

    with open(JSON_PATH, "r", encoding="utf-8") as f:
        data = json.load(f)

    client_ids = {name: cfg["ClientId"] for name, cfg in data.items() if "ClientId" in cfg}
    return client_ids

def clear_rpc(client_id):
    """ 指定した Client ID の RPC をクリア """
    try:
        RPC = Presence(client_id, loop=asyncio.get_event_loop())  # 明示的に `ProactorEventLoop` を使用
        RPC.connect()
        RPC.clear()
        RPC.close()
        print(f"✅ Successfully cleared RPC for Client ID: {client_id}")
    except Exception as e:
        print(f"❌ Failed to clear RPC for Client ID {client_id}: {e}")

def main():
    """ ユーザーに Client ID を選択させて RPC を削除する """
    client_ids = load_client_ids()

    if not client_ids:
        print("Error: No Client IDs found in processes.json")
        return

    print("\nAvailable Client IDs:")
    for i, (name, client_id) in enumerate(client_ids.items(), start=1):
        print(f"{i}. {name} ({client_id})")

    print("\n0. Clear all RPCs")

    try:
        choice = int(input("\nSelect the number of the Client ID to clear (0 for all): "))

        if choice == 0:
            # 全ての RPC を削除
            for name, client_id in client_ids.items():
                clear_rpc(client_id)
        elif 1 <= choice <= len(client_ids):
            selected_name = list(client_ids.keys())[choice - 1]
            clear_rpc(client_ids[selected_name])
        else:
            print("❌ Invalid selection. Exiting.")
    except ValueError:
        print("❌ Invalid input. Please enter a number.")

if __name__ == "__main__":
    main()
