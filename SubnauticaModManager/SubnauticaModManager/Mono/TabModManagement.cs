using SubnauticaModManager.Files;
using System.Text;

namespace SubnauticaModManager.Mono;

internal class TabModManagement : Tab
{
    public PluginData currentData;

    private Transform buttonsParent;
    private GameObject manageArea;
    private GameObject manageAreaPlaceholder;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI versionText;
    private TextMeshProUGUI guidText;
    private TextMeshProUGUI dependencyText;
    private TextMeshProUGUI manualUninstallRequiredText;
    private Toggle enableToggle;
    private Button openFolderButton;
    private GameObject modManageButton;

    private bool updateToggleDirty;

    private static bool warnedForSMLHelperThisSession;

    public override void OnCreate()
    {
        buttonsParent = gameObject.SearchChild("Content").transform;
        manageArea = transform.Find("ManageArea").gameObject;
        manageAreaPlaceholder = transform.Find("ManageAreaPlaceholder").gameObject;

        titleText = gameObject.SearchChild("Title").GetComponent<TextMeshProUGUI>();
        versionText = gameObject.SearchChild("Version").GetComponent<TextMeshProUGUI>();
        guidText = gameObject.SearchChild("GUID").GetComponent<TextMeshProUGUI>();
        manualUninstallRequiredText = gameObject.SearchChild("ManualUninstallRequired").GetComponent<TextMeshProUGUI>();
        dependencyText = gameObject.SearchChild("DependencyText").GetComponent<TextMeshProUGUI>();
        enableToggle = gameObject.GetComponentInChildren<Toggle>();
        enableToggle.onValueChanged = new Toggle.ToggleEvent();
        enableToggle.onValueChanged.AddListener(OnToggleChanged);
        openFolderButton = gameObject.SearchChild("OpenFolder").GetComponent<Button>();
        openFolderButton.onClick = new Button.ButtonClickedEvent();
        openFolderButton.onClick.AddListener(OpenFolder);

        modManageButton = Plugin.assetBundle.LoadAsset<GameObject>("ManageTabButton");

        SetActiveMod(null);
    }

    private void OpenFolder()
    {
        if (currentData != null && currentData.IsValid)
        {
            Application.OpenURL(currentData.ContainingFolder);
            SoundUtils.PlaySound(UISound.Tweak);
        }
    }

    public override void OnActivate()
    {
        UpdateModList();
        CheckForImportantDependencies();
    }

    private void CheckForImportantDependencies()
    {
        if (warnedForSMLHelperThisSession || KnownPlugins.list == null) return;
        var menu = ModManagerMenu.main;
        if (menu == null) return;

        bool hasSMLHelper = false;
        foreach (var plugin in KnownPlugins.list)
        {
            if (plugin != null && ModEnablement.GetEnableState(plugin) && plugin.GUID == Web.ImportantModGUIDs.smlHelper)
            {
                hasSMLHelper = true;
                break;
            }
        }
        if (!hasSMLHelper)
        {
            menu.prompt.Ask(StringConstants.notice, "Due to how mods are loaded, some may not be listed here until their dependencies (e.g., SMLHelper or ECCLibrary) are installed.", new PromptChoice[] { new PromptChoice("I understand") });
            warnedForSMLHelperThisSession = true;
        }
    }

