﻿using System;

namespace SubnauticaModManager.Files;

internal class PluginData
{
    public string dllPath;

    public string ContainingFolder => Path.Combine(dllPath, "../");

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
            bool foundPlugin = HasDependency(allPluginsToSearch, dependency);
            if (!foundPlugin && dependency.IsHard)
            {
                return false;
            }
        }
        return true;
    }

    public bool HasDependency(List<PluginData> allPluginsToSearch, PluginDependency dependencyToCheckFor)
    {
        foreach (var installedPlugin in allPluginsToSearch)
        {
            if (installedPlugin.Installed && installedPlugin.GUID.Equals(dependencyToCheckFor.guid))
            {
                if (dependencyToCheckFor.versionRequirement == null || dependencyToCheckFor.versionRequirement <= installedPlugin.Version)
                {
                    return true;
                }
            }
        }
        return false;
    }
}
