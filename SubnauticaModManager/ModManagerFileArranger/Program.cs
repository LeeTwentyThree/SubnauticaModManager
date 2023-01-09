using FileArranger;
using System.Diagnostics;
using ModManagerFileArranger;

internal static class Application
{
    public static void Main(string[] args) // arg 0 : instructions JSON path, arg 1: game id
    {
        Console.WriteLine("\n[ Subnautica Mod Manager for BepInEx File Arranger ]\n");
        Console.WriteLine("- This application was designed for automatic use, but can be utilized manually for testing purposes.\n");

        if (args == null || args.Length < 1 || string.IsNullOrEmpty(args[0]))
        {
            Console.WriteLine("Invalid arguments.\n" +
                "Argument 0 should be a valid file path for the instructions JSON file. This window will automatically close on input.");
            Console.ReadKey();
            return;
        }

        var instructionsPath = args[0];

        Console.WriteLine($"Instructions JSON path: '{instructionsPath}'.");

        if (!File.Exists(instructionsPath))
        {
            Console.WriteLine("File not found! Closing.");
            return;
        }

        Console.WriteLine("Path appears to be valid.");

        if (GetIsSubnauticaRunning()) Console.WriteLine("Game is still loaded! Waiting for it to close.");
        while (GetIsSubnauticaRunning())
        {
            Thread.Sleep(10);
        }
        Thread.Sleep(200);

        Console.WriteLine("Game is not active. Ready to modify files.");

        var instructionSet = new InstructionSet(instructionsPath);

        var results = instructionSet.ExecuteInstructions();

        Console.WriteLine($"Executed instructions. Outputting results:");
        for (int i = 0; i < results.Length; i++)
        {
            Console.WriteLine(i + ": " + results[i]);
        }

        if (TryGetGameID(args, out var platform))
        {
            SNLoader.LoadGame(platform);
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
                return platform != SNPlatform.Unknown;
            }
        }
        platform = SNPlatform.Unknown;
        return false;
    }
}