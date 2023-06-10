namespace SubnauticaModManager.Mono;

internal class CloseButton : MonoBehaviour
{
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
        if (menu.UnappliedChanges)
        {
            menu.prompt.Ask(
                Translation.Translate(StringConstants.unsavedChanges),
                Translation.Translate("UnsavedChangesDescription"),
                new PromptChoice(Translation.Translate("Yes"), true, () => MenuCreator.HideMenu()),
                new PromptChoice(Translation.Translate("No"))
            );
            return;
        }
        MenuCreator.HideMenu();
    }
}
