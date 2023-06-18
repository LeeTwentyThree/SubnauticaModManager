namespace SubnauticaModManager.Mono;

internal class Tab : MonoBehaviour
{
    public Type type;

    private bool _hasActivatedAtLeastOnce;

    internal enum Type
    {
        News,
        Install,
        Manage,
        Download
    }

    public virtual void OnCreate() { }

    protected virtual void OnActivate() { }

    protected virtual void OnDeactivate() { }

    public void SetActiveState(bool active)
    {
        gameObject.SetActive(active);
        if (active)
        {
            OnActivate();
            _hasActivatedAtLeastOnce = true;
        }
        else if (_hasActivatedAtLeastOnce)
        {
            OnDeactivate();
        }
    }
}