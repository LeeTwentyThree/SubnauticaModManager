using BepInEx;
using BepInEx.Configuration;

namespace SubnauticaModManager;

internal static class ModManagerConfig
{
    public static ConfigEntry<Color> UninstalledModColor { get; private set; }

    public static void RegisterConfig(ConfigFile config)
    {
        UninstalledModColor = config.Bind("Subnautica Mod Manager",
            "Uninstalled mod color",
            new Color(1, 1, 1, 0.3f));
    }
}
