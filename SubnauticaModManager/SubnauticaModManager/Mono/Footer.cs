using SubnauticaModManager.Files;

namespace SubnauticaModManager.Mono;

internal class Footer : MonoBehaviour
{
    private void Update()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) return;

        var waitingOnRestart = ModArrangement.WaitingOnRestart;

        menu.closeButton.gameObject.SetActive(!waitingOnRestart);
        menu.quitGameButton.gameObject.SetActive(waitingOnRestart);
        menu.restartRequiredText.gameObject.SetActive(waitingOnRestart);
    }
}
