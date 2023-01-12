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
                StringConstants.unsavedChanges,
                "You have unsaved changes. Are you sure you wish to continue?",
                new PromptChoice("Yes", true, () => QuitGame()),
                new PromptChoice("No")
            );
            return;
        }
        menu.prompt.Ask(
                StringConstants.applyChanges,
                "Apply all changes and restart the game?",
                new PromptChoice("Yes", false, () => Files.ModArrangement.RestartAndApplyChanges()),
                new PromptChoice("No")
            );
    }

    private void QuitGame()
    {
        SoundUtils.PlaySound(UISound.BatteryDie);
        Application.Quit();
    }
}