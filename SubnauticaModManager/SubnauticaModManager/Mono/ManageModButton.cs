using SubnauticaModManager.Files;

namespace SubnauticaModManager.Mono;

internal class ManageModButton : MonoBehaviour
{
    public PluginData data;
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = gameObject.GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetData(PluginData data)
    {
        this.data = data;
        text.text = $"{data.Name} v{data.Version} ({data.EnabledStateText})";
    }
}
