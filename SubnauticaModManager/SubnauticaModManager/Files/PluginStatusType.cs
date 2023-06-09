namespace SubnauticaModManager.Files;

public enum PluginStatusType
{
    CouldNotFind = -1,
    NoError = 0,
    MissingDependencies = 1,
    Duplicate = 2,
    FailedToLoad = 3
}
public static class PluginWarningTypeExtensions
{
    public static string FormatPluginStatus(this PluginStatusType type, bool enabled)
    {
        switch (type)
        {
            default: return enabled ? "Enabled" : "Disabled";
            case PluginStatusType.CouldNotFind: return "Mod manager error!";
            case PluginStatusType.MissingDependencies: return "Missing dependencies!";
            case PluginStatusType.Duplicate: return "Duplicate mod!";
            case PluginStatusType.FailedToLoad: return "Failed to load";
        }
    }
}