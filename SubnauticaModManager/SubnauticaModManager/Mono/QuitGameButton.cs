namespace SubnauticaModManager.Mono;

internal class QuitGameButton : MonoBehaviour
{
    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => OnClick());
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
                new PromptChoice(Translation.Translate("Yes"), true, () => QuitGame()),
                new PromptChoice(Translation.Translate("No"))
            );
            return;
        }
        menu.prompt.Ask(
                Translation.Translate(StringConstants.applyChanges),
                Translation.Translate("ApplyChangesDescription"),
                new PromptChoice(Translation.Translate("Yes"), false, () => Files.ModArrangement.RestartAndApplyChanges()),
                new PromptChoice(Translation.Translate("No"))
            );
    }

    private void QuitGame()
    {
        SoundUtils.PlaySound(UISound.BatteryDie);
        Application.Quit();
    }
}