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
                var data = GetPluginDataFromAssembly(dll, assembly, location);
                foreach (var pluginData in data)
                {
                    if (pluginData != null)
                    {
                        list.Add(pluginData);
                    }
                }
            }
        }
        return list;
    }

    public static List<PluginData> GetPluginDataFromAssembly(string path, Assembly assembly, PluginLocation location)
    {
        var list = new List<PluginData>();
        IEnumerable<Type> types;
        try
        {
            types = GetLoadableTypes(assembly);
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"Failed to load types from assembly '{path}'! Exception caught: " + e);
            return list;
        }
        if (types == null) return list;
        foreach (var type in types)
        {
            try
            {
                var attribute = type.GetCustomAttribute(typeof(BepInPlugin));
                if (attribute != null)
                {
                    var pluginAttribute = attribute as BepInPlugin;
                    list.Add(new PluginData(path, pluginAttribute.GUID, pluginAttribute.Version, pluginAttribute.Name, location, GetDependencies(pluginAttribute.GUID, type)));
                }
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError($"Failed to load attribute(s) in assembly '{path}'! Exception caught: " + e);
                return list;
            }
        }
        return list;
    }

    private static PluginDependency[] GetDependencies(string pluginGUID, Type pluginClass)
    {
        Attribute[] attributes = pluginClass.GetCustomAttributes(typeof(BepInDependency)).ToArray();
        if (attributes == null) return new PluginDependency[0];
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