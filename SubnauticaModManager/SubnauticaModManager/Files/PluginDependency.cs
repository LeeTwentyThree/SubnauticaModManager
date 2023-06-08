using DependencyFlags = BepInEx.BepInDependency.DependencyFlags;
using System;

namespace SubnauticaModManager.Files;

internal class PluginDependency
{
    public string Guid { get; }
    public DependencyFlags Flags { get; }
    public Version VersionRequirement { get; }

    public bool IsSoft => Flags.HasFlag(DependencyFlags.SoftDependency);

    public bool IsHard => Flags.HasFlag(DependencyFlags.HardDependency);

    private string _knownDisplayName;

    private bool _resolvedDisplayName;

    public PluginDependency(string guid, DependencyFlags flags, Version versionRequirement)
    {
        this.Guid = guid;
        this.Flags = flags;
        this.VersionRequirement = versionRequirement;
    }

    public string GetDisplayNameOrDefault(List<PluginData> knownPlugins)
    {
        if (TryGetDisplayName(knownPlugins, out var name))
        {
            return name;
        }
        return Guid;
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
                if (plugin.GUID.Equals(Guid))
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