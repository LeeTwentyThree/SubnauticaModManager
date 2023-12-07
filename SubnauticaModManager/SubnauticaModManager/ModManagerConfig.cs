using BepInEx;
using BepInEx.Configuration;

namespace SubnauticaModManager;

internal static class ModManagerConfig
{
    public static ConfigEntry<Color> NormalModButtonColor { get; private set; }
    public static ConfigEntry<Color> UninstalledModButtonColor { get; private set; }

    public static void RegisterConfig(ConfigFile config)
    {
        NormalModButtonColor = config.Bind("Interface settings",
            "Installed mod button color",
            new Color(1, 1, 1));

        UninstalledModButtonColor = config.Bind("Interface settings",
            "Uninstalled mod button color",
            new Color(1, 1, 1, 0.3f));
    }
}
