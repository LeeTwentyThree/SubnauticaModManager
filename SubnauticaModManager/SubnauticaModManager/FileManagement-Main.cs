using System;
using System.IO.Compression;
using System.Security.Cryptography;
using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;
using SharpCompress.Common;

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

        var extension = Path.GetExtension(path).ToLowerInvariant();
        if (extension == ".zip")
        {
            ZipFile.ExtractToDirectory(path, intoDirectory);
        }
        else if (extension == ".7z")
        {
            using var archive = SevenZipArchive.Open(path);
            foreach (var entry in archive.Entries)
            {
                if (!entry.IsDirectory)
                    entry.WriteToDirectory(intoDirectory, new ExtractionOptions 
                    { 
                        ExtractFullPath = true, 
                        Overwrite = true 
                    });
            }
        }
        else
        {
            throw new NotSupportedException("Unsupported file extension: " + extension);
        }

        if (deleteZipFile)
        {
            File.Delete(path);
        }
    }

    public static string GetPartialGUID(int length)
    {
        var guid = Guid.NewGuid();
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

    private static bool IsDLL(string path) => Path.GetExtension(path).ToLower() == ".dll";

    public static string NormalizePath(string path)
    {
        return Path.GetFullPath(new Uri(path).LocalPath)
                   .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                   .ToUpperInvariant();
    }

    /* Left over from Submodica integration
    public static string GetMD5Checksum(string filename)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filename))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
    */
}
