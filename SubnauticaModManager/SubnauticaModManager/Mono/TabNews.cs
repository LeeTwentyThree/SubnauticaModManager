using System.Collections;
using SubnauticaModManager.Web;

namespace SubnauticaModManager.Mono;

internal class TabNews : Tab
{
    public Transform parent;

    private GameObject newsEntryPrefab;
    private GameObject newsEntryButtonPrefab;

    public override void OnCreate()
    {
        newsEntryPrefab = Plugin.assetBundle.LoadAsset<GameObject>("NewsEntry");
        parent = gameObject.SearchChild("Content").transform;
    }

    public override void OnActivate()
    {
        StopAllCoroutines();
        StartCoroutine(ActivateCoroutine());
    }

    private IEnumerator ActivateCoroutine()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) yield break;
        if (LoadingProgress.Busy) yield break;
        var request = new NewsRequest();
        yield return NewsReader.ReadNews(new LoadingProgress(), request);
        ShowResults(request);
    }

    private void ShowResults(NewsRequest newsRequest)
    {
        if (newsRequest.data == null) return;
        ClearList();
        foreach (var item in newsRequest.data)
        {
            if (item != null) ShowNewsEntry(item);
        }
    }

    private void ClearList()
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    private void ShowNewsEntry(NewsEntry entry)
    {
        bool button = !string.IsNullOrEmpty(entry.link);
        var spawned = Instantiate(button ? newsEntryButtonPrefab : newsEntryPrefab);
        spawned.transform.SetParent(parent, false);
        spawned.transform.Find("Title").GetComponent<TextMeshProUGUI>().text = $"{entry.title} - {entry.date}";
        spawned.transform.Find("Body").GetComponent<TextMeshProUGUI>().text = entry.body;
        if (button) spawned.AddComponent<OpenLinkButton>().link = entry.link;
    }
}
