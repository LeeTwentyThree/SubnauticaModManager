using System.Text.RegularExpressions;

namespace SubnauticaModManager.Files;

internal static class Cleanup
{
    public static void CleanupCache()
    {
        var tempModFolderDirectories = Directory.GetDirectories(FileManagement.TempModExtractionsFolder, "*", SearchOption.TopDirectoryOnly);
        foreach (var dir in tempModFolderDirectories)
        {
            if (Directory.Exists(dir)) Directory.Delete(dir, true);
        }
        var tempModFoldersFiles = Directory.GetFiles(FileManagement.TempModExtractionsFolder, "*", SearchOption.TopDirectoryOnly);
        foreach (var file in tempModFoldersFiles)
        {
            if (File.Exists(file)) File.Delete(file);
        }
    }
}