    private void UpdateModList()
    {
        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject);
        }
        KnownPlugins.list = PluginUtils.GetAllPluginData(true);
        foreach (var plugin in KnownPlugins.list)
        {
            var spawned = Instantiate(modManageButton);
            spawned.SetActive(true);
            spawned.GetComponent<RectTransform>().SetParent(buttonsParent, false);
            spawned.AddComponent<PluginButton>().SetData(plugin);
            Helpers.FixUIObjects(spawned);
        }
    }

    private void OnToggleChanged(bool state)
    {
        if (updateToggleDirty)
        {
            updateToggleDirty = false;
            return;
        }
        if (!VerifyIntegrity.IsIntact)
        {
            VerifyIntegrity.WarnNotIntact();
            return;
        }
        if (currentData == null) return;
        ModEnablement.SetModEnable(currentData, state);
        foreach (var button in buttonsParent.GetComponentsInChildren<PluginButton>())
        {
            if (button.data != null && button.data.SamePluginOrFileAsOther(currentData))
            {
                button.pluginSupposedToBeEnabled = state;
            }
        }
        SoundUtils.PlaySound(state ? UISound.Select : UISound.Select);
        CheckForImportantDependencies();
    }

    public void SetActiveMod(PluginData data)
    {
        if (data != null)
        {
            bool wasOn = enableToggle.isOn;
            var newOnState = ModEnablement.GetEnableState(data);
            if (newOnState != wasOn) updateToggleDirty = true;
            enableToggle.isOn = newOnState;
        }
        currentData = data;
        if (data == null || !data.IsValid)
        {
            manageArea.SetActive(false);
            manageAreaPlaceholder.SetActive(true);
            return;
        }
        else
        {
            manageArea.SetActive(true);
            manageAreaPlaceholder.SetActive(false);
        }
        bool canToggle = data.GetCanBeToggled();
        enableToggle.gameObject.SetActive(canToggle);
        manualUninstallRequiredText.gameObject.SetActive(!canToggle);
        titleText.text = data.Name;
        versionText.text = "v" + data.Version;
        guidText.text = "GUID: " + data.GUID;
        dependencyText.text = GetDependenciesText(data);
    }
    private string GetDependenciesText(PluginData plugin)
    {
        var sb = new StringBuilder();

        sb.AppendLine();

        bool listedSomething = false;
        if (plugin.HasLinkedModLimitations())
        {
            sb.AppendLine("<b><u>This mod is linked with the following mod(s):</u></b>");
            var linkedMods = plugin.GetLinkedMods();
            foreach (var linked in linkedMods)
            {
                sb.AppendLine("- " + linked.Name);
            }
            sb.AppendLine();
            listedSomething = true;
        }

        if (plugin.Dependencies != null && plugin.Dependencies.Length > 0 && KnownPlugins.list != null)
        {
            sb.AppendLine("<b><u>This mod depends on the following mods:</u></b>");
            foreach (var dependency in plugin.Dependencies)
            {
                FormatDependency(sb, plugin, dependency);
            }
            sb.AppendLine();
            listedSomething = true;
        }

        if (!listedSomething)
        {
            sb.AppendLine("No requirements listed");
        }

        return sb.ToString().TrimEnd('\n');
    }

    private void FormatDependency(StringBuilder sb, PluginData plugin, PluginDependency dependency)
    {
        var lastLoadedPluginData = KnownPlugins.list;

        bool shownDisplayName = dependency.TryGetDisplayName(lastLoadedPluginData, out var displayName);
        if (shownDisplayName)
        {
            sb.Append($"<b>{displayName}</b>");
        }
        else
        {
            sb.Append($"<b>{dependency.guid}</b>");
        }
        if (dependency.versionRequirement != null && dependency.versionRequirement > new System.Version(0, 0))
        {
            sb.AppendLine(" v" + dependency.versionRequirement);
        }
        else
        {
            sb.AppendLine();
        }

        if (shownDisplayName)
        {
            sb.AppendLine($"({dependency.guid})");
        }

        var optional = !dependency.IsHard;

        if (optional)
        {
            sb.AppendLine("Optional");
        }

        var dependencyState = plugin.HasDependency(lastLoadedPluginData, dependency);
        switch (dependencyState)
        {
            default:
                sb.AppendLine("Unknown");
                break;
            case DependencyState.Installed:
                sb.AppendLine("Installed");
                break;
            case DependencyState.NotInstalled:
                if (optional)
                    sb.AppendLine("Not found");
                else
                    sb.AppendLine("<color=#FF0000>Not found</color>");
                break;
            case DependencyState.Disabled:
                if (optional)
                    sb.AppendLine("Not enabled");
                else
                    sb.AppendLine("<color=#FF0000>Not enabled</color>");
                break;
            case DependencyState.Outdated:
                sb.AppendLine("<color=#FF0000>Update required!</color>");
                break;
        }
    }
}
