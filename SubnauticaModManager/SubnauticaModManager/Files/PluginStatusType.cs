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
        return Translation.Translate(FormatPluginStatusRaw(type, enabled));
    }

    public static string FormatPluginStatusRaw(this PluginStatusType type, bool enabled)
    {
        switch (type)
        {
            default: return enabled ? "PluginStatus_Enabled" : "PluginStatus_Disabled";
            case PluginStatusType.CouldNotFind: return "PluginStatus_CouldNotFind";
            case PluginStatusType.MissingDependencies: return "PluginStatus_MissingDependencies";
            case PluginStatusType.Duplicate: return "PluginStatus_Duplicate";
            case PluginStatusType.FailedToLoad: return "PluginStatus_FailedToLoad";
        }
    }
}