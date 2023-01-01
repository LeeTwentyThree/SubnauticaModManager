﻿using System.Collections;
using SubnauticaModManager.Web;

namespace SubnauticaModManager.Mono;

internal class LoadMostRecentButton : MonoBehaviour
{
    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick = new Button.ButtonClickedEvent();
        button.onClick.AddListener(() => OnClick());
    }

    private void OnClick()
    {
        StartCoroutine(Behaviour());
    }

    private IEnumerator Behaviour()
    {
        var menu = ModManagerMenu.main;
        if (menu == null) yield break;
        if (LoadingProgress.Busy) yield break;
        var searchResult = new SubmodicaSearchResult();
        yield return SubmodicaAPI.SearchRecentlyUpdated(new LoadingProgress(), searchResult);
        menu.downloadModsTab.ShowModResults(searchResult);
    }
}
