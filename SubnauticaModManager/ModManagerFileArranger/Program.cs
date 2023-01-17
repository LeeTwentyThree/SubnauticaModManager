using FileArranger;
using System.Text;
using System.Diagnostics;
using ModManagerFileArranger;

internal static class Application
{
    private static StringBuilder logOutput = new StringBuilder();

    public static void Main(string[] args) // arg 0 : instructions JSON path, arg 1: game id
    {
        Log("\n[ Subnautica Mod Manager for BepInEx File Arranger ]\n");
        Log("- This application was designed for automatic use, but can be utilized manually for testing purposes.\n");

        if (args == null || args.Length < 1 || string.IsNullOrEmpty(args[0]))
        {
            Log("Invalid arguments.\n" +
                "Argument 0 should be a valid file path for the instructions JSON file. This window will automatically close on input.");
            Console.ReadKey();
            return;
        }

        var instructionsPath = args[0];

        Log($"Instructions JSON path: '{instructionsPath}'.");

        if (!File.Exists(instructionsPath))
        {
            Log("File not found! Closing.");
            SaveLogFile(instructionsPath);
            return;
        }

        Log("Path appears to be valid.");

        if (GetIsSubnauticaRunning()) Log("Game is still loaded! Waiting for it to close.");
        while (GetIsSubnauticaRunning())
        {
            Thread.Sleep(10);
        }
        Thread.Sleep(200);

        Log("Game is not active. Ready to modify files.");

        var instructionSet = new InstructionSet(instructionsPath);

        var results = instructionSet.ExecuteInstructions();

        Log($"Executed instructions. Outputting results:");
        for (int i = 0; i < results.Length; i++)
        {
            Log(i + Environment.NewLine + results[i]);
        }

        if (TryGetGameID(args, out var platform))
        {
            SNLoader.LoadGame(platform);
        }

        Log("Success!");

        SaveLogFile(instructionsPath);
    }

    private static void SaveLogFile(string instructionsPath)
    {
        try
        {
            var logOutputPath = Path.Combine(instructionsPath, @"..\..\..\", "installation-log.txt");
            File.WriteAllText(logOutputPath, logOutput.ToString());
        }
        catch { }
    }

    private static void Log(string msg = "")
    {
        if (msg != null)
        {
            Console.WriteLine(msg);
            logOutput.AppendLine(msg);
        }
    }

    private static bool GetIsSubnauticaRunning()
    {
        return Process.GetProcessesByName("subnautica").Length > 0;
    }

    private static bool TryGetGameID(string[] args, out SNPlatform platform)
    {
        if (args.Length >= 2)
        {
            if (args[1] != null && int.TryParse(args[1], out int result))
            {
                platform = (SNPlatform)result;
                Log($"Launching game on platform {platform}.");
                return platform != SNPlatform.Unknown;
            }
        }
        platform = SNPlatform.Unknown;
        Log("Unable to determine platform. Game will not restart.");
        return false;
    }
}