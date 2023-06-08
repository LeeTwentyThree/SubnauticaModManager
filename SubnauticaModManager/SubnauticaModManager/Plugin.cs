namespace SubnauticaModManager;

using System.Reflection;

[BepInPlugin(GUID, Name, Version)]
public class Plugin : BaseUnityPlugin
{
    public const string Version = "1.0.9";
    public const string GUID = "com.lee23.subnauticamodmanager";
    public const string Name = "SubnauticaModManager for BepInEx";

    private static readonly Harmony harmony = new Harmony(GUID);

    new internal static ManualLogSource Logger { get; private set; }

    internal static Assembly assembly = Assembly.GetExecutingAssembly();

    internal static AssetBundle assetBundle;

    private void Awake()
    {
        Logger = base.Logger;
        harmony.PatchAll(assembly);
        assetBundle = AssetBundle.LoadFromFile(FileManagement.FromAssetsFolder("subnauticamodmanager"));

        Files.Cleanup.CleanupCache();
    }
}