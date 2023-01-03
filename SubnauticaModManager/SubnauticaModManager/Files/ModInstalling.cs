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
        var mods = GetModDownloadZips();
        int failed = 0;
        int successes = 0;
        int updates = 0;
        for (int i = 0; i < mods.Length; i++)
        {
            try
            {
                InstallOrUpdateMod(mods[i], out bool update);
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

    public static void InstallOrUpdateMod(string modPath, out bool update)
    {
        var folderName = Path.GetFileNameWithoutExtension(modPath) + "-" + FileManagement.GetPartialGUID(8);
        var extractionDirectory = Path.Combine(FileManagement.TempModExtractionsFolder, folderName);
        if (!Directory.Exists(extractionDirectory)) Directory.CreateDirectory(extractionDirectory);
        FileManagement.UnzipContents(modPath, extractionDirectory, false);
        ModArrangement.DeleteFileSafely(modPath);
        update = false;
    }
}
