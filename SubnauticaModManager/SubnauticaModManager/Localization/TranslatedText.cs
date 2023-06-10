namespace SubnauticaModManager.Localization;

internal class TranslatableText : MonoBehaviour
{
    public string languageKey;

    private TextMeshProUGUI textObject;

    private void Start()
    {
        textObject = GetComponent<TextMeshProUGUI>();
        if (textObject == null)
            textObject = GetComponentInChildren<TextMeshProUGUI>();
        UpdateText();
    }

    private void OnEnable()
    {
        global::Language.OnLanguageChanged += UpdateText;
        if (textObject != null)
        {
            UpdateText();
        }
    }

    private void OnDisable()
    {
        global::Language.OnLanguageChanged -= UpdateText;
    }

    public void UpdateText()
    {
        textObject.text = Translation.Translate(languageKey);
    }

    public static void Create(GameObject obj, string languageKey)
    {
        obj.AddComponent<TranslatableText>().languageKey = languageKey;
    }
}