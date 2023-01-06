using SubnauticaModManager.Web;

namespace SubnauticaModManager.Mono;

internal class TabDownloadMods : Tab
{
    private RectTransform modButtonsParent;

    private ScrollRect scrollRect;

    private GameObject buttonPrefab;

    private void Awake()
    {
        modButtonsParent = transform.Find("Scroll View/Viewport/Content").GetComponent<RectTransform>();
        buttonPrefab = transform.Find("SubmodicaButtonReference").gameObject;
        scrollRect = transform.Find("Scroll View").GetComponent<ScrollRect>();
    }

    public void ShowModResults(SubmodicaSearchResult searchResult)
    {
        if (searchResult.Mods == null || searchResult.Mods.Length == 0)
        {
            return;
        }
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
        scrollRect.verticalNormalizedPosition = 0;
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
        menu.submodicaSearchBar.ClearInput();
        if (modButtonsParent != null && modButtonsParent.childCount == 0)
        {
            menu.submodicaSearchBar.BeginSearching();
        }
    }
}
