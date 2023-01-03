using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace SubnauticaModManager;

internal static partial class FileManagement
{
    private static string _bepInExPluginsFolderPath;
    private static string _bepInExFolderPath;
    private static string _thisPluginFolderPath;
    private static string _myAssetsFolderPath;
    private static string _subnauticaFolderPath;
    private static string _modDownloadFolderPath;
    private static string _disabledModsFolderPath;
    private static string _tempFolderPath;
    private static string _imageCacheFolderPath;
    private static string _modManagerDataFolderPath;

    private const string kModDownloadFolderName = "ModDownloads";
    private const string kDisabledModsFolderName = "DisabledMods";
    private const string kTempFolderName = "Temp";
    private const string kAssetsFolderName = "Assets";
    private const string kImageCacheFolderName = "ImageCache";
    private const string kModManagerDataFolderName = "ModManagerData";

    public static string BepInExFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_bepInExFolderPath))
            {
                _bepInExFolderPath = Path.Combine(ThisPluginFolder, "../", "../");
                if (!Directory.Exists(_bepInExFolderPath))
                {
                    Directory.CreateDirectory(_bepInExFolderPath);
                }
            }
            return _bepInExFolderPath;
        }
    }

    public static string BepInExPluginsFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_bepInExPluginsFolderPath))
            {
                _bepInExPluginsFolderPath = Path.Combine(BepInExFolder, "plugins");
                if (!Directory.Exists(_bepInExPluginsFolderPath))
                {
                    Directory.CreateDirectory(_bepInExPluginsFolderPath);
                }
            }
            return _bepInExPluginsFolderPath;
        }
    }

    public static string ThisPluginFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_thisPluginFolderPath))
            {
                _thisPluginFolderPath = Path.GetDirectoryName(Plugin.assembly.Location);
            }
            return _thisPluginFolderPath;
        }
    }

    public static string PluginAssetsFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_myAssetsFolderPath))
            {
                _myAssetsFolderPath = Path.Combine(ThisPluginFolder, kAssetsFolderName);
            }
            return _myAssetsFolderPath;
        }
    }

    public static string SubnauticaFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_subnauticaFolderPath))
            {
                _subnauticaFolderPath = Path.Combine(ThisPluginFolder, @"..\..\..\");
            }
            return _subnauticaFolderPath;
        }
    }

    public static string ModDownloadFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_modDownloadFolderPath))
            {
                _modDownloadFolderPath = Path.Combine(SubnauticaFolder, kModDownloadFolderName);
                if (!Directory.Exists(_modDownloadFolderPath))
                {
                    Directory.CreateDirectory(_modDownloadFolderPath);
                }
            }
            return _modDownloadFolderPath;
        }
    }

    public static string DisabledModsFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_disabledModsFolderPath))
            {
                _disabledModsFolderPath = Path.Combine(SubnauticaFolder, kDisabledModsFolderName);
                if (!Directory.Exists(_disabledModsFolderPath))
                {
                    Directory.CreateDirectory(_disabledModsFolderPath);
                }
            }
            return _disabledModsFolderPath;
        }
    }

    public static string TempFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_tempFolderPath))
            {
                _tempFolderPath = Path.Combine(ThisPluginFolder, kTempFolderName);
                if (!Directory.Exists(_tempFolderPath))
                {
                    Directory.CreateDirectory(_tempFolderPath).Attributes = FileAttributes.Directory | FileAttributes.Hidden;
                }
            }
            return _tempFolderPath;
        }
    }

    public static string ImageCacheFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_imageCacheFolderPath))
            {
                _imageCacheFolderPath = Path.Combine(TempFolder, kImageCacheFolderName);
                if (!Directory.Exists(_imageCacheFolderPath))
                {
                    Directory.CreateDirectory(_imageCacheFolderPath);
                }
            }
            return _imageCacheFolderPath;
        }
    }

    public static string ModManagerDataFolderPath
    {
        get
        {
            if (string.IsNullOrEmpty(_modManagerDataFolderPath))
            {
                _modManagerDataFolderPath = Path.Combine(SubnauticaFolder, kModManagerDataFolderName);
                if (!Directory.Exists(_modManagerDataFolderPath))
                {
                    Directory.CreateDirectory(_modManagerDataFolderPath);
                }
            }
            return _modManagerDataFolderPath;
        }
    }

    public static string FromAssetsFolder(string localPath)
    {
        return Path.Combine(PluginAssetsFolder, localPath);
    }
}