using FileArranger;
using System.Diagnostics;

internal static class Application
{
    public static void Main(string[] args) // arg 0 : instructions JSON path
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
    }

    private static bool GetIsSubnauticaRunning()
    {
        return Process.GetProcessesByName("subnautica").Length > 0;
    }
}