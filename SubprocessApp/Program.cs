using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using DiscordRPC;

class SubApp
{
    static void Main(string[] args)
    {
        PrintStartupMessage();

        if (args.Length < 1)
        {
            Log("Error: No process name provided.", "ERROR");
            return;
        }

        string processName = args[0];
        var config = LoadConfig(processName);

        if (config == null)
        {
            Log($"No configuration found for: {processName}", "ERROR");
            return;
        }

        try
        {
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
                Buttons = config.Buttons?.Select(button => new DiscordRPC.Button
                {
                    Label = button.Label.Length > 32 ? button.Label.Substring(0, 32) : button.Label,
                    Url = button.Url
                }).ToArray(),
                Timestamps = Timestamps.Now
            });

            Log($"Rich Presence updated for {processName}. Press Ctrl+C to exit.", "INFO");

            while (true)
            {
                client.Invoke();

                // **対象のプロセスがまだ動いているか確認**
                bool processExists = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(processName)).Any();

                if (!processExists)
                {
                    Log($"Process {processName} has exited. Shutting down SubApp...", "WARN");
                    break;
                }

                System.Threading.Thread.Sleep(5000);
            }

            Log("SubApp is exiting...", "INFO");
        }
        catch (Exception ex)
        {
            Log($"Unhandled exception: {ex.Message}", "ERROR");
        }
    }

    static void PrintStartupMessage()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("============================================");
        Console.WriteLine("   Discord Rich Presence SubApp");
        Console.WriteLine("   Created by darui3018823");
        Console.WriteLine("   Version: SubApp Version 1.2.0");
        Console.WriteLine("   Repository: https://github.com/darui3018823/Set-Discord-RPC");
        Console.WriteLine("============================================");
        Console.ResetColor();
    }

    static void Log(string message, string type = "INFO")
    {
        switch (type.ToUpper())
        {
            case "INFO":
                Console.ForegroundColor = ConsoleColor.Green;
                break;
            case "ERROR":
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case "DEBUG":
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case "WARN":
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            default:
                Console.ResetColor();
                break;
        }

        Console.WriteLine($"[{type}] {message}");
        Console.ResetColor();
    }

    
    static ProcessConfig? LoadConfig(string processName)
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        string jsonPath1 = Path.Combine(basePath, "processes.json");
        string jsonPath2 = Path.Combine(basePath, @"..\..\..\processes.json");

        string jsonPath = File.Exists(jsonPath1) ? jsonPath1 : jsonPath2;

        if (!File.Exists(jsonPath))
        {
            Log($"Configuration file not found at: {jsonPath}", "ERROR");
            return null;
        }

        try
        {
            var json = File.ReadAllText(jsonPath);
            var config = JsonSerializer.Deserialize<Dictionary<string, ProcessConfig>>(json);

            return config != null && config.ContainsKey(processName) ? config[processName] : null;
        }
        catch (Exception ex)
        {
            Log($"Error reading processes.json: {ex.Message}", "ERROR");
            return null;
        }
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
    public string? PartyId { get; set; }
    public int[]? PartySize { get; set; }
    public int Priority { get; set; }
}

public class Button
{
    public string Label { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
