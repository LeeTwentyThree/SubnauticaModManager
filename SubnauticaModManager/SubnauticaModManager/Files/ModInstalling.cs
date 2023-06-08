using System.Collections;
using SubnauticaModManager.Mono;
using SubnauticaModManager.Web;

namespace SubnauticaModManager.Files;

internal static class ModInstalling
{
    private const string zipExtension = ".zip";

    public static int GetDownloadedModsCount()
    {
        int count = 0;
        var allFiles = Directory.GetFiles(CustomDownloadsFolder.CurrentModDownloadFolder);
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
        var allFiles = Directory.GetFiles(CustomDownloadsFolder.CurrentModDownloadFolder);
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
        List<PluginData> alreadyInstalledPlugins = PluginUtils.GetAllPluginData(false);
        var modZips = GetModDownloadZips();
        InstallResults results = new InstallResults();
        for (int i = 0; i < modZips.Length; i++)
        {
            try
            {
                InstallOrUpdatePlugins(modZips[i], alreadyInstalledPlugins, results);
            }
            catch (System.Exception e)
            {
                Plugin.Logger.LogError($"Failed to install mod(s) in zip file at path {modZips[i]}!\nException caught: {e}");
                results.AddOne(InstallResultType.Failure);
            }
            progress.Progress = ((float)i + 1) / modZips.Length;
        }
        progress.Complete();
        if (ModManagerMenu.main != null)
        {
            var installText = $"{results.successes} mod(s) installed.\n" +
            $"{results.updates} mod(s) updated.\n" +
            $"{results.failures} failed installation(s).";
            if (results.attemptedToInstallSelf) installText += "\nError: Mod manager must be updated manually!";
            ModManagerMenu.main.prompt.Ask(
            StringConstants.installationCompleted, installText, new PromptChoice("Ok", () => ModArrangement.UrgeGameRestart(true))
            );
        }
    }

    public static void InstallOrUpdatePlugins(string zipPath, List<PluginData> alreadyInstalledPlugins, InstallResults results)
    {
        // create a new unique folder to unzip the mod into, without having to deal with potential conflicts

        var folderName = Path.GetFileNameWithoutExtension(zipPath).Truncate(48) + "-" + FileManagement.GetPartialGUID(8);
        var tempModDirectory = Path.Combine(FileManagement.TempModExtractionsFolder, folderName);
        if (!Directory.Exists(tempModDirectory)) Directory.CreateDirectory(tempModDirectory);

        // unzip the mod contents into this new directory!

        FileManagement.UnzipContents(zipPath, tempModDirectory, false);

        var modPlugins = PluginUtils.GetAllPluginDataInFolder(tempModDirectory, PluginLocation.Uninstalled); // a single file could have MULTIPLE DLLs

        bool multiplePluginsInZip = modPlugins.Count > 1;

        foreach (var plugin in modPlugins)
        {
            InstallResultType resultType;
            try
            {
                UpdateOrInstallPlugin(plugin, alreadyInstalledPlugins, results, out resultType);
            }
            catch
            {
                Plugin.Logger.LogError($"Failed to install plugin '{plugin.GUID}'!");
                resultType = InstallResultType.Failure;
            }
            results.AddOne(resultType);
            Plugin.Logger.Log(resultType == InstallResultType.Failure ? LogLevel.Error : LogLevel.Message, InstallResults.FormatResult(plugin, resultType));
            if (resultType != InstallResultType.Failure && plugin.GUID.Length > 1 && !multiplePluginsInZip)
            {
                SubmodicaAPI.RecordGUIDToSubmodica(FileManagement.GetMD5Checksum(zipPath), plugin.GUID);
            }
        }

        ModArrangement.DeleteDirectorySafely(tempModDirectory);
        ModArrangement.DeleteFileSafely(zipPath);
    }

    public static void UpdateOrInstallPlugin(PluginData plugin, List<PluginData> alreadyInstalledPlugins, InstallResults results, out InstallResultType resultType)
    {
        if (plugin.GUID == Plugin.GUID)
        {
            results.attemptedToInstallSelf = true;
            resultType = InstallResultType.Failure;
            return;
        }

        bool isUpdate = false;

        // get the path of the folder that is supposed to be placed into the actual BepInEx "plugins" folder

        string thisModPluginFolder = plugin.ContainingFolder;

        string pluginFolderName = new DirectoryInfo(thisModPluginFolder).Name;

        if (pluginFolderName.ToLower() == "plugins") pluginFolderName = Path.GetFileNameWithoutExtension(plugin.dllPath).Truncate(48);

        string destinationFolder = Path.Combine(FileManagement.BepInExPluginsFolder, pluginFolderName);

        foreach (var installed in alreadyInstalledPlugins)
        {
            if (installed.GUID == plugin.GUID)
            {
                destinationFolder = installed.ContainingFolder;
                isUpdate = true;
            }
        }

        // move files & cleanup unneeded files. due to the nature of the ModArrangement class, these changes will only occur AFTER the game restart.

        ModArrangement.OverwriteDirectorySafely(thisModPluginFolder, destinationFolder);

        if (isUpdate) resultType = InstallResultType.Update;
        else resultType = InstallResultType.Success;
    }
}
