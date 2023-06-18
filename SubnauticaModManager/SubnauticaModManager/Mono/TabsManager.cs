namespace SubnauticaModManager.Mono;

internal class TabsManager : MonoBehaviour
{
    private Tab[] tabs;

    public Tab ActiveTab { get; private set; }

    private void Start()
    {
        tabs = gameObject.GetComponentsInChildren<Tab>(true);

        foreach (var tab in tabs) tab.OnCreate();

        SetTabActive(Tab.Type.Manage);
    }

    public Tab GetTab(Tab.Type type)
    {
        foreach (var tab in tabs)
            if (tab.type == type)
                return tab;
        return null;
    }

    public void SetTabActive(Tab.Type type)
    {
        LoadingProgress.CancelAll();
        ActiveTab = GetTab(type);
        foreach (Tab tab in tabs)
        {
            tab.SetActiveState(tab.type == type);
        }
    }
}