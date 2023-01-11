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
        if (known == null) return "(Error)";
        var count = GetInstalledModsCount(known);
        if (count == 0) return "No mods installed";
        if (count == 1) return "1 mod installed";
        return $"{count} mods installed";
    }

    private int GetInstalledModsCount(List<PluginData> list)
    {
        int c = 0;
        foreach (var mod in list)
        {
            if (mod != null && mod.IsValid && mod.Installed)
            {
                c++;
            }
        }
        return c;
    }
}
