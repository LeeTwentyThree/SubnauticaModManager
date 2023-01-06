using System.Collections;
using System.IO.Compression;
using SubnauticaModManager.Mono;

namespace SubnauticaModManager.Files;

internal static class ModInstalling
{
    public static string ModDownloadsPath => FileManagement.ModDownloadFolder;
    public static string ModDataPath => FileManagement.ModDataFolderPath;

    private const string zipExtension = ".zip";

    public static int GetDownloadedModsCount()
    {
        int count = 0;
        var allFiles = Directory.GetFiles(ModDownloadsPath);
        foreach (var file in allFiles)
        {
            if (IsValidMod(file))
            {
                count++;
            }
        }
        return count;
    }

    public static string[] GetModDownloadZips()
    {
        List<string> downloadFiles = new List<string>();
        var allFiles = Directory.GetFiles(ModDownloadsPath);
        foreach (var file in allFiles)
        {
            if (IsValidMod(file))
            {
                downloadFiles.Add(file);
            }
        }
        return downloadFiles.ToArray();
    }

    private static bool IsValidMod(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        if (!File.Exists(path)) return false;
        return Path.GetExtension(path) == zipExtension;
    }

    public static IEnumerator InstallAllMods()
    {
        if (ModArrangement.WaitingOnRestart)
        {
            ModArrangement.WarnPossibleConflict();
            yield break;
        }
        LoadingProgress progress = new LoadingProgress();
        progress.Status = "Installing mods...";
        yield return new WaitForSeconds(0.5f);
        List<PluginData> alreadyInstalledPlugins = PluginUtils.GetAllPluginDataInFolder(FileManagement.BepInExPluginsFolder);
        var mods = GetModDownloadZips();
        int failed = 0;
        int successes = 0;
        int updates = 0;
        for (int i = 0; i < mods.Length; i++)
        {
            try
            {
                InstallOrUpdateMod(mods[i], alreadyInstalledPlugins, out bool update);
                if (update) updates++;
                else successes++;
            }
            catch (System.Exception e)
            {
                Plugin.Logger.LogError($"Failed to install mod! {e} Exception caught.");
                failed++;
            }
            progress.Progress = ((float)i + 1) / mods.Length;
        }
        progress.Complete();
        if (ModManagerMenu.main != null)
        {
            ModManagerMenu.main.prompt.Ask(
            $"{successes} mod(s) installed.\n" +
            $"{updates} mod(s) updated.\n" +
            $"{failed} failed installation(s).",
                new PromptChoice("Ok", () => ModArrangement.UrgeGameRestart(false))
            );
        }
    }

    public static void InstallOrUpdateMod(string modPath, List<PluginData> alreadyInstalledPlugins, out bool update)
    {
        // create a new unique folder to unzip the mod into, without having to deal with potential conflicts

        var folderName = Path.GetFileNameWithoutExtension(modPath) + "-" + FileManagement.GetPartialGUID(8);
        var tempModDirectory = Path.Combine(FileManagement.TempModExtractionsFolder, folderName);
        if (!Directory.Exists(tempModDirectory)) Directory.CreateDirectory(tempModDirectory);

        // unzip the mod contents into this new directory!

        FileManagement.UnzipContents(modPath, tempModDirectory, false);

        var modPlugins = PluginUtils.GetAllPluginDataInFolder(tempModDirectory); // hopefully just returns an array with 0 or 1, but a mod COULD have more than one!
        if (modPlugins.Count == 0 || modPlugins[0] == null || !modPlugins[0].IsValid) throw new NoPluginException();
        var mainPlugin = modPlugins[0];

        // get the path of the folder that is supposed to be placed into the "plugins" folder

        string thisModPluginFolder = mainPlugin.ContainingFolder;

        string pluginFolderName = new DirectoryInfo(thisModPluginFolder).Name;

        string destinationFolder = Path.Combine(FileManagement.BepInExPluginsFolder, pluginFolderName);

        update = Directory.Exists(destinationFolder);

        // move files & cleanup unneeded files. due to the nature of the ModArrangement class, these changes will only occur AFTER the game restart.

        ModArrangement.MoveDirectorySafely(thisModPluginFolder, destinationFolder);
        ModArrangement.DeleteDirectorySafely(tempModDirectory);
        ModArrangement.DeleteFileSafely(modPath);
    }
}
