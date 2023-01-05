using System.IO.Compression;

namespace SubnauticaModManager;

internal static partial class FileManagement
{
    public static void UnzipContents(string path, string intoDirectory, bool deleteZipFile)
    {
        if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(intoDirectory))
        {
            Plugin.Logger.LogError($"Empty path(s) detected while trying to unzip a file.");
            return;
        }
        if (!File.Exists(path) || !Directory.Exists(intoDirectory))
        {
            Plugin.Logger.LogError($"Invalid path/directory detected while trying to unzip a file.");
            return;
        }
        ZipFile.ExtractToDirectory(path, intoDirectory);
        if (deleteZipFile)
        {
            File.Delete(path);
        }
    }

    public static string GetPartialGUID(int length)
    {
        var guid = System.Guid.NewGuid();
        return guid.ToString().Substring(0, length);
    }

    public static string[] GetDLLs(string inDirectory, SearchOption searchOption = SearchOption.AllDirectories)
    {
        List<string> dlls = new List<string>();
        var allFiles = Directory.GetFiles(inDirectory, "*", searchOption);
        foreach (var file in allFiles)
        {
            if (IsDLL(file))
            {
                dlls.Add(file);
            }
        }
        return dlls.ToArray();
    }

    private static bool IsDLL(string path) => Path.GetExtension(path) == ".dll";
}
