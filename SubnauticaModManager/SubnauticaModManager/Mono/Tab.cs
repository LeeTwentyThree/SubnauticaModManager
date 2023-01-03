namespace SubnauticaModManager.Mono;

internal class Tab : MonoBehaviour
{
    public Type type;

    internal enum Type
    {
        News,
        Install,
        Manage,
        Download
    }

    public virtual void OnCreate() { }

    public virtual void OnActivate() { }
}