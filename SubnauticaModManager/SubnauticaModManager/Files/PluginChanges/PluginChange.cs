namespace SubnauticaModManager.Files;

public abstract class PluginChange
{
    public string pluginGUID;

    public abstract void ApplyChange();
}
