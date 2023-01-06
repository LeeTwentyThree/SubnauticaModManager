using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

namespace SubnauticaModManager.Web;

public static class SubmodicaAPI
{
    private const string key = "0XAWs3EuMW5PyvpLbFVf7DviL86QQPtP";

    private const string urlFormat = "https://submodica.xyz/api/search/{0}/{1}/{2}"; // 0 - game, 1 - token, 2 - url-encoded query

    private const int maxQueryLength = 128;

    private const float search_fakeLoadDuration = 0.5f;

    public const float errorDisplayDuration = 0.5f;

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
        return string.Format(urlFormat, Game.Current, key, System.Uri.EscapeUriString(query.ToLower()));
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
        SoundUtils.PlaySound(UISound.Select);
        float timeStarted = Time.realtimeSinceStartup;
        var url = GetSearchURL(query);
        string text = "";
        using (var request = UnityWebRequest.Get(url))
        {
            request.timeout = 20;
            var operation = request.SendWebRequest();
            loadingProgress.Status = "Fetching search results...";
            while (!operation.isDone)
            {
                yield return null;
                loadingProgress.Progress = operation.progress;
            }
            if (!string.IsNullOrEmpty(request.error))
            {
                yield return LoadingError(request.error, loadingProgress);
                yield break;
            }
            if (request.responseCode != 200)
            {
                yield return LoadingError("Error code: " + request.responseCode, loadingProgress);
                yield break;
            }
            text = request.downloadHandler.text;
            yield return new WaitForSeconds(0.1f);
        }

        if (string.IsNullOrEmpty(text))
        {
            loadingProgress.Status = "No data loaded!";
            yield return new WaitForSeconds(errorDisplayDuration);
            loadingProgress.Complete();
            yield break;
        }

        loadingProgress.Progress = 0;
        loadingProgress.Status = "Loading mods...";

        var parsed = JObject.Parse(text, new JsonLoadSettings());
        var resultData = new SubmodicaSearchResult.Data();
        resultData.success = parsed.Value<bool>("success");
        resultData.message = parsed.Value<string>("message");
        JArray modsJson = parsed.Value<JArray>("data");
        SubmodicaMod[] mods = modsJson.ToObject<SubmodicaMod[]>();
        resultData.mods = mods;
        result.SetData(resultData);

        if (!result.Success)
        {
            yield return LoadingError("Search failed!", loadingProgress);
            yield break;
        }
        if (!result.ValidResults)
        {
            yield return LoadingError("No results!", loadingProgress, 1f);
            loadingProgress.Progress = 1;
            yield break;
        }

        for (int i = 0; i < result.Mods.Length; i++)
        {
            if (result.Mods[i] != null)
            {
                yield return result.Mods[i].LoadData();
                loadingProgress.Progress = (float)(i + 1) / result.Mods.Length;
            }
        }

        loadingProgress.Status = "Success!";
        loadingProgress.Progress = 1f;
        if (Time.realtimeSinceStartup - timeStarted > 1) yield return new WaitForSeconds(search_fakeLoadDuration);
        loadingProgress.Complete();
    }

    private static IEnumerator LoadingError(string error, LoadingProgress loadingProgress, float duration = errorDisplayDuration)
    {
         loadingProgress.Status = error;
         yield return new WaitForSeconds(duration);
         loadingProgress.Complete();
    }
}