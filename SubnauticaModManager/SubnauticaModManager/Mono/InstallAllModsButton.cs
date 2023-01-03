using SubnauticaModManager.Mods;

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
        menu.prompt.Ask(
            "Do you want to automatically install all mods? This operation will extract and remove all zip files in the folder.",
            new PromptChoice("Yes", OnConfirm),
            new PromptChoice("No")
            );
    }

    private void OnConfirm()
    {

    }

    private void Update()
    {
        var modCount = ModInstalling.GetDownloadedModsCount();
        text.text = modCount == 1 ? text.text = $"Install all mods (1 mod file detected in your downloads folder)" : $"Install all mods ({modCount} mod files detected in your downloads folder)";
    }
}
