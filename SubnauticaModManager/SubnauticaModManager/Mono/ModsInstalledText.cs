using SubnauticaModManager.Files;

namespace SubnauticaModManager.Mono;

internal class ModsInstalledText : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        text.text = "";
    }

    private void Update()
    {
        text.text = GetCurrentText();
    }
    
    private string GetCurrentText()
    {
        var known = KnownPlugins.list;
        if (known == null) return $"({Translation.Translate("Error")})";
        var count = GetInstalledModsCount(known);
        if (count == 0) return Translation.Translate("NoModsInstalled");
        if (count == 1) return Translation.Translate("OneModInstalled");
        return Translation.TranslateFormat("MultipleModsInstalled", count);
    }

    private int GetInstalledModsCount(List<PluginData> list)
    {
        int c = 0;
        foreach (var mod in list)
        {
            if (mod != null && mod.Installed && mod.IsValid)
            {
                c++;
            }
        }
        return c;
    }
}
