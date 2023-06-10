using SubnauticaModManager.Web;

namespace SubnauticaModManager.Mono;

internal class SubmodicaModButton : MonoBehaviour
{
    private SubmodicaMod modData;

    public void SetCurrentModData(SubmodicaMod modData)
    {
        this.modData = modData;
        var modImageSprite = modData.ModImageSprite;
        if (modImageSprite == null) modImageSprite = Plugin.assetBundle.LoadAsset<Sprite>("DefaultModIcon");
        gameObject.SearchChild("Image").GetComponent<Image>().sprite = modImageSprite;
        gameObject.SearchChild("Title").GetComponent<TextMeshProUGUI>().text = modData.Title;
        gameObject.SearchChild("Author").GetComponent<TextMeshProUGUI>().text = modData.Creator;
        gameObject.SearchChild("Tagline").GetComponent<TextMeshProUGUI>().text = modData.Tagline;
        gameObject.SearchChild("Version").GetComponent<TextMeshProUGUI>().text = modData.GetVersionWithTimestampsString();
        gameObject.SearchChild("Views").GetComponent<TextMeshProUGUI>().text = modData.GetViewsString();
        gameObject.SearchChild("Downloads").GetComponent<TextMeshProUGUI>().text = modData.GetDownloadsString();
        gameObject.SearchChild("Favorites").GetComponent<TextMeshProUGUI>().text = modData.GetFavoritesString();
    }

    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => OnClick());
    }

    private void OnClick()
    {
        var menu = ModManagerMenu.main;
        if (menu == null || modData == null || string.IsNullOrEmpty(modData.Url)) return;
        SoundUtils.PlaySound(UISound.Select);
        menu.prompt.Ask(
                Translation.Translate(StringConstants.viewURLInBrowser),
                modData.Url,
                new PromptChoice(Translation.Translate("Yes"), () => ViewInBrowser()),
                new PromptChoice(Translation.Translate("No"))
            );
        return;
    }

    private void ViewInBrowser()
    {
        Application.OpenURL(modData.Url);
    }
}
