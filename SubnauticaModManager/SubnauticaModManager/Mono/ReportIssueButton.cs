﻿namespace SubnauticaModManager.Mono;

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
                "Open GitHub in a new browser tab?",
                new PromptChoice("Continue", () => ViewInBrowser()),
                new PromptChoice("Cancel")
            );
        return;
    }

    private void ViewInBrowser()
    {
        Application.OpenURL(url);
    }
}
