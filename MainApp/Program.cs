using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using DiscordRPC;


class Program
{
    static void Main(string[] args)
    {
        PrintStartupMessage();

        try
        {
            var config = LoadConfig();
            var activeProcesses = new HashSet<string>();
            var subprocesses = new Dictionary<string, Process>();
            var lastStartedProcesses = new Dictionary<string, DateTime>();

            // **MainApp が終了するときに SubApp を安全に終了**
            AppDomain.CurrentDomain.ProcessExit += (sender, e) => CleanupSubProcesses(subprocesses);

            while (true)
            {
                var runningProcesses = GetRunningProcesses(config);

                // **新しく検出されたプロセスごとに `SubApp.exe` を起動**
                foreach (var processName in runningProcesses)
                {
                    if (!activeProcesses.Contains(processName))
                    {
                        if (lastStartedProcesses.ContainsKey(processName) &&
                            (DateTime.Now - lastStartedProcesses[processName]).TotalSeconds < 10)
                        {
                            Log($"Skipping restart of {processName}, too soon.", "WARN");
                            continue;
                        }

                        Log($"Starting subprocess for: {processName}", "INFO");
                        var process = StartSubprocess(processName);
                        if (process != null)
                        {
                            subprocesses[processName] = process;
                            activeProcesses.Add(processName);
                            lastStartedProcesses[processName] = DateTime.Now;
                        }
                    }
                }

                // **終了した `SubApp.exe` をクリーンアップ**
                var finishedProcesses = activeProcesses
                    .Where(p => !runningProcesses.Contains(p) || (subprocesses.ContainsKey(p) && subprocesses[p].HasExited))
                    .ToList();

                foreach (var processName in finishedProcesses)
                {
                    Log($"Subprocess for {processName} (PID: {subprocesses[processName].Id}) has exited.", "WARN");

                    // **SubApp がクラッシュした場合、RPC をクリア**
                    ClearPresence(config[processName]);

                    subprocesses.Remove(processName);
                    activeProcesses.Remove(processName);
                }

                Thread.Sleep(5000); // **監視間隔**
            }
        }
        catch (Exception ex)
        {
            Log($"Unexpected error: {ex.Message}", "ERROR");
        }
    }

    // **初回起動メッセージ**
    static void PrintStartupMessage()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("============================================");
        Console.WriteLine("   Discord Rich Presence Manager");
        Console.WriteLine("   Created by darui3018823");
        Console.WriteLine("   Version: MainApp Version 1.3.0");
        Console.WriteLine("   Repository: https://github.com/darui3018823/Set-Discord-RPC");
        Console.WriteLine("============================================");
        Console.ResetColor();
    }

    // **ロギング関数（色付き）**
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

    // **MainApp が終了する際に SubApp をクリーンアップ**
    static void CleanupSubProcesses(Dictionary<string, Process> subprocesses)
    {
        Log("Cleaning up all SubApp processes...", "INFO");

        foreach (var process in subprocesses.Values)
        {
            if (!process.HasExited)
            {
                try
                {
                    process.Kill();
                    Log($"SubApp (PID: {process.Id}) terminated.", "WARN");
                }
                catch (Exception ex)
                {
                    Log($"Failed to terminate SubApp (PID: {process.Id}): {ex.Message}", "ERROR");
                }
            }
        }
    }

    // **プロセス設定を読み込む**
    static Dictionary<string, ProcessConfig> LoadConfig()
    {
        // `MainApp` のルートにある `processes.json` を使用する
        string jsonPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\processes.json"));

        if (!File.Exists(jsonPath))
        {
            Log($"Configuration file not found at: {jsonPath}", "ERROR");
            Environment.Exit(1);
        }

        try
        {
            var json = File.ReadAllText(jsonPath);
            return JsonSerializer.Deserialize<Dictionary<string, ProcessConfig>>(json)
                ?? throw new InvalidOperationException("Failed to load configuration.");
        }
        catch (Exception ex)
        {
            Log($"Error reading processes.json: {ex.Message}", "ERROR");
            Environment.Exit(1);
            return null;
        }
    }

    // **SubApp.exe の起動**
    static Process? StartSubprocess(string processName)
    {
        string subAppPath = @"E:\Programs\Cs\DiscordRichPresenceApp\SubprocessApp\bin\Debug\net9.0\SubApp.exe";

        if (!File.Exists(subAppPath))
        {
            Log($"SubApp.exe not found at: {subAppPath}", "ERROR");
            return null;
        }

        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = subAppPath,
                Arguments = processName,
                UseShellExecute = true,
                CreateNoWindow = false
            };

            var process = Process.Start(startInfo);
            if (process == null)
            {
                Log($"Failed to start subprocess for: {processName}", "ERROR");
                return null;
            }

            Log($"Started SubApp for {processName}, PID: {process.Id}", "INFO");
            return process;
        }
        catch (Exception ex)
        {
            Log($"Failed to start SubApp for {processName}: {ex.Message}", "ERROR");
            return null;
        }
    }

    // **RPC をクリア**
    static void ClearPresence(ProcessConfig config)
    {
        try
        {
            Log($"Clearing RPC for Client ID: {config.ClientId}", "INFO");

            using var client = new DiscordRPC.DiscordRpcClient(config.ClientId);
            client.Initialize();
            client.ClearPresence();
            Thread.Sleep(2000); // 反映待機
            client.Dispose();

            Log($"RPC successfully cleared for Client ID: {config.ClientId}", "INFO");
        }
        catch (Exception ex)
        {
            Log($"Failed to clear RPC for Client ID {config.ClientId}: {ex.Message}", "ERROR");
        }
    }

    // **実行中のプロセスを取得**
    static List<string> GetRunningProcesses(Dictionary<string, ProcessConfig> config)
    {
        var runningProcesses = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var proc in Process.GetProcesses())
        {
            try
            {
                runningProcesses.Add(proc.ProcessName);
            }
            catch (Exception)
            {
                // アクセス拒否されるプロセスをスキップ
            }
        }

        return config.Keys
            .Where(key => runningProcesses.Contains(Path.GetFileNameWithoutExtension(key)))
            .ToList();
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
