using rail;

namespace SubnauticaModManager.Localization;

public static class Translation
{
    private static bool _localizationInitialized;
    private static readonly Dictionary<string, Language> _languages = new();

    private const string kFallbackLanguageName = "English";

    private static Dictionary<string, string> _languageRedirections = new Dictionary<string, string>()
    {
        { "Spanish (Latin America)", "Spanish" }
    };

    private static void EnsureLocalizationExists()
    {
        var localizationFolderPath = Path.Combine(FileManagement.ThisPluginFolder, "Localization");

        if (!Directory.Exists(localizationFolderPath))
        {
            Plugin.Logger.LogError("Localization not found!");
            return;
        }

        var languageFilePaths = Directory.GetFiles(localizationFolderPath);

        foreach (var file in languageFilePaths)
        {
            var language = Language.LoadFromJsonFile(file);
            _languages.Add(language.name, language);
        }
        
        _localizationInitialized = true;
    }

    public static bool UsingEnglish()
    {
        return GetCurrentLanguage() == kFallbackLanguageName;
    }

    public static string GetCurrentLanguage()
    {
        var raw = GetRawCurrentLanguage();
        if (_languageRedirections.TryGetValue(raw, out var redirected))
        {
            return redirected;
        }
        return raw;
    }

    private static string GetRawCurrentLanguage()
    {
        var language = global::Language.main;
        if (language == null)
        {
            Plugin.Logger.LogWarning("Game language not initialized yet!");
            var lastFoundLanguage = PlayerPrefs.GetString("Language", null);

            return string.IsNullOrEmpty(lastFoundLanguage) ? kFallbackLanguageName : lastFoundLanguage;
        }
        return language.GetCurrentLanguage();
    }

    public static string Translate(string key)
    {
        if (!_localizationInitialized)
        {
            EnsureLocalizationExists();
        }

        if (_languages.TryGetValue(GetCurrentLanguage(), out var lang))
        {
            if (lang.TryGetValue(key, out var translated))
            {
                return translated;
            }
        }
        if (_languages.TryGetValue(kFallbackLanguageName, out var fallbackLang))
        {
            if (fallbackLang.TryGetValue(key, out var translated))
            {
                return translated;
            }
        }
        return key;
    }

    public static string TranslateFormat(string key, params object[] args)
    {
        return string.Format(Translate(key), args);
    }
}