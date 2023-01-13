namespace SubnauticaModManager.Mono;

internal class CloseInfoButton : MonoBehaviour
{
    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => OnClick() );
    }

    private void OnClick()
    {
        ModManagerMenu.main.infoPanel.SetActive(false);
    }
}
