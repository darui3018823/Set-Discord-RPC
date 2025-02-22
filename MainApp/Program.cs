using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

class Program
{
    static Dictionary<string, ProcessConfig> LoadConfig()
    {
        string path = "processes.json";
        if (!File.Exists(path))
        {
            Console.WriteLine($"Configuration file not found at: {Path.GetFullPath(path)}");
            Environment.Exit(1);
        }

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Dictionary<string, ProcessConfig>>(json)
            ?? throw new InvalidOperationException("Failed to load configuration.");
    }

    static List<string> GetRunningProcesses(Dictionary<string, ProcessConfig> config)
    {
        var runningProcesses = Process.GetProcesses()
            .Select(p => p.ProcessName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return config.Keys
            .Where(key => runningProcesses.Contains(Path.GetFileNameWithoutExtension(key)))
            .ToList();
    }

    static void Main(string[] args)
    {
        var config = LoadConfig();
        var activeProcesses = new HashSet<string>(); // 現在起動中のプロセス
        var subprocesses = new Dictionary<string, Process>(); // 各プロセスに対応する `SubApp.exe`
        var lastStartedProcesses = new Dictionary<string, DateTime>(); // 直近の起動時間管理

        while (true)
        {
            var runningProcesses = GetRunningProcesses(config);

            // **新しいプロセスを検出し `SubApp.exe` を起動**
            foreach (var processName in runningProcesses)
            {
                if (!activeProcesses.Contains(processName))
                {
                    // **10秒以内の再起動を防止**
                    if (lastStartedProcesses.ContainsKey(processName) &&
                        (DateTime.Now - lastStartedProcesses[processName]).TotalSeconds < 10)
                    {
                        Console.WriteLine($"Skipping restart of {processName}, too soon.");
                        continue;
                    }

                    Console.WriteLine($"Starting subprocess for: {processName}");
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
                .Where(p => subprocesses.ContainsKey(p) && subprocesses[p] != null && subprocesses[p].HasExited)
                .ToList();

            foreach (var processName in finishedProcesses)
            {
                Console.WriteLine($"Subprocess for {processName} has exited.");
                subprocesses.Remove(processName);
                activeProcesses.Remove(processName);
            }

            Thread.Sleep(5000); // **監視間隔**
        }
    }

    static Process? StartSubprocess(string processName)
    {
        var subAppPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\exe\SubApp.exe");
        if (!File.Exists(subAppPath))
        {
            Console.WriteLine($"[ERROR] SubApp.exe not found at: {subAppPath}");
            return null;
        }

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
            Console.WriteLine($"[ERROR] Failed to start subprocess for: {processName}");
            return null;
        }

        Console.WriteLine($"Started SubApp for {processName}, PID: {process.Id}");
        return process;
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
