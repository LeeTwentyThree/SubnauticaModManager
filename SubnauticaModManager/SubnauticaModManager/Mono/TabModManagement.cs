using SubnauticaModManager.Files;

namespace SubnauticaModManager.Mono;

internal class TabModManagement : Tab
{
    private Transform buttonsParent;
    private GameObject manageArea;
    private GameObject manageAreaPlaceholder;

    private PluginData currentData;
    private TextMeshProUGUI titleText;
    private TextMeshProUGUI versionText;
    private TextMeshProUGUI guidText;
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
        enableToggle = gameObject.GetComponentInChildren<Toggle>();
        enableToggle.onValueChanged = new Toggle.ToggleEvent();
        enableToggle.onValueChanged.AddListener(OnToggleChanged);
        openFolderButton = gameObject.GetComponentInChildren<Button>();
        openFolderButton.onClick = new Button.ButtonClickedEvent();
        openFolderButton.onClick.AddListener(OpenFolder);

        modManageButton = Plugin.assetBundle.LoadAsset<GameObject>("ManageTabButton");

        SetActiveMod(null);
    }

    private void OpenFolder()
    {
        if (currentData != null && currentData.IsValid)
        {
            Application.OpenURL(currentData.FolderPath);
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
        var dllsInPluginsFolder = FileManagement.GetDLLs(FileManagement.BepInExPluginsFolder);
        var pluginDLLs = PlugingManagement.FilterPluginsFromDLLs(dllsInPluginsFolder, PluginLocation.Plugins);
        foreach (var plugin in pluginDLLs)
        {
            var spawned = Instantiate(modManageButton);
            spawned.SetActive(true);
            spawned.GetComponent<RectTransform>().SetParent(buttonsParent, false);
            spawned.AddComponent<ManageModButton>().SetData(plugin);
        }
    }

    private void OnToggleChanged(bool state)
    {

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
    }
}
