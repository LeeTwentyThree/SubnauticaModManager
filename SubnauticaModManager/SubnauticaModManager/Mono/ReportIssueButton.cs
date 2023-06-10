namespace SubnauticaModManager.Mono;

internal class ReportIssueButton : MonoBehaviour
{
    private const string url = "https://github.com/LeeTwentyThree/SubnauticaModManager/issues";

    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => OnClick() );
    }

    private void OnClick()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) return;
        SoundUtils.PlaySound(UISound.Select);
        menu.prompt.Ask(
                Translation.Translate(StringConstants.viewURLInBrowser),
                url,
                new PromptChoice(Translation.Translate("Continue"), () => ViewInBrowser()),
                new PromptChoice(Translation.Translate("Cancel"))
            );
        return;
    }

    private void ViewInBrowser()
    {
        Application.OpenURL(url);
    }
}
