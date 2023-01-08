using SubnauticaModManager.Files;
using System.Text;

namespace SubnauticaModManager.Mono;

internal class TabModManagement : Tab
{
    public List<PluginData> lastLoadedPluginData;
    public PluginData currentData;

    private Transform buttonsParent;
    private GameObject manageArea;
    private GameObject manageAreaPlaceholder;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI versionText;
    private TextMeshProUGUI guidText;
    private TextMeshProUGUI dependencyText;
    private Toggle enableToggle;
    private Button openFolderButton;
    private GameObject modManageButton;

    public override void OnCreate()
    {
        buttonsParent = gameObject.SearchChild("Content").transform;
        manageArea = transform.Find("ManageArea").gameObject;
        manageAreaPlaceholder = transform.Find("ManageAreaPlaceholder").gameObject;

        titleText = gameObject.SearchChild("Title").GetComponent<TextMeshProUGUI>();
        versionText = gameObject.SearchChild("Version").GetComponent<TextMeshProUGUI>();
        guidText = gameObject.SearchChild("GUID").GetComponent<TextMeshProUGUI>();
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
    }

    private void UpdateModList()
    {
        foreach (Transform child in buttonsParent)
        {
            Destroy(child.gameObject);
        }
        lastLoadedPluginData = PluginUtils.GetAllPluginData(true);
        foreach (var plugin in lastLoadedPluginData)
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
        if (currentData == null) return;
        ModArrangement.SetPluginEnabled(currentData.GUID, state);
    }

    public void SetActiveMod(PluginData data)
    {
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
        titleText.text = data.Name;
        versionText.text = "v" + data.Version;
        guidText.text = "GUID: " + data.GUID;
        enableToggle.isOn = data.Location == PluginLocation.Plugins;
        dependencyText.text = GetDependencyText(data);
    }

    private string GetDependencyText(PluginData data)
    {
        if (data.Dependencies == null || data.Dependencies.Length == 0 || lastLoadedPluginData == null) return "None listed";
        var sb = new StringBuilder();
        foreach (var dependency in data.Dependencies)
        {
            if (data.HasDependency(lastLoadedPluginData, dependency))
            {
                sb.AppendLine(dependency.GetDisplayNameOrDefault());
            }
        }
        return sb.ToString().TrimEnd('\n');
    }
}
