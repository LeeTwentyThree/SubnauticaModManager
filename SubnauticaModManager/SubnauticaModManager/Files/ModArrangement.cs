using SubnauticaModManager.Mono;
using FileArranger;
using FileArranger.Instructions;

namespace SubnauticaModManager.Files;

internal static class ModArrangement
{
    public static bool WaitingOnRestart => _waitingOnRestart;

    public static void UrgeGameRestart(bool show)
    {
        var menu = ModManagerMenu.main;
        if (menu != null)
        {
            menu.prompt.Ask(
              "A game restart is required. Exit now?",
              new PromptChoice("Yes", RestartAndApplyChanges),
              new PromptChoice("One second")
              );
        }
        _waitingOnRestart = true;
    }

    private static bool _waitingOnRestart;

    private static List<Instruction> instructions = new List<Instruction>();

    public static void WarnPossibleConflict()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) return;
        menu.prompt.Ask(
            "You must restart the game before doing this!",
            new PromptChoice("Exit now", RestartAndApplyChanges),
            new PromptChoice("One second")
            );
    }

    public static void MoveFileSafely(string target, string destination)
    {
        instructions.Add(new Move(target, destination));
        UrgeGameRestart(false);
    }

    public static void MoveDirectorySafely(string target, string destination)
    {
        instructions.Add(new MoveDirectory(target, destination));
        UrgeGameRestart(false);
    }

    public static void DeleteFileSafely(string target)
    {
        instructions.Add(new Delete(target));
        UrgeGameRestart(false);
    }


    public static void DeleteDirectorySafely(string target)
    {
        instructions.Add(new DeleteDirectory(target));
        UrgeGameRestart(false);
    }

    public static void RestartAndApplyChanges()
    {
        string instructionsPath = Path.Combine(FileManagement.TempModExtractionsFolder, "instructions.json");
        var instructionSet = new InstructionSet(instructionsPath);
        instructionSet.SaveToDisk();
        ModManagerFileArranger.API.Run(Path.Combine(FileManagement.ThisPluginFolder, "ModManagerFileArranger.exe"), instructionsPath);
        Application.Quit();
    }
}
