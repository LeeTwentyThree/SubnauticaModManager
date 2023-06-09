namespace SubnauticaModManager.Files;

internal class InstallResults
{
    public int Successes { get; private set; }
    public int Updates { get; private set; }
    public int Failures { get; private set; }
    public bool AttemptedToInstallSelf { get; private set; }

    public InstallResults()
    {
    }

    public InstallResults(int successes, int updates, int failures)
    {
        Successes = successes;
        Updates = updates;
        Failures = failures;
    }

    public void AddOne(InstallResultType result)
    {
        switch (result)
        {
            default: return;
            case InstallResultType.Success:
                Successes++;
                return;
            case InstallResultType.Update:
                Updates++;
                return;
            case InstallResultType.Failure:
                Failures++;
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

    public void WarnForAttemptToInstallSelf()
    {
        AttemptedToInstallSelf = true;
    }
}
public enum InstallResultType
{
    Success,
    Update,
    Failure
}