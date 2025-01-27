using System;                         // Console クラスのため
using System.Collections.Generic;     // List<> のため
using System.IO;                      // ファイル操作のため
using System.Text.Json;               // JSON 操作のため
using DiscordRPC;                     // DiscordRPC クラスのため

class SubApp
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Error: No process name provided.");
            return;
        }

        string processName = args[0];
        var config = LoadConfig(processName);

        if (config == null)
        {
            Console.WriteLine($"No configuration found for: {processName}");
            return;
        }

        if (string.IsNullOrEmpty(config.ClientId))
        {
            Console.WriteLine($"Error: Client ID is missing for process: {processName}");
            return;
        }

        var client = new DiscordRpcClient(config.ClientId);
        client.Initialize();

        client.SetPresence(new RichPresence()
        {
            Details = config.Details ?? "No details available",
            State = config.State ?? "No state available",
            Assets = new Assets()
            {
                LargeImageKey = config.LargeImage,
                LargeImageText = config.LargeImageText,
                SmallImageKey = config.SmallImage,
                SmallImageText = config.SmallImageText
            },
            Buttons = config.Buttons?.ConvertAll(button => new DiscordRPC.Button
            {
                Label = button.Label,
                Url = button.Url
            }).ToArray(),
            Timestamps = Timestamps.Now
        });

        Console.WriteLine($"Rich Presence updated for {processName}. Press Ctrl+C to exit.");
        while (true)
        {
            client.Invoke();
            System.Threading.Thread.Sleep(2000);
        }
    }

    static ProcessConfig? LoadConfig(string processName)
    {
        var config = JsonSerializer.Deserialize<Dictionary<string, ProcessConfig>>(File.ReadAllText("../MainApp/processes.json"));
        return config != null && config.ContainsKey(processName) ? config[processName] : null;
    }
}

public class ProcessConfig
{
    public string? ClientId { get; set; }
    public string? State { get; set; }
    public string? Details { get; set; }
    public string? LargeImage { get; set; }
    public string? LargeImageText { get; set; }
    public string? SmallImage { get; set; }
    public string? SmallImageText { get; set; }
    public List<Button>? Buttons { get; set; }
}

public class Button
{
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
