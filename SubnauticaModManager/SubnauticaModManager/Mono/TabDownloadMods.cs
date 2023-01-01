using SubnauticaModManager.Web;

namespace SubnauticaModManager.Mono;

internal class TabDownloadMods : Tab
{
    private RectTransform modButtonsParent;

    private GameObject buttonPrefab;

    private void Awake()
    {
        modButtonsParent = transform.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();
        buttonPrefab = transform.Find("SubmodicaButtonReference").gameObject;
    }

    public void ShowModResults(SubmodicaSearchResult searchResult)
    {
        ClearList();
        foreach (var mod in searchResult.Mods)
        {
            if (mod != null)
            {
                AddModButton(mod);
            }
            else
            {
                Plugin.Logger.LogWarning("Trying to display null mod");
            }
        }
    }

    private void ClearList()
    {
        foreach (Transform child in modButtonsParent)
        {
            Destroy(child.gameObject);
        }
    }

    private SubmodicaModButton AddModButton(SubmodicaMod mod)
    {
        var go = Instantiate(buttonPrefab);
        go.SetActive(true);
        go.GetComponent<RectTransform>().SetParent(modButtonsParent, false);
        var button = go.AddComponent<SubmodicaModButton>();
        button.SetCurrentModData(mod);
        return button;
    }

    public override void OnActivate()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) return;
        if (!menu.submodicaSearchBar.ReferencesSet) menu.submodicaSearchBar.SetReferences();
        menu.submodicaSearchBar.inputField.text = string.Empty;
        if (modButtonsParent != null && modButtonsParent.childCount == 0)
        {
            menu.submodicaSearchBar.BeginSearching();
        }
    }
}
