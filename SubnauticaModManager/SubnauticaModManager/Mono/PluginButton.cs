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
        statusText.text = data.StatusText;
        if (UseWarningSprite())
        {
            image.sprite = Plugin.assetBundle.LoadAsset<Sprite>("Panel-Warning");
            statusText.text = "Missing dependencies!";
        }
    }

    private void Update()
    {
        image.color = (data.Installed) ? defaultColor : disabledColor;
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
    }

    private bool UseWarningSprite()
    {
        if (data == null) return true;

        var menu = ModManagerMenu.main;
        if (menu == null) return true;

        var loadedData = menu.modManagerTab.lastLoadedPluginData;
        if (loadedData == null) return true;

        if (!data.HasAllHardDependencies(loadedData)) return true;

        return false;
    }
}
