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

        var checkForUpdatesButton = menuComponent.gameObject.SearchChild("CheckForUpdates").AddComponent<CheckForUpdatesButton>();
        menuComponent.gameObject.SearchChild("GetModListButton").AddComponent<GetModListButton>();
        menuComponent.gameObject.AddComponent<CloseModManagerOnEscape>();

        TranslatableText.Create(menuComponent.closeButton.gameObject, "Close");
        TranslatableText.Create(menuComponent.restartRequiredText.gameObject, "RestartRequired");
        TranslatableText.Create(menuComponent.reportIssueButton.gameObject, "ReportIssue");
        TranslatableText.Create(menuComponent.quitGameButton.gameObject, "ApplyChangesButton");
        TranslatableText.Create(checkForUpdatesButton.gameObject, "CheckForUpdates");

        // tab buttons

        var newsTab = menuObject.SearchChild("NewsTabButton").AddComponent<TabButton>();
        newsTab.tabType = Tab.Type.News;
        var installTab = menuObject.SearchChild("InstallTabButton").AddComponent<TabButton>();
        installTab.tabType = Tab.Type.Install;
        var manageTab = menuObject.SearchChild("ManageTabButton").AddComponent<TabButton>();
        manageTab.tabType = Tab.Type.Manage;
        var downloadTab = menuObject.SearchChild("DownloadTabButton").AddComponent<TabButton>();
        downloadTab.tabType = Tab.Type.Download;

        TranslatableText.Create(newsTab.gameObject, "Tab_News");
        TranslatableText.Create(installTab.gameObject, "Tab_Install");
        TranslatableText.Create(manageTab.gameObject, "Tab_Manage");
        TranslatableText.Create(downloadTab.gameObject, "Tab_Browse");

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
        var enableAll = menuComponent.modManagerTab.gameObject.SearchChild("EnableAll").AddComponent<ToggleAllButton>();
        enableAll.targetEnabledState = true;
        var disableAll = menuComponent.modManagerTab.gameObject.SearchChild("DisableAll").AddComponent<ToggleAllButton>();
        disableAll.targetEnabledState = false;

        TranslatableText.Create(menuComponent.filterModsInputField.gameObject.SearchChild("Placeholder"), "FilterMods");
        TranslatableText.Create(enableAll.gameObject, "EnableAll");
        TranslatableText.Create(disableAll.gameObject, "DisableAll");
        TranslatableText.Create(menuComponent.modManagerTab.gameObject.transform.GetChild(1).gameObject.SearchChild("DependenciesTitle"), "Dependencies");
        TranslatableText.Create(menuComponent.modManagerTab.gameObject.transform.GetChild(1).gameObject.SearchChild("OpenFolder"), "OpenInFileExplorer");
        TranslatableText.Create(menuComponent.modManagerTab.gameObject.transform.GetChild(1).gameObject.SearchChild("Toggle"), "EnableModToggle");
        TranslatableText.Create(menuComponent.modManagerTab.gameObject.transform.GetChild(2).gameObject, "NoModSelected");
        TranslatableText.Create(menuComponent.modManagerTab.gameObject.transform.GetChild(1).GetChild(1).gameObject, "MustBeUninstalledManually");
        TranslatableText.Create(menuComponent.modManagerTab.gameObject.SearchChild("LoadingModsText"), "LoadingMods");

        // mod browser tab

        menuComponent.submodicaSearchBar = menuComponent.downloadModsTab.gameObject.SearchChild("SearchBar").AddComponent<SubmodicaSearchBar>();
        menuComponent.downloadModsTab.gameObject.SearchChild("SearchButton").AddComponent<SubmodicaSearchButton>();
        var loadMostPopularButton = menuComponent.downloadModsTab.gameObject.SearchChild("MostPopular").AddComponent<LoadMostPopularButton>();
        var loadMostRecentButton = menuComponent.downloadModsTab.gameObject.SearchChild("RecentlyUpdated").AddComponent<LoadMostRecentButton>();

        TranslatableText.Create(loadMostPopularButton.gameObject, "MostPopular");
        TranslatableText.Create(loadMostRecentButton.gameObject, "RecentlyUpdated");
        TranslatableText.Create(menuComponent.submodicaSearchBar.gameObject.SearchChild("Placeholder"), "SearchSubmodica");

        // mod installation tab

        menuComponent.installModsTab.gameObject.SearchChild("ModDownloadsFolderButton").AddComponent<OpenModDownloadsFolderButton>();
        menuComponent.installModsTab.gameObject.SearchChild("InstallModsButton").AddComponent<InstallAllModsButton>();

        TranslatableText.Create(menuComponent.installModsTab.transform.GetChild(0).gameObject, "InstallationInstructions");
        TranslatableText.Create(menuComponent.installModsTab.gameObject.SearchChild("ModDownloadsFolderButton"), "OpenModsDownloadFolder");

        // info panel

        menuComponent.infoPanel = menuObject.SearchChild("InfoPanel");
        var showInfoButton = menuObject.SearchChild("InfoButton").AddComponent<ShowInfoButton>();
        menuObject.SearchChild("CloseInfoButton").AddComponent<CloseInfoButton>();

        TranslatableText.Create(showInfoButton.gameObject, "AboutButtonSymbol");
        TranslatableText.Create(menuComponent.infoPanel.transform.GetChild(0).gameObject, "About");

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