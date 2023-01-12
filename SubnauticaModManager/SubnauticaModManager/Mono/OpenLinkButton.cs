namespace SubnauticaModManager.Mono;

internal class OpenLinkButton : MonoBehaviour
{
    public string link;

    private void Start()
    {
        var button = gameObject.GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        var menu = ModManagerMenu.main;
        if (menu == null || string.IsNullOrEmpty(link)) return;
        SoundUtils.PlaySound(UISound.Select);
        menu.prompt.Ask(
                StringConstants.viewURLInBrowser,
                "Would you like to view this in your browser? " + link,
                new PromptChoice("Yes", () => ViewInBrowser()),
                new PromptChoice("No")
            );
        return;
    }

    private void ViewInBrowser()
    {
        Application.OpenURL(link);
    }
}
