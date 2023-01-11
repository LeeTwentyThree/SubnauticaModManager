using SubnauticaModManager.Files;
using System;
using System.Reflection;

namespace SubnauticaModManager.Files;

internal static class PluginUtils
{
    public static List<PluginData> FilterPluginsFromDLLs(AppDomain domain, string[] dlls, PluginLocation location)
    {
        var list = new List<PluginData>();
        foreach (var dll in dlls)
        {
            var assemblyName = new AssemblyName();
            assemblyName.CodeBase = dll;
            Assembly assembly = null;
            try
            {
                if (location == PluginLocation.Plugins)
                {
                    assembly = domain.Load(assemblyName);
                }
                else
                {
                    assembly = Assembly.Load(File.ReadAllBytes(dll));
                }
            }
            catch (Exception e) { } // { Plugin.Logger.LogError($"Failed to load assembly '{dll}'! Exception caught: " + e); }
            if (assembly != null)
            {
                bool valid = false;
                PluginData data = null;
                try
                {
                    valid = TryGetPluginDataFromAssembly(dll, assembly, location, out data);
                }
                catch (Exception e)
                {
                    // Plugin.Logger.LogError($"Failed to load types/attributes from assembly '{dll}'! Exception caught: " + e);
                }
                if (valid && data != null)
                {
                    list.Add(data);
                }
            }
        }
        return list;
    }

    public static bool TryGetPluginDataFromAssembly(string path, Assembly assembly, PluginLocation location, out PluginData pluginData)
    {
        var types = GetLoadableTypes(assembly);
        foreach (var type in types)
        {
            var attribute = type.GetCustomAttribute(typeof(BepInPlugin));
            if (attribute != null)
            {
                var pluginAttribute = attribute as BepInPlugin;
                pluginData = new PluginData(path, pluginAttribute.GUID, pluginAttribute.Version, pluginAttribute.Name, location, GetDependencies(pluginAttribute.GUID, type));
                return true;
            }
        }
        pluginData = null;
        return false;
    }

    private static PluginDependency[] GetDependencies(string pluginGUID, Type pluginClass)
    {
        Attribute[] attributes = pluginClass.GetCustomAttributes(typeof(BepInDependency)).ToArray();
        if (attributes == null || pluginGUID == "Tobey.BepInEx.ConfigurationManagerTweaks.Subnautica") return new PluginDependency[0];
        PluginDependency[] dependencies = new PluginDependency[attributes.Length];
        for (int i = 0; i < attributes.Length; i++)
        {
            var dependencyAtIndex = attributes[i] as BepInDependency;
            dependencies[i] = new PluginDependency(dependencyAtIndex.DependencyGUID, dependencyAtIndex.Flags, dependencyAtIndex.MinimumVersion);
        }
        return dependencies;
    }

    public static List<PluginData> GetAllPluginDataInFolder(string folder, PluginLocation location)
    {
        var dllsInPluginsFolder = FileManagement.GetDLLs(folder);
        AppDomain domain = AppDomain.CreateDomain("modviewer");
        var plugins = FilterPluginsFromDLLs(domain, dllsInPluginsFolder, location);
        AppDomain.Unload(domain);
        return plugins;
    }

    public static bool TryGetMatchingPluginData(List<PluginData> collection, string GUID, out PluginData matching)
    {
        foreach (var pluginData in collection)
        {
            if (pluginData.GUID.Equals(GUID))
            {
                matching = pluginData;
                return true;
            }
        }
        matching = null;
        return false;
    }

    public static List<PluginData> GetAllPluginData(bool sort)
    {
        List<PluginData> pluginDataList = new List<PluginData>();

        AppDomain domain = AppDomain.CreateDomain("modviewer");

        var dllsInPluginsFolder = FileManagement.GetDLLs(FileManagement.BepInExPluginsFolder);
        pluginDataList.AddRange(FilterPluginsFromDLLs(domain, dllsInPluginsFolder, PluginLocation.Plugins));

        var dllsInDisabledFolder = FileManagement.GetDLLs(FileManagement.DisabledModsFolder);
        pluginDataList.AddRange(FilterPluginsFromDLLs(domain, dllsInDisabledFolder, PluginLocation.Disabled));

        AppDomain.Unload(domain);

        if (sort)
        {
            pluginDataList.Sort((item1, item2) => string.Compare(item1.Name, item2.Name));
        }

        return pluginDataList;
    }

    public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            return e.Types.Where(t => t != null);
        }
    }
}