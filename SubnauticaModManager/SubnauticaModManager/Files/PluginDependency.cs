using DependencyFlags = BepInEx.BepInDependency.DependencyFlags;

namespace SubnauticaModManager.Files;

public class PluginDependency
{
    public string guid;
    public DependencyFlags flags;

    public bool IsSoft => flags.HasFlag(DependencyFlags.SoftDependency);

    public bool IsHard => flags.HasFlag(DependencyFlags.HardDependency);
}
