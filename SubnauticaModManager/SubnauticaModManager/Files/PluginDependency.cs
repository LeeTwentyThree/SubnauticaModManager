using DependencyFlags = BepInEx.BepInDependency.DependencyFlags;
using System;

namespace SubnauticaModManager.Files;

internal class PluginDependency
{
    public string guid;
    public DependencyFlags flags;
    public SemVersion versionRequirement;

    public bool IsSoft => flags.HasFlag(DependencyFlags.SoftDependency);

    public bool IsHard => flags.HasFlag(DependencyFlags.HardDependency);

    private string _knownDisplayName;

    private bool _resolvedDisplayName;

    public PluginDependency(string guid, DependencyFlags flags, SemVersion versionRequirement)
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
        if (_resolvedDisplayName)
        {
            if (string.IsNullOrEmpty(_knownDisplayName))
            {
                displayName = null;
                return false;
            }
            else
            {
                displayName = _knownDisplayName;
                return true;
            }
        }
        if (knownPlugins != null)
        {
            foreach (var plugin in knownPlugins)
            {
                if (plugin.GUID.Equals(guid))
                {
                    _knownDisplayName = plugin.Name;
                    displayName = _knownDisplayName;
                    _resolvedDisplayName = true;
                    return true;
                }
            }
        }
        displayName = null;
        _resolvedDisplayName = true;
        return false;
    }
}