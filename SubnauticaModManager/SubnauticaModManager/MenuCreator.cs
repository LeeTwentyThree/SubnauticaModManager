namespace SubnauticaModManager;

using Mono;
using Files;

internal static class MenuCreator
{
    public static ModManagerMenu MenuInstance { get; private set; }

    public static bool MenuExists => MenuInstance != null;

    public static void ShowMenu()
    {
        if (MenuExists) return;
        MenuInstance = InstantiateMenu();
        SetModMenuShownState(true);
        if (!VerifyIntegrity.IsIntact)
        {
            VerifyIntegrity.WarnNotIntact();
        }
    }

    private static ModManagerMenu InstantiateMenu()
    {
        // setup gameobject
        var menuObject = Object.Instantiate(Plugin.assetBundle.LoadAsset<GameObject>("ModManagerCanvas"));
        var menuComponent = menuObject.AddComponent<ModManagerMenu>();

        // fix existing components
        Helpers.FixUIObjects(menuObject);
        Object.DestroyImmediate(menuObject.GetComponent<GraphicRaycaster>());
        menuObject.EnsureComponent<uGUI_GraphicRaycaster>();
        Object.DestroyImmediate(menuObject.GetComponent<CanvasScaler>());
        var scaler = menuObject.EnsureComponent<uGUI_CanvasScaler>();
        scaler.vrMode = uGUI_CanvasScaler.Mode.World;

        // add essential components
        menuComponent.mainHeader = menuObject.SearchChild("MainHeader").AddComponent<MainHeader>();
        menuComponent.modsInstalledText = menuObject.SearchChild("ModsInstalledText").AddComponent<ModsInstalledText>();
        menuComponent.closeButton = menuObject.SearchChild("CloseButton").AddComponent<CloseButton>();
        menuComponent.quitGameButton = menuObject.SearchChild("QuitGameButton").AddComponent<QuitGameButton>();
        menuComponent.restartRequiredText = menuObject.SearchChild("RestartRequiredText").AddComponent<RestartRequiredText>();
        menuComponent.footer = menuObject.SearchChild("Footer").AddComponent<Footer>();
        menuComponent.prompt = menuObject.SearchChild("Prompt").AddComponent<PromptMenu>();
        menuComponent.loadingPrompt = menuObject.SearchChild("LoadingPrompt").AddComponent<LoadingPrompt>();
        menuComponent.reportIssueButton = menuObject.SearchChild("ReportIssueButton").AddComponent<ReportIssueButton>();

        menuComponent.gameObject.SearchChild("CheckForUpdates").AddComponent<CheckForUpdatesButton>();

        menuComponent.gameObject.AddComponent<CloseModManagerOnEscape>();

        // tab buttons
        menuObject.SearchChild("NewsTabButton").AddComponent<TabButton>().tabType = Tab.Type.News;
        menuObject.SearchChild("InstallTabButton").AddComponent<TabButton>().tabType = Tab.Type.Install;
        menuObject.SearchChild("ManageTabButton").AddComponent<TabButton>().tabType = Tab.Type.Manage;
        menuObject.SearchChild("DownloadTabButton").AddComponent<TabButton>().tabType = Tab.Type.Download;

        // tabs
        menuComponent.tabManager = menuObject.SearchChild("TabRoot").AddComponent<TabsManager>();

        menuComponent.newsTab = menuObject.SearchChild("NewsTab").AddComponent<TabNews>();
        menuComponent.newsTab.type = Tab.Type.News;
        menuComponent.installModsTab = menuObject.SearchChild("InstallTab").AddComponent<TabInstallMods>();
        menuComponent.installModsTab.type = Tab.Type.Install;
        menuComponent.modManagerTab = menuObject.SearchChild("ManageTab").AddComponent<TabModManagement>();
        menuComponent.modManagerTab.type = Tab.Type.Manage;
        menuComponent.downloadModsTab = menuObject.SearchChild("DownloadTab").AddComponent<TabDownloadMods>();
        menuComponent.downloadModsTab.type = Tab.Type.Download;

        // mod manager tab

        menuComponent.filterModsInputField = menuComponent.modManagerTab.gameObject.SearchChild("ModFilterInput").AddComponent<FilterModsInputField>();

        // mod browser tab
        menuComponent.submodicaSearchBar = menuComponent.downloadModsTab.gameObject.SearchChild("SearchBar").AddComponent<SubmodicaSearchBar>();
        menuComponent.downloadModsTab.gameObject.SearchChild("SearchButton").AddComponent<SubmodicaSearchButton>();
        menuComponent.downloadModsTab.gameObject.SearchChild("MostPopular").AddComponent<LoadMostPopularButton>();
        menuComponent.downloadModsTab.gameObject.SearchChild("RecentlyUpdated").AddComponent<LoadMostRecentButton>();

        // mod installation tab
        menuComponent.installModsTab.gameObject.SearchChild("ModDownloadsFolderButton").AddComponent<OpenModDownloadsFolderButton>();
        menuComponent.installModsTab.gameObject.SearchChild("InstallModsButton").AddComponent<InstallAllModsButton>();

        // info panel
        menuComponent.infoPanel = menuObject.SearchChild("InfoPanel");
        menuObject.SearchChild("InfoButton").AddComponent<ShowInfoButton>();
        menuObject.SearchChild("CloseInfoButton").AddComponent<CloseInfoButton>();

        return menuComponent;
    }

    public static void HideMenu()
    {
        if (!MenuExists) return;
        MenuInstance.Hide();
        SetModMenuShownState(false);
        SoundUtils.PlaySound(UISound.Select);
        LoadingProgress.CancelAll();
    }

    private static void SetModMenuShownState(bool shown)
    {
        SetMainMenuShown(!shown);
    }

    private static void SetMainMenuShown(bool shown)
    {
        var mainMenu = uGUI_MainMenu.main ?? Object.FindObjectOfType<uGUI_MainMenu>();
        if (mainMenu == null) return;
        mainMenu.gameObject.SetActive(shown);
    }
}