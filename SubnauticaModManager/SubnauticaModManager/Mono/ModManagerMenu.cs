namespace SubnauticaModManager.Mono;

internal class ModManagerMenu : MonoBehaviour
{
    public static ModManagerMenu main;

    public MainHeader mainHeader;
    public CloseButton closeButton;
    public QuitGameButton quitGameButton;
    public RestartRequiredText restartRequiredText;
    public ReportIssueButton reportIssueButton;
    public Footer footer;
    public TabsManager tabManager;
    public PromptMenu prompt;
    public LoadingPrompt loadingPrompt;
    public TabNews newsTab;
    public TabInstallMods installModsTab;
    public TabModManagement modManagerTab;
    public TabDownloadMods downloadModsTab;
    public SubmodicaSearchBar submodicaSearchBar;

    public bool UnappliedChanges { get; private set; }

    public void NotifyUnsavedChanges() => UnappliedChanges = true;

    public void MarkAllChangesSaved() => UnappliedChanges = false;

    private void Awake()
    {
        main = this;
    }

    public void Hide()
    {
        Destroy(gameObject);
    }
}
