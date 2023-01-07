namespace SubnauticaModManager.Files;

internal class InstallResults
{
    public int successes;
    public int updates;
    public int failures;

    public InstallResults()
    {
    }

    public InstallResults(int successes, int updates, int failures)
    {
        this.successes = successes;
        this.updates = updates;
        this.failures = failures;
    }

    public void AddOne(InstallResultType result)
    {
        switch (result)
        {
            default: return;
            case InstallResultType.Success:
                successes++;
                return;
            case InstallResultType.Update:
                updates++;
                return;
            case InstallResultType.Failure:
                failures++;
                return;
        }
    }

    public static string FormatResult(PluginData plugin, InstallResultType type)
    {
        switch (type)
        {
            default: return null;
            case InstallResultType.Success: return $"Successfully installed {plugin.Name} ({plugin.GUID}).";
            case InstallResultType.Update: return $"Successfully updated {plugin.Name} ({plugin.GUID}) to v{plugin.Version}.";
            case InstallResultType.Failure: return $"Failed to update {plugin.Name} ({plugin.GUID}).";
        }
    }
}
public enum InstallResultType
{
    Success,
    Update,
    Failure
}