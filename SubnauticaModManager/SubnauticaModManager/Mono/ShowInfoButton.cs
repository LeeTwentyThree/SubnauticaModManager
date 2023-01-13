namespace SubnauticaModManager.Mono;

internal class ShowInfoButton : MonoBehaviour
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
        if (menu.prompt.isActiveAndEnabled) return;
        if (LoadingProgress.Busy) return;
        if (Files.ModArrangement.WaitingOnRestart) return;

        menu.infoPanel.SetActive(true);
        SoundUtils.PlaySound(UISound.Tweak);
    }
}