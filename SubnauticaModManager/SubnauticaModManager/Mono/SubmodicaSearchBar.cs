using System.Collections;
using SubnauticaModManager.Web;

namespace SubnauticaModManager.Mono;

internal class SubmodicaSearchBar : MonoBehaviour
{
    private TMP_InputField inputField;

    public bool ReferencesSet { get; private set; }

    private void Awake()
    {
        if (!ReferencesSet) SetReferences();
    }

    public void SetReferences()
    {
        inputField = GetComponent<TMP_InputField>();
        inputField.onEndEdit = new TMP_InputField.SubmitEvent();
        inputField.onEndEdit.AddListener((string _) => BeginSearching());
        ReferencesSet = true;
    }

    public void BeginSearching()
    {
        StartCoroutine(Behaviour());
    }

    public void BeginSearchingMostRecent()
    {
        StartCoroutine(SearchMostRecent());
    }

    public void ClearInput()
    {
        inputField.text = string.Empty;
    }

    private IEnumerator Behaviour()
    {
        var inputFieldText = inputField.text;
        if (inputFieldText == null) inputFieldText = string.Empty;

        var menu = ModManagerMenu.main;
        if (menu == null) yield break;
        if (LoadingProgress.Busy) yield break;
        var searchResult = new SubmodicaSearchResult();
        yield return SubmodicaAPI.Search(inputFieldText, new LoadingProgress(), searchResult);
        menu.downloadModsTab.ShowModResults(searchResult);
    }

    private IEnumerator SearchMostRecent()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) yield break;
        if (LoadingProgress.Busy) yield break;
        var searchResult = new SubmodicaSearchResult();
        yield return SubmodicaAPI.SearchRecentlyUpdated(new LoadingProgress(), searchResult);
        menu.downloadModsTab.ShowModResults(searchResult);
        menu.submodicaSearchBar.ClearInput();
    }
}
