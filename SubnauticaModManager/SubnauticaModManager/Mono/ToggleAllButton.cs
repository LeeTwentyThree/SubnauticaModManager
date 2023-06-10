namespace SubnauticaModManager.Mono;

internal class ToggleAllButton : MonoBehaviour
{
    public bool targetEnabledState;

    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => OnClick());
    }

    private void OnClick()
    {
        SoundUtils.PlaySound(UISound.Select);
        string action = Translation.Translate(targetEnabledState ? "Enable" : "Disable");
        ModManagerMenu.main.prompt.Ask(Translation.Translate("ToggleAllMods"), Translation.TranslateFormat("ToggleAllModsConfirmation", action),
            new PromptChoice[]
            {
                new PromptChoice(Translation.Translate("Yes"), Perform),
                new PromptChoice(Translation.Translate("No"))
            });
    }

    private void Perform()
    {
        var modManageTab = ModManagerMenu.main.modManagerTab;
        modManageTab.ToggleAll(targetEnabledState);
    }
}
