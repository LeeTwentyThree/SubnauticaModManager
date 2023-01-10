using SubnauticaModManager.Files;

namespace SubnauticaModManager.Mono;

internal class CloseModManagerOnEscape : MonoBehaviour
{
    private void Update()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) return;

        if (Input.GetKeyDown(KeyCode.Escape) && CanExit(menu))
        {
            MenuCreator.HideMenu();
        }
    }

    private bool CanExit(ModManagerMenu menu)
    {
        if (ModArrangement.WaitingOnRestart) return false;
        bool inNewsTab = menu.tabManager.ActiveTab is TabNews;
        if (LoadingProgress.Busy && !inNewsTab) return false;
        return true;
    }
}
