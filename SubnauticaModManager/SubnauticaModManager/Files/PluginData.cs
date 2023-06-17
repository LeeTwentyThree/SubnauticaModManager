using System;

namespace SubnauticaModManager.Files;

internal class PluginData
{
    public string DllPath { get; }
    public string GUID { get; }
    public Version Version { get; }
    public string Name { get; }
    public PluginLocation Location { get; }
    public PluginDependency[] Dependencies { get; }
    public bool PluginIsLoaded { get; }

    public string ContainingFolder => Path.GetFullPath(Path.Combine(DllPath, @"..\"));

    private PluginData(string dllPath, string guid, Version version, string name, PluginLocation location, PluginDependency[] dependencies, bool pluginIsLoaded)
    {
        DllPath = dllPath;
        GUID = guid;
        Version = version;
        Name = name;
        Location = location;
        Dependencies = dependencies;
        PluginIsLoaded = pluginIsLoaded;
    }

    public static PluginData Create(string dllPath, BepInPlugin pluginAttribute, PluginLocation location, Type pluginClassType)
    {
        var dependencies = PluginUtils.GetDependencies(pluginClassType);
        return new PluginData(dllPath, pluginAttribute.GUID, new Version(pluginAttribute.Version), pluginAttribute.Name, location, dependencies, true);
    }

    public bool IsValid => !string.IsNullOrEmpty(DllPath) && File.Exists(DllPath);

    public bool Installed => Location == PluginLocation.Plugins;

    public bool IsNakedDLL => string.Equals(FileManagement.NormalizePath(ContainingFolder), FileManagement.NormalizePath(FileManagement.BepInExPluginsFolder));

    public bool GetCanBeToggled()
    {
        if (GUID == Plugin.GUID) return false; // this plugin shouldn't be able to be disabled by itself
        if (IsNakedDLL) return false; // we can't deal with these, that is the user's responsibility
        return true;
    }

    public bool Equals(PluginData other)
    {
        if (other == null) return false;
        return other.GUID == GUID;
    }

    public bool HasAllHardDependencies(List<PluginData> allPluginsToSearch)
    {
        if (Dependencies == null) return true;

        foreach (var dependency in Dependencies)
        {
            bool foundPlugin = HasDependency(allPluginsToSearch, dependency) == DependencyState.Installed;
            if (!foundPlugin && dependency.IsHard)
            {
                return false;
            }
        }
        return true;
    }

    public bool GetIsDuplicate(List<PluginData> allPluginsToSearch)
    {
        foreach (var plugin in allPluginsToSearch)
        {
            if (plugin.GUID == GUID && plugin != this) return true;
        }
        return false;
    }

    public DependencyState HasDependency(List<PluginData> allPluginsToSearch, PluginDependency dependencyToCheckFor)
    {
        foreach (var installedPlugin in allPluginsToSearch)
        {
            if (installedPlugin.GUID.Equals(dependencyToCheckFor.Guid))
            {
                if (!installedPlugin.Installed)
                {
                    return DependencyState.Disabled;
                }
                if (dependencyToCheckFor.VersionRequirement == null || dependencyToCheckFor.VersionRequirement <= installedPlugin.Version)
                {
                    return DependencyState.Installed;
                }
                return DependencyState.Outdated;
            }
        }
        return DependencyState.NotInstalled;
    }

    public bool SamePluginOrFileAsOther(PluginData other)
    {
        if (other == null) return false;
        return GUID == other.GUID || FileManagement.NormalizePath(DllPath) == FileManagement.NormalizePath(other.DllPath) || FileManagement.NormalizePath(ContainingFolder) == FileManagement.NormalizePath(other.ContainingFolder);
    }

    public bool HasLinkedModLimitations()
    {
        foreach (var plugin in KnownPlugins.list)
        {
            if (plugin != this && plugin.SamePluginOrFileAsOther(this))
            {
                return true;
            }
        }
        return false;
    }

    public PluginData[] GetLinkedMods()
    {
        int count = 0;
        foreach (var plugin in KnownPlugins.list)
        {
            if (plugin != this && plugin != null && plugin.SamePluginOrFileAsOther(this))
            {
                count++;
            }
        }
        PluginData[] array = new PluginData[count];
        int i = 0;
        foreach (var plugin in KnownPlugins.list)
        {
            if (plugin != this && plugin != null && plugin.SamePluginOrFileAsOther(this))
            {
                array[i] = plugin;
                i++;
            }
        }
        return array;
    }
}