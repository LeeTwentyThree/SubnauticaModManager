using SubnauticaModManager.Mono;
using FileArranger;
using FileArranger.Instructions;
using ModManagerFileArranger;

namespace SubnauticaModManager.Files;

internal static class ModArrangement
{
    public static bool WaitingOnRestart => _waitingOnRestart;

    public static void UrgeGameRestart(bool show)
    {
        var menu = ModManagerMenu.main;
        if (menu != null && show)
        {
            menu.prompt.Ask(
              Translation.Translate(StringConstants.restartRequired),
              Translation.Translate("RestartRequiredDescriptionA"),
              new PromptChoice(Translation.Translate("Yes"), RestartAndApplyChanges),
              new PromptChoice(Translation.Translate("OneSecond"))
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
            Translation.Translate(StringConstants.restartRequired),
            Translation.Translate("RestartRequiredDescriptionB"),
            new PromptChoice(Translation.Translate("RestartNow"), RestartAndApplyChanges),
            new PromptChoice(Translation.Translate("OneSecond"))
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

    public static void OverwriteDirectorySafely(string target, string destination)
    {
        instructions.Add(new OverwriteDirectory(target, destination));
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
        string instructionsPath = Path.Combine(FileManagement.TempModExtractionsFolder, "installation.json");
        var allInstructions = new List<Instruction>();
        allInstructions.AddRange(instructions);
        allInstructions.AddRange(ModEnablement.GetInstructionsForEnablement());
        var instructionSet = new InstructionSet(instructionsPath);
        instructionSet.instructions = allInstructions.ToArray();
        Plugin.Logger.LogMessage("Instruction set result: " + instructionSet.SaveToDisk());
        API.Run(Path.Combine(FileManagement.ThisPluginFolder, "ModManagerFileArranger.exe"), instructionsPath, GetCurrentPlatform());
        Application.Quit();
    }

    private static SNPlatform GetCurrentPlatform()
    {
        var platformUtils = PlatformUtils.main;
        if (platformUtils == null) return SNPlatform.Unknown;
        var services = PlatformUtils.main.GetServices();
        if (services == null) return SNPlatform.Unknown;
        if (services is PlatformServicesSteam)
        {
            return SNPlatform.Steam;
        }
        if (services is PlatformServicesEpic)
        {
            return SNPlatform.Epic;
        }
        return SNPlatform.Unknown;
    }
}