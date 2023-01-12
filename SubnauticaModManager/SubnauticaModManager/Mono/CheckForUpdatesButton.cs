using System.Collections;
using SubnauticaModManager.Web;

namespace SubnauticaModManager.Mono;

internal class CheckForUpdatesButton : MonoBehaviour
{
    private float timeCanUseAgain;
    private const float delay = 5f;

    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => OnClick());
    }

    private void OnClick()
    {
        if (LoadingProgress.Busy) return;
        if (Files.ModArrangement.WaitingOnRestart)
        {
            Files.ModArrangement.WarnPossibleConflict();
            return;
        }
        ModManagerMenu.main.tabManager.SetTabActive(Tab.Type.Download);
        ModManagerMenu.main.prompt.Ask(StringConstants.notice, "Some mods that are up to date may still appear in this list. Please let the authors of these mods know if that occurs.", new PromptChoice[] { new PromptChoice("I understand", Do) });
    }

    private void Do()
    {
        StartCoroutine(Behaviour());
    }

    private IEnumerator Behaviour()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) yield break;
        if (LoadingProgress.Busy) yield break;
        if (Time.time < timeCanUseAgain)
        {
            ModManagerMenu.main.prompt.Ask(StringConstants.notice, "Please wait a few seconds before doing this again.", new PromptChoice("Close"));
            yield break;
        }
        var results = new List<SubmodicaSearchResult>();
        yield return SubmodicaAPI.SearchForUpdates(new LoadingProgress(), results, new GenericStatusReport());
        menu.downloadModsTab.ShowUpdateAvailableResults(results);
        menu.submodicaSearchBar.ClearInput();
        timeCanUseAgain = Time.time + delay;
    }
}
