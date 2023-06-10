using Newtonsoft.Json.Linq;

namespace SubnauticaModManager.Localization;

internal class Language
{
    public readonly string name;

    private readonly Dictionary<string, string> translations;

    public Language(string name, Dictionary<string, string> translations)
    {
        this.name = name;
        this.translations = translations;
    }

    public static Language LoadFromJsonFile(string jsonFilePath)
    {
        var json = File.ReadAllText(jsonFilePath);
        return new Language(Path.GetFileNameWithoutExtension(jsonFilePath), JObject.Parse(json).ToObject<Dictionary<string, string>>());
    }

    public bool TryGetValue(string key, out string value)
    {
        return translations.TryGetValue(key, out value);
    }
}