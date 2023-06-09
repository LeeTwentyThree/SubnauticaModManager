using SubnauticaModManager.Files;

namespace SubnauticaModManager.Mono;

internal class PluginButton : MonoBehaviour
{
    public PluginData data;
    private TextMeshProUGUI mainText;
    private TextMeshProUGUI statusText;
    private Button button;
    private Image image;

    private Color defaultColor = Color.white;
    private Color disabledColor = new Color(1, 1, 1, 0.3f);

    public bool pluginSupposedToBeEnabled;

    private void Awake()
    {
        mainText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        statusText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        button = gameObject.GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(OnClick);
        image = gameObject.gameObject.GetComponent<Image>();
    }

    public void SetData(PluginData data)
    {
        this.data = data;
        mainText.text = $"{data.Name} v{data.Version}";
        var warningType = GetWarningType();
        statusText.text = warningType.FormatPluginStatus(data.Installed);
        if (warningType != PluginStatusType.NoError)
        {
            image.sprite = Plugin.assetBundle.LoadAsset<Sprite>("Panel-Warning");
        }
        pluginSupposedToBeEnabled = data.Installed;
    }

    private void Update()
    {
        image.color = pluginSupposedToBeEnabled ? defaultColor : disabledColor;
    }

    private bool IsCurrentlySelected()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) return false;
        if (menu.modManagerTab.currentData == null) return false;
        return menu.modManagerTab.currentData.Equals(data);
    }

    private void OnClick()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) return;
        menu.modManagerTab.SetActiveMod(data);
        SoundUtils.PlaySound(UISound.Tweak);
    }

    private PluginStatusType GetWarningType()
    {
        if (data == null) return PluginStatusType.CouldNotFind;

        var knownPlugins = KnownPlugins.list;
        if (knownPlugins == null) return PluginStatusType.CouldNotFind;

        if (!data.HasAllHardDependencies(knownPlugins)) return PluginStatusType.MissingDependencies;
        if (data.GetIsDuplicate(knownPlugins)) return PluginStatusType.Duplicate;
        if (!data.PluginIsLoaded) return PluginStatusType.FailedToLoad;

        return PluginStatusType.NoError;
    }
}
