using System.Collections;
using SubnauticaModManager.Web;

namespace SubnauticaModManager.Mono;

internal class CheckForUpdatesButton : MonoBehaviour
{
    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => OnClick());
    }

    private void OnClick()
    {
        StartCoroutine(Behaviour());
    }

    private IEnumerator Behaviour()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) yield break;
        if (LoadingProgress.Busy) yield break;
        var results = new List<SubmodicaSearchResult>();
        yield return SubmodicaAPI.SearchForUpdates(new LoadingProgress(), results, new GenericStatusReport());
        menu.downloadModsTab.ShowUpdateAvailableResults(results);
        menu.submodicaSearchBar.ClearInput();
    }
}
