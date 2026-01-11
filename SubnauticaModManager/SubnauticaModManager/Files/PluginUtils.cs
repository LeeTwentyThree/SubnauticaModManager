using SubnauticaModManager.Files;
using System;
using System.Collections;
using System.Reflection;

namespace SubnauticaModManager.Files;

internal static class PluginUtils
{
    private const int MaxModsPerFrame = 5;
    private const int MaxPluginTypesPerFrame = 150;
    
    public static IEnumerator FilterPluginsFromDLLs(AppDomain domain, string[] dlls, PluginLocation location, List<PluginData> list)
    {
        int counter = 0;

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
            catch { } // (Exception e) { } { Plugin.Logger.LogError($"Failed to load assembly '{dll}'! Exception caught: " + e); }
            if (assembly != null)
            {
                yield return GetPluginDataFromAssembly(dll, assembly, location, list);
            }
            
            if (++counter % MaxModsPerFrame == 0)
                yield return null;
        }
    }

    public static IEnumerator GetPluginDataFromAssembly(string path, Assembly assembly, PluginLocation location, List<PluginData> list)
    {
        IEnumerable<Type> types;
        try
        {
            types = GetLoadableTypes(assembly);
        }
        catch (Exception e)
        {
            Plugin.Logger.LogError($"Failed to load types from assembly '{path}'! Exception caught: " + e);
            yield break;
        }
        
        int counter = 0;

        if (types == null) yield break;
        foreach (var pluginClassType in types)
        {
            try
            {
                var attribute = pluginClassType.GetCustomAttribute(typeof(BepInPlugin));
                if (attribute != null)
                {
                    var pluginAttribute = attribute as BepInPlugin;
                    list.Add(PluginData.Create(path, pluginAttribute, location, pluginClassType));
                }
            }
            catch (Exception e)
            {
                Plugin.Logger.LogError($"Failed to load attribute(s) in assembly '{path}'! Exception caught: " + e);
            }

            if (++counter % MaxPluginTypesPerFrame == 0)
                yield return null;
        }
    }

    public static PluginDependency[] GetDependencies(Type pluginClass)
    {
        Attribute[] attributes = pluginClass.GetCustomAttributes(typeof(BepInDependency)).ToArray();
        if (attributes == null) return new PluginDependency[0];
        PluginDependency[] dependencies = new PluginDependency[attributes.Length];
        for (int i = 0; i < attributes.Length; i++)
        {
            var dependencyAtIndex = attributes[i] as BepInDependency;
            dependencies[i] = new PluginDependency(dependencyAtIndex.DependencyGUID, dependencyAtIndex.Flags, new Version(dependencyAtIndex.MinimumVersion));
        }
        return dependencies;
    }

    public static IEnumerator GetAllPluginDataInFolder(string folder, PluginLocation location, List<PluginData> list)
    {
        var dllsInPluginsFolder = FileManagement.GetDLLs(folder);
        AppDomain domain = AppDomain.CreateDomain("modviewer");
        yield return FilterPluginsFromDLLs(domain, dllsInPluginsFolder, location, list);
        AppDomain.Unload(domain);
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

    public static IEnumerator GetAllPluginData(bool sort, List<PluginData> list)
    {
        AppDomain domain = AppDomain.CreateDomain("modviewer");

        var dllsInPluginsFolder = FileManagement.GetDLLs(FileManagement.BepInExPluginsFolder);
        yield return FilterPluginsFromDLLs(domain, dllsInPluginsFolder, PluginLocation.Plugins, list);

        var dllsInDisabledFolder = FileManagement.GetDLLs(FileManagement.DisabledModsFolder);
        yield return FilterPluginsFromDLLs(domain, dllsInDisabledFolder, PluginLocation.Disabled, list);

        AppDomain.Unload(domain);

        if (sort)
        {
            list.Sort((item1, item2) => string.Compare(item1.Name, item2.Name));
        }
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