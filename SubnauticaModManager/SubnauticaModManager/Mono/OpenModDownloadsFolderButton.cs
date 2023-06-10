using SubnauticaModManager.Files;

namespace SubnauticaModManager.Mono;

internal class OpenModDownloadsFolderButton : MonoBehaviour
{
    private string modDownloadsDirectory;

    private void Start()
    {
        modDownloadsDirectory = CustomDownloadsFolder.CurrentModDownloadFolder;
        var button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        SoundUtils.PlaySound(UISound.Tweak);
        Application.OpenURL(modDownloadsDirectory);
    }
}