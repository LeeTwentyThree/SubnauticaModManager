using UnityEngine.EventSystems;

namespace SubnauticaModManager.Mono;

internal class ButtonColorFixer : MonoBehaviour
{
    private void Start()
    {
        var c = gameObject.EnsureComponent<uGUI_BasicColorSwap>();
        var eventTrigger = gameObject.EnsureComponent<EventTrigger>();

        eventTrigger.triggers = new List<EventTrigger.Entry>();

        var enter = new EventTrigger.Entry();
        enter.eventID = EventTriggerType.PointerEnter;
        enter.callback.AddListener((eventData) => { c.makeTextBlack(); });

        var exit = new EventTrigger.Entry();
        exit.eventID = EventTriggerType.PointerExit;
        exit.callback.AddListener((eventData) => { c.makeTextWhite(); });

        eventTrigger.triggers.Add(enter);
        eventTrigger.triggers.Add(exit);
    }
}
