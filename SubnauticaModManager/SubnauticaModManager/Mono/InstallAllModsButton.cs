using SubnauticaModManager.Files;

namespace SubnauticaModManager.Mono;

internal class InstallAllModsButton : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Start()
    {
        var button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
        text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnClick()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) return;
        if (LoadingProgress.Busy) return;
        SoundUtils.PlaySound(UISound.Tweak);
        if (ModArrangement.WaitingOnRestart)
        {
            ModArrangement.WarnPossibleConflict();
            return;
        }
        if (ModInstalling.GetDownloadedModsCount() == 0)
        {
            menu.prompt.Ask(
            StringConstants.failed,
            "No mods to install!",
            new PromptChoice("Ok")
            );
            return;
        }
        menu.prompt.Ask(
            StringConstants.installAllMods,
            "Do you want to install all mods? This operation will extract and remove all mod zip files.",
            new PromptChoice("Yes", OnConfirm),
            new PromptChoice("No")
            );
    }

    private void OnConfirm()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) return;
        if (LoadingProgress.Busy) return;
        UWE.CoroutineHost.StartCoroutine(ModInstalling.InstallAllMods());
    }

    private void Update()
    {
        var modCount = ModInstalling.GetDownloadedModsCount();
        text.text = modCount == 1 ? text.text = $"Install all mods (1 file detected)" : $"Install all mods ({modCount} files detected)";
    }
}
