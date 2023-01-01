using FileArranger;
using FileArranger.Instructions;

public static class Application
{
    public static void Main(string[] args) // arg 0 : instructions JSON path
    {
        Console.WriteLine("\n[ Subnautica Mod Manager for BepInEx File Arranger ]\n");
        Console.WriteLine("- This application was designed for automatic use, but can be utilized manually for testing purposes.\n");

        if (args == null || args.Length < 1 || string.IsNullOrEmpty(args[0]))
        {
            Console.WriteLine("Invalid arguments. Argument 0 should be a valid file path for the instructions JSON file. This window will automatically close on input.");
            Console.ReadKey();
        }

        var path = args[0];

        Console.WriteLine($"Instructions JSON path: '{path}'.");

        if (!File.Exists(path))
        {
            Console.WriteLine("File not found! Closing.");
            return;
        }

        Console.WriteLine("Path appears to be valid.");

        var instructionSet = new InstructionSet(path);

        var results = instructionSet.ExecuteInstructions();

        Console.WriteLine($"Executed instructions. Outputting results:");
        for (int i = 0; i < results.Length; i++)
        {
            Console.WriteLine(i + ": " + results[i]);
        }
    }
}