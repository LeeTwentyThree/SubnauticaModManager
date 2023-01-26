namespace SubnauticaModManager.Mono;

internal class FilterModsInputField : MonoBehaviour
{
    private TMP_InputField field;

    private void Start()
    {
        field = gameObject.GetComponent<TMP_InputField>();
        field.onValueChanged = new TMP_InputField.OnChangeEvent();
        field.onValueChanged.AddListener(OnTextUpdated);
    }

    private void OnTextUpdated(string newText)
    {
        var manageTab = ModManagerMenu.main.modManagerTab;
        if (manageTab != null)
        {
            manageTab.FilterMods(newText);
        }
    }

    public string CurrentText => field.text;
}
