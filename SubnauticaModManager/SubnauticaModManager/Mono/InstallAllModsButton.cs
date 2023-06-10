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
        if (!VerifyIntegrity.IsIntact)
        {
            VerifyIntegrity.WarnNotIntact();
            return;
        }
        if (ModArrangement.WaitingOnRestart)
        {
            ModArrangement.WarnPossibleConflict();
            return;
        }
        if (ModInstalling.GetDownloadedModsCount() == 0)
        {
            menu.prompt.Ask(
                Translation.Translate(StringConstants.failed),
                Translation.Translate("NoModsToInstall"),
                new PromptChoice(Translation.Translate("Ok"))
            );
            return;
        }
        menu.prompt.Ask(
            Translation.Translate(StringConstants.installAllMods),
            Translation.Translate("InstallAllModsDescription"),
            new PromptChoice(Translation.Translate("Yes"), OnConfirm),
            new PromptChoice(Translation.Translate("No"))
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
        text.text = modCount == 1 ? text.text = Translation.Translate("InstallOneMod") : Translation.TranslateFormat("InstallMultipleOrZeroMods", modCount);
    }
}
