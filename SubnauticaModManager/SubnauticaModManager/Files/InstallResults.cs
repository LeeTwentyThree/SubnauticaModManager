namespace SubnauticaModManager.Files;

public class InstallResults
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
}
public enum InstallResultType
{
    Success,
    Update,
    Failure
}