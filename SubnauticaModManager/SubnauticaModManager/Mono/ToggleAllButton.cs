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
        string action = targetEnabledState ? "enable" : "disable";
        ModManagerMenu.main.prompt.Ask($"Toggle all mods?", $"Would you like to {action} all mods?",
            new PromptChoice[]
            {
                new PromptChoice("Yes", Perform),
                new PromptChoice("No")
            });
    }

    private void Perform()
    {
        var modManageTab = ModManagerMenu.main.modManagerTab;
        modManageTab.ToggleAll(targetEnabledState);
    }
}
