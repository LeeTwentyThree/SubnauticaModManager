using System.Collections;
using SubnauticaModManager.Web;

namespace SubnauticaModManager.Mono;

internal class SubmodicaSearchButton : MonoBehaviour
{
    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => OnClick());
    }

    private void OnClick()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) return;
        menu.submodicaSearchBar.BeginSearching();
    }
}
