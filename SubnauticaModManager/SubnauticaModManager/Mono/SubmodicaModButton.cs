using SubnauticaModManager.Web;

namespace SubnauticaModManager.Mono;

internal class SubmodicaModButton : MonoBehaviour
{
    private SubmodicaMod modData;

    public void SetCurrentModData(SubmodicaMod modData)
    {
        this.modData = modData;
        gameObject.SearchChild("Image").GetComponent<Image>().sprite = modData.ModImageSprite;
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
        menu.prompt.Ask(
                "Would you like to view this mod page in your browser? " + modData.Url,
                new PromptChoice("Yes", () => ViewInBrowser()),
                new PromptChoice("No")
            );
        return;
    }

    private void ViewInBrowser()
    {
        Application.OpenURL(modData.Url);
    }
}
