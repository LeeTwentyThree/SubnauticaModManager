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
    private static string _modDataFolderPath;
    private static string _tempModExtractionsFolderPath;
    private static string _modManagerFolderPath;

    private const string kModDownloadFolderName = "ModDownloads";
    private const string kDisabledModsFolderName = "DisabledMods";
    private const string kTempFolderName = "Temp";
    private const string kAssetsFolderName = "Assets";
    private const string kImageCacheFolderName = "ImageCache";
    private const string kModDataFolderName = "ModData";
    private const string kTempModExtractionsFolderName = "Installation";
    private const string kModManagerFolderName = "SNModManager";

    public static string BepInExFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_bepInExFolderPath))
            {
                _bepInExFolderPath = Path.Combine(SubnauticaFolder, "BepInEx");
                if (!Directory.Exists(_bepInExFolderPath))
                {
                    Directory.CreateDirectory(_bepInExFolderPath);
                }
            }
            return _bepInExFolderPath;
        }
    }

    public static string ModManagerFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_modManagerFolderPath))
            {
                _modManagerFolderPath = Path.Combine(SubnauticaFolder, kModManagerFolderName);
                if (!Directory.Exists(_modManagerFolderPath))
                {
                    Directory.CreateDirectory(_modManagerFolderPath);
                }
            }
            return _modManagerFolderPath;
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
                _subnauticaFolderPath = Path.GetFullPath(Paths.GameRootPath);
            }
            return _subnauticaFolderPath;
        }
    }

    public static string DefaultModDownloadFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_modDownloadFolderPath))
            {
                _modDownloadFolderPath = Path.Combine(ModManagerFolder, kModDownloadFolderName);
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
                _disabledModsFolderPath = Path.Combine(ModManagerFolder, kDisabledModsFolderName);
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
                _tempFolderPath = Path.Combine(ModManagerFolder, kTempFolderName);
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

    public static string TempModExtractionsFolder
    {
        get
        {
            if (string.IsNullOrEmpty(_tempModExtractionsFolderPath))
            {
                _tempModExtractionsFolderPath = Path.Combine(TempFolder, kTempModExtractionsFolderName);
                if (!Directory.Exists(_tempModExtractionsFolderPath))
                {
                    Directory.CreateDirectory(_tempModExtractionsFolderPath);
                }
            }
            return _tempModExtractionsFolderPath;
        }
    }

    public static string ModDataFolderPath
    {
        get
        {
            if (string.IsNullOrEmpty(_modDataFolderPath))
            {
                _modDataFolderPath = Path.Combine(ModManagerFolder, kModDataFolderName);
                if (!Directory.Exists(_modDataFolderPath))
                {
                    Directory.CreateDirectory(_modDataFolderPath);
                }
            }
            return _modDataFolderPath;
        }
    }

    public static string FromAssetsFolder(string localPath)
    {
        return Path.Combine(PluginAssetsFolder, localPath);
    }
}