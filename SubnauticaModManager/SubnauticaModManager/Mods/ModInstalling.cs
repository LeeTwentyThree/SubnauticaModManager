using System.IO.Compression;

namespace SubnauticaModManager.Mods;

internal static class ModInstalling
{
    public static string ModDownloadsPath => FileManagement.ModDownloadFolder;
    public static string ModDataPath => FileManagement.ModManagerDataFolderPath;

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
}
