using SubnauticaModManager.Mono;

namespace SubnauticaModManager.Files;

internal static class VerifyIntegrity
{
    private static bool hasVerified;

    private static bool isIntact;

    private static string[] requiredFiles = new string[]
    {
        "FileArranger.dll",
        "ModManagerFileArranger.exe",
        "Newtonsoft.Json.dll",
        "libwebp_x64.dll",
        "libwebp_x86.dll",
        "Semver.dll"
    };

    public static bool IsIntact
    {
        get
        {
            if (!hasVerified)
            {
                isIntact = GetMissingFiles().Count == 0;
                hasVerified = true;
            }
            return isIntact;
        }
    }

    public static List<string> GetMissingFiles()
    {
        var list = new List<string>();
        foreach (var required in requiredFiles)
        {
            CheckMissingFile(required, list);
        }
        return list;
    }

    private static void CheckMissingFile(string fileName, List<string> list)
    {
        if (!File.Exists(Path.Combine(FileManagement.ThisPluginFolder, fileName)))
        {
            list.Add(fileName);
        }
    }

    public static void WarnNotIntact()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) return;
        string desc = Translation.Translate("FilesNotIntactWarning") + "\n";
        foreach (var file in GetMissingFiles())
        {
            desc += file + " ";
        }
        menu.prompt.Ask(
            Translation.Translate(StringConstants.missingFiles),
            desc,
            new PromptChoice(Translation.Translate("Close"))
            );
    }
}
