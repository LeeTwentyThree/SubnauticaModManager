using DependencyFlags = BepInEx.BepInDependency.DependencyFlags;
using System;

namespace SubnauticaModManager.Files;

public class PluginDependency
{
    public string guid;
    public DependencyFlags flags;
    public Version versionRequirement;

    public bool IsSoft => flags.HasFlag(DependencyFlags.SoftDependency);

    public bool IsHard => flags.HasFlag(DependencyFlags.HardDependency);

    public PluginDependency(string guid, DependencyFlags flags, Version versionRequirement)
    {
        this.guid = guid;
        this.flags = flags;
        this.versionRequirement = versionRequirement;
    }
}
