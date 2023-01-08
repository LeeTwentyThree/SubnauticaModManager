using DependencyFlags = BepInEx.BepInDependency.DependencyFlags;
using System;

namespace SubnauticaModManager.Files;

internal class PluginDependency
{
    public string guid;
    public DependencyFlags flags;
    public Version versionRequirement;

    public bool IsSoft => flags.HasFlag(DependencyFlags.SoftDependency);

    public bool IsHard => flags.HasFlag(DependencyFlags.HardDependency);

    private string _knownDisplayName;

    public PluginDependency(string guid, DependencyFlags flags, Version versionRequirement)
    {
        this.guid = guid;
        this.flags = flags;
        this.versionRequirement = versionRequirement;
    }

    public string GetDisplayNameOrDefault(List<PluginData> knownPlugins)
    {
        if (TryGetDisplayName(knownPlugins, out var name))
        {
            return name;
        }
        return guid;
    }

    public bool TryGetDisplayName(List<PluginData> knownPlugins, out string displayName)
    {
        if (_knownDisplayName != null)
        {
            displayName = _knownDisplayName;
            return true;
        }
        if (knownPlugins != null)
        {
            foreach (var plugin in knownPlugins)
            {
                if (plugin.GUID.Equals(guid))
                {
                    _knownDisplayName = plugin.Name;
                    displayName = _knownDisplayName;
                    return true;
                }
            }
        }
        _knownDisplayName = string.Empty;
        displayName = null;
        return false;
    }
}