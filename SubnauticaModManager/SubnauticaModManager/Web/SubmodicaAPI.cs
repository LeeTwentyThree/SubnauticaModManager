using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json;

namespace SubnauticaModManager.Web;

public static class SubmodicaAPI
{
    private const string key = "0XAWs3EuMW5PyvpLbFVf7DviL86QQPtP";

    private const string urlFormat = "https://submodica.xyz/api/search/{0}/{1}/{2}"; // 0 - game, 1 - token, 2 - url-encoded query

    private const int maxQueryLength = 128;

    private const float search_fakeLoadDuration = 0.5f;

    public const float errorDisplayDuration = 3f;

    public static class Game
    {
        public static string Subnautica = "sn1";
        public static string BelowZero = "sbz";

        public static string Current
        {
            get
            {
                return Subnautica;
            }
        }
    }

    public static bool IsValidSearchQuery(string query)
    {
        if (string.IsNullOrEmpty(query)) return false;
        if (query.Length > maxQueryLength) return false;
        return true;
    }

    private static string GetSearchURL(string query)
    {
        return string.Format(urlFormat, Game.Current, key, UnityWebRequest.EscapeURL(query.ToLower()));
    }

    public static IEnumerator SearchRecentlyUpdated(LoadingProgress loadingProgress, SubmodicaSearchResult result)
    {
        yield return Search("recently_updated", loadingProgress, result);
    }

    public static IEnumerator SearchMostDownloaded(LoadingProgress loadingProgress, SubmodicaSearchResult result)
    {
        yield return Search(string.Empty, loadingProgress, result);
    }

    public static IEnumerator Search(string query, LoadingProgress loadingProgress, SubmodicaSearchResult result)
    {
        var url = GetSearchURL(query);
        string text = "";
        using (var request = UnityWebRequest.Get(url))
        {
            loadingProgress.Status = "Fetching search results...";
            while (!request.isDone)
            {
                yield return null;
                loadingProgress.Progress = request.downloadProgress;
            }
            if (!string.IsNullOrEmpty(request.error))
            {
                yield return LoadingError(request.error, loadingProgress);
                yield break;
            }
        }

        if (string.IsNullOrEmpty(text))
        {
            loadingProgress.Status = "No data loaded!";
            yield return new WaitForSeconds(errorDisplayDuration);
            loadingProgress.Complete();
            yield break;
        }

        loadingProgress.Status = "Creating mod list...";

        result.SetData(JsonConvert.DeserializeObject<SubmodicaSearchResult.Data>(text));

        if (!result.Success)
        {
            yield return LoadingError("Search failed!", loadingProgress);
            yield break;
        }
        if (!result.ValidResults)
        {
            yield return LoadingError("No results!", loadingProgress);
            yield break;
        }

        foreach (var mod in result.Mods)
        {
            if (mod != null)
            {
                yield return mod.LoadData();
            }
        }

        loadingProgress.Status = "Success!";
        yield return new WaitForSeconds(search_fakeLoadDuration);
        loadingProgress.Complete();
    }

    private static IEnumerator LoadingError(string error, LoadingProgress loadingProgress)
    {
         loadingProgress.Status = error;
         yield return new WaitForSeconds(errorDisplayDuration);
         loadingProgress.Complete();
    }
}