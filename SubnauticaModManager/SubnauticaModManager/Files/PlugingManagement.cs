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
        Plugin.Logger.LogMessage("Attribute count: " + assembly.GetCustomAttributes(typeof(BepInPlugin), false).Length);
        object[] attributes = assembly.GetCustomAttributes(typeof(BepInPlugin), false);

        if (attributes.Length > 0)
        {
            BepInPlugin attribute = attributes[0] as BepInPlugin;
            pluginData = new PluginData(path, attribute.GUID, attribute.Version, attribute.Name, location);
            return true;
        }
        pluginData = null;
        return false;
    }
}
