namespace SubnauticaModManager.Mono;

internal class MainHeader : MonoBehaviour
{
    private void Start()
    {
        var text = GetComponent<TextMeshProUGUI>();
        if (Translation.UsingEnglish())
        {
            text.text = $"{Plugin.Name} - v{Plugin.Version}";
        }
        else
        {
            text.text = $"{Translation.Translate("ModTitle")} - {Translation.TranslateFormat("VersioningFormat", Plugin.Version)}";
        }
    }
}
