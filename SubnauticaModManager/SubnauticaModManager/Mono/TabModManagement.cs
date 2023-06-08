using SubnauticaModManager.Files;
using System.Text;
using System.Text.RegularExpressions;

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

    private List<PluginButton> pluginButtons = new List<PluginButton>();

    private bool updateToggleDirty;
    private string rawFilterString;

    private static bool warnedForMissingCoreLibraryThisSession;

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
        if (warnedForMissingCoreLibraryThisSession || KnownPlugins.list == null) return;
        var menu = ModManagerMenu.main;
        if (menu == null) return;

        bool hasSMLHelper = false;
        bool hasNautilus = false;
        foreach (var plugin in KnownPlugins.list)
        {
            if (plugin == null || !ModEnablement.GetEnableState(plugin))
                continue;

            if (plugin.GUID == Web.ImportantModGUIDs.smlHelper)
                hasSMLHelper = true;

            if (plugin.GUID == Web.ImportantModGUIDs.nautilus)
                hasNautilus = true;
        }

        if (!hasSMLHelper || !hasNautilus)
        {
            string notice = "Due to how mods are loaded, some may not be listed here until their dependencies (e.g., Nautilus, SMLHelper, or ECC Library) are installed.";
            menu.prompt.Ask(StringConstants.notice, notice, new PromptChoice[] { new PromptChoice("I understand") });
            warnedForMissingCoreLibraryThisSession = true;
        }
    }

    private void UpdateModList()
    {
        pluginButtons = new List<PluginButton>();
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
            var button = spawned.AddComponent<PluginButton>();
            button.SetData(plugin);
            Helpers.FixUIObjects(spawned);
            pluginButtons.Add(button);
        }
        FilterMods(ModManagerMenu.main.filterModsInputField.CurrentText);
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

        bool listedSomething = false;
        if (plugin.IsNakedDLL)
        {
            sb.AppendLine("<color=#FF0000><b>This mod is directly in the plugins folder and does not have a specific folder of its own. To manage it, please place the DLL in a new folder or install it with the mod manager.</color></b>");
            sb.AppendLine();
            listedSomething = true;
        }
        else if (plugin.HasLinkedModLimitations())
        {
            sb.AppendLine("<b>This mod is linked with the following mod(s):</b>");
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
            sb.AppendLine("<b>This mod depends on the following mods:</b>");
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

    public void FilterMods(string input)
    {
        if (rawFilterString != input)
        {
            rawFilterString = input;
            RefreshFilter();
        }
    }

    public void ToggleAll(bool newState)
    {
        if (!VerifyIntegrity.IsIntact)
        {
            VerifyIntegrity.WarnNotIntact();
            return;
        }
        foreach (var button in pluginButtons)
        {
            if (button.data != null && button.data.GUID != Plugin.GUID)
            {
                button.pluginSupposedToBeEnabled = newState;
                ModEnablement.SetModEnable(button.data, newState);
            }
        }
        if (currentData != null && currentData.GUID != Plugin.GUID)
        {
            updateToggleDirty = true;
            enableToggle.isOn = newState;
        }
    }

    private static readonly Regex sWhitespace = new Regex(@"\s+");

    private void RefreshFilter()
    {
        string mustContain = "";
        if (!string.IsNullOrEmpty(rawFilterString))
        {
            mustContain = GetSearchString(rawFilterString);
        }
        if (string.IsNullOrEmpty(mustContain))
        {
            foreach (var button in pluginButtons)
            {
                SetButtonActive(button, true);
            }
            return;
        }
        foreach (var button in pluginButtons)
        {
            var data = button.data;
            if (data != null && !string.IsNullOrEmpty(data.GUID) && !string.IsNullOrEmpty(data.Name))
            {
                bool allowedByFilter = CompareInitials(data.Name, mustContain) || GetSearchString(data.GUID).Contains(mustContain) || GetSearchString(data.Name).Contains(mustContain);
                SetButtonActive(button, allowedByFilter);
            }
        }
    }

    private void SetButtonActive(PluginButton button, bool active)
    {
        if (button == null) return;
        button.gameObject.SetActive(active);
        var colorSwap = button.gameObject.GetComponent<uGUI_BasicColorSwap>();
        if (colorSwap != null && !active) colorSwap.makeTextWhite();
    }

    private static bool CompareInitials(string fullName, string initials)
    {
        int index = 0;
        foreach (var c in fullName)
        {
            if (index < initials.Length && char.IsUpper(c) && char.ToLower(c) == char.ToLower(initials[index]))
            {
                index++;
            }
        }
        return index == initials.Length;
    }

    // space and case insensitive
    private static string GetSearchString(string input) => sWhitespace.Replace(input.Trim(), "").ToLower();

    private void FormatDependency(StringBuilder sb, PluginData plugin, PluginDependency dependency)
    {
        if (dependency.IsSoft) return; // skip soft dependencies to avoid confusion

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
            sb.Append(" v" + dependency.versionRequirement);
        }
        sb.Append(" ");

        if (shownDisplayName)
        {
            sb.Append($"({dependency.guid})");
        }

        var optional = !dependency.IsHard;

        if (optional)
        {
            sb.Append("Optional");
        }

        var dependencyState = plugin.HasDependency(lastLoadedPluginData, dependency);
        sb.Append("\n -");
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
                    sb.AppendLine("-Not enabled");
                else
                    sb.AppendLine("<color=#FF0000>Not enabled</color>");
                break;
            case DependencyState.Outdated:
                sb.AppendLine("<color=#FF0000>Update required!</color>");
                break;
        }
    }
}
