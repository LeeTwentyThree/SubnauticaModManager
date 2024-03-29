﻿namespace SubnauticaModManager;

internal static class Helpers
{
    public static void FixText(TextMeshProUGUI text, FontManager.FontType font = FontManager.FontType.Aller_Rg)
    {
        text.font = FontManager.GetFontAsset(font);
    }

    public static void FixButton(Button button, bool colorSwap = true)
    {
        if (colorSwap)
        {
            button.gameObject.EnsureComponent<Mono.ButtonColorFixer>();
        }
    }

    public static void FixScrollRect(ScrollRect sr)
    {
        var obj = sr.gameObject;
        var content = sr.content;
        var viewport = sr.viewport;
        var verticalSB = sr.verticalScrollbar;
        var visibility = sr.verticalScrollbarVisibility;

        Object.DestroyImmediate(sr);

        var n = obj.AddComponent<Mono.FixedScrollRect>();
        n.content = content;
        n.viewport = viewport;
        n.verticalScrollbar = verticalSB;
        n.verticalScrollbarVisibility = visibility;

        n.horizontal = false;
        n.scrollSensitivity = 7;
    }

    public static void FixUIObjects(GameObject root)
    {
        var texts = root.GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (var text in texts)
            FixText(text);
        var buttons = root.GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
            FixButton(button);
        var scrollRects = root.GetComponentsInChildren<ScrollRect>(true);
        foreach (var scrollRect in scrollRects)
            FixScrollRect(scrollRect);
    }

    public static FMODAsset GetFmodAsset(string path)
    {
        var asset = ScriptableObject.CreateInstance<FMODAsset>();
        asset.path = path;
        asset.id = path;
        return asset;
    }
}
/// <summary>
/// Enum primarily used in the <see cref="ExtensionMethods.Compare"/> method.
/// </summary>
public enum CompareMode
{
    /// <summary>
    /// a == a, A == a, b != a
    /// </summary>
    Equals,

    /// <summary>
    /// A != a
    /// </summary>
    EqualsCaseSensitive,

    /// <summary>
    /// Whether this string starts with the given string. Not case sensitive.
    /// </summary>
    StartsWith,

    /// <summary>
    /// Whether this string starts with the given string. Case sensitive.
    /// </summary>
    StartsWithCaseSensitive,

    /// <summary>
    /// Whether this string contains the given string. Not case sensitive.
    /// </summary>
    Contains,

    /// <summary>
    /// Whether this string contains the given string. Case sensitive.
    /// </summary>
    ContainsCaseSensitive,

    /// <summary>
    /// Whether this string ends with or starts with the given string. Not case sensitive.
    /// </summary>
    StartsWithOrEndsWith,

    /// <summary>
    /// Whether this string ends with or starts with the given string. Case sensitive.
    /// </summary>
    StartsWithOrEndsWithCaseSensitive,
}