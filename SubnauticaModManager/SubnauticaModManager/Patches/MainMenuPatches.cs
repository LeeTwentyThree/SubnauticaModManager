namespace SubnauticaModManager.Patches;

[HarmonyPatch(typeof(uGUI_MainMenu))]
internal class MainMenuPatches
{
    [HarmonyPatch(nameof(uGUI_MainMenu.Awake))]
    [HarmonyPostfix]
    private static void AwakePatch(uGUI_MainMenu __instance)
    {
        var playButton = __instance.gameObject.GetComponentInChildren<MainMenuPrimaryOptionsMenu>().transform.Find("PrimaryOptions/MenuButtons/ButtonPlay").gameObject;
        var modManagerButton = Object.Instantiate(playButton);
        modManagerButton.GetComponent<RectTransform>().SetParent(playButton.transform.parent, false);
        modManagerButton.name = "ButtonModManager";
        var text = modManagerButton.GetComponentInChildren<TextMeshProUGUI>();
        text.text = Translation.Translate("ModManagerButton");
        Object.DestroyImmediate(text.gameObject.GetComponent<TranslationLiveUpdate>());
        text.gameObject.AddComponent<TranslatableText>().languageKey = "ModManagerButton";
        modManagerButton.transform.SetSiblingIndex(1);
        var button = modManagerButton.GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(MenuCreator.ShowMenu);
    }
}