namespace SubnauticaModManager.Mono;

internal class OpenModDownloadsFolderButton : MonoBehaviour
{
    private string modDownloadsDirectory;

    private void Start()
    {
        modDownloadsDirectory = FileManagement.ModDownloadFolder;
        var button = gameObject.GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        SoundUtils.PlaySound(UISound.Tweak);
        Application.OpenURL(modDownloadsDirectory);
    }
}
