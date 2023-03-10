using System;

namespace SubnauticaModManager.Files;

internal class PluginData
{
    public string dllPath;

    public string ContainingFolder => Path.GetFullPath(Path.Combine(dllPath, @"..\"));

    public string GUID;
    public Version Version;
    public string Name;
    public readonly PluginLocation Location;
    public PluginDependency[] Dependencies;

    public PluginData(string dllPath, string gUID, Version version, string name, PluginLocation location, PluginDependency[] dependencies)
    {
        this.dllPath = dllPath;
        GUID = gUID;
        Version = version;
        Name = name;
        Location = location;
        Dependencies = dependencies;
    }

    public bool IsValid => !string.IsNullOrEmpty(dllPath) && File.Exists(dllPath);

    public bool Installed => Location == PluginLocation.Plugins;

    public bool IsNakedDLL => string.Equals(FileManagement.NormalizePath(ContainingFolder), FileManagement.NormalizePath(FileManagement.BepInExPluginsFolder));

    public bool GetCanBeToggled()
    {
        if (GUID == Plugin.GUID) return false; // this plugin shouldn't be able to be disabled by itself
        if (IsNakedDLL) return false; // we can't deal with these, that is the user's responsibility
        return true;
    }

    public string StatusText
    {
        get
        {
            if (Location == PluginLocation.Plugins)
            {
                return "Enabled";
            }
            return "Disabled";
        }
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

    public PluginStatusType GetWarningType(List<PluginData> allPluginsToSearch)
    {
        if (allPluginsToSearch == null) return PluginStatusType.NoError;
        if (GetIsDuplicate(allPluginsToSearch)) return PluginStatusType.Duplicate;
        if (!HasAllHardDependencies(allPluginsToSearch)) return PluginStatusType.MissingDependencies;
        return PluginStatusType.NoError;
    }

    public DependencyState HasDependency(List<PluginData> allPluginsToSearch, PluginDependency dependencyToCheckFor)
    {
        foreach (var installedPlugin in allPluginsToSearch)
        {
            if (installedPlugin.GUID.Equals(dependencyToCheckFor.guid))
            {
                if (!installedPlugin.Installed)
                {
                    return DependencyState.Disabled;
                }
                if (dependencyToCheckFor.versionRequirement == null || dependencyToCheckFor.versionRequirement <= installedPlugin.Version)
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
        return GUID == other.GUID || FileManagement.NormalizePath(dllPath) == FileManagement.NormalizePath(other.dllPath) || FileManagement.NormalizePath(ContainingFolder) == FileManagement.NormalizePath(other.ContainingFolder);
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