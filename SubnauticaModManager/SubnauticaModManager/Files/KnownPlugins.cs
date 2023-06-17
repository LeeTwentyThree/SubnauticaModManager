namespace SubnauticaModManager.Files;

internal class KnownPlugins
{
    public static List<PluginData> list = new List<PluginData>();

    public static PluginData GetByGUID(string guid)
    {
        if (list == null) return null;
        foreach (var plugin in list)
        {
            if (plugin.GUID != null && plugin.GUID.Equals(guid)) return plugin;
        }
        return null;
    }

    public static bool GetUpdateAvailable(string guid, Version latest)
    {
        var plugin = GetByGUID(guid);
        if (plugin == null || !plugin.IsValid) return false;
        return latest > plugin.Version;
    }
}
