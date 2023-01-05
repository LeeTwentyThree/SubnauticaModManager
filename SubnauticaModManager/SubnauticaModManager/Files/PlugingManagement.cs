using SubnauticaModManager.Files;
using System;
using System.Reflection;

namespace SubnauticaModManager.Files;

internal static class PlugingManagement
{
    public static List<PluginData> FilterPluginsFromDLLs(string[] dlls, PluginLocation location)
    {
        var list = new List<PluginData>();
        foreach (var dll in dlls)
        {
            Assembly assembly = null;
            try
            {
                assembly = Assembly.Load(File.ReadAllBytes(dll));
            }
            catch { }
            if (assembly != null)
            {
                if (TryGetPluginDataFromAssembly(dll, assembly, location, out var data))
                {
                    list.Add(data);
                }
            }
        }
        return list;
    }

    public static bool TryGetPluginDataFromAssembly(string path, Assembly assembly, PluginLocation location, out PluginData pluginData)
    {
        var types = assembly.GetTypes();
        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute(typeof(BepInPlugin));
            if (attribute != null)
            {
                var pluginAttribute = attribute as BepInPlugin;
                pluginData = new PluginData(path, pluginAttribute.GUID, pluginAttribute.Version, pluginAttribute.Name, location);
                return true;
            }
        }
        pluginData = null;
        return false;
    }

    public static List<PluginData> GetAllPluginData(bool sort)
    {
        List<PluginData> pluginDataList = new List<PluginData>();

        var dllsInPluginsFolder = FileManagement.GetDLLs(FileManagement.BepInExPluginsFolder);
        pluginDataList.AddRange(FilterPluginsFromDLLs(dllsInPluginsFolder, PluginLocation.Plugins));

        var dllsInDisabledFolder = FileManagement.GetDLLs(FileManagement.DisabledModsFolder);
        pluginDataList.AddRange(FilterPluginsFromDLLs(dllsInDisabledFolder, PluginLocation.Disabled));

        if (sort)
        {
            pluginDataList.Sort((item1, item2) => string.Compare(item1.Name, item2.Name));
        }

        return pluginDataList;
    }
}
