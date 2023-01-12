using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;
using SubnauticaModManager.Files;

namespace SubnauticaModManager.Web;

public static class SubmodicaAPI
{
    private const string key = "0XAWs3EuMW5PyvpLbFVf7DviL86QQPtP";

    private const string urlFormat = "https://submodica.xyz/api/search/{0}/{1}/{2}"; // 0 - game, 1 - token, 2 - url-encoded query

    private const string checkUpdatesUrl = "https://submodica.xyz/api/getVersions";

    private const string recordGUIDUrl = "https://submodica.xyz/api/recordGuid";

    private const string getByGUIDUrl = "https://submodica.xyz/api/getByGuid";

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

    private static IEnumerator LoadingErrorDontCloseBar(string error, LoadingProgress loadingProgress, float duration = errorDisplayDuration)
    {
        loadingProgress.Status = error;
        yield return new WaitForSeconds(duration);
    }

    public static void RecordGUIDToSubmodica(string checksum, string guid)
    {
        UWE.CoroutineHost.StartCoroutine(RecordGUIDToSubmodicaInternal(checksum, guid));
    }

    private static IEnumerator RecordGUIDToSubmodicaInternal(string checksum, string guid)
    {
        Dictionary<string, string> postData = new Dictionary<string, string>()
        {
            { "token", key },
            { "checksum", checksum },
            { "guid", guid }
        };
        using (var request = UnityWebRequest.Post(recordGUIDUrl, postData))
        {
            request.timeout = 2;
            var operation = request.SendWebRequest();
            yield return operation;
        }
    }

    public static IEnumerator SearchForUpdates(LoadingProgress progress, List<SubmodicaSearchResult> results, GenericStatusReport status)
    {
        SoundUtils.PlaySound(UISound.Select);
        var list = KnownPlugins.list;
        if (list == null) yield break;

        progress.Status = "Checking for updates...";

        for (int i = 0; i < list.Count; i++)
        {
            var known = list[i];
            if (!known.IsValid) continue;
            var singleResult = new SubmodicaSearchResult();
            yield return SearchModsByGUID(known.GUID, singleResult, status);
            if (singleResult != null && singleResult.ValidResults)
            {
                results.Add(singleResult);
            }
            progress.Progress = (float)i / list.Count;
            progress.Status = $"Checking for updates...\nChecked {i} out of {list.Count}";
        }

        progress.Progress = 1;
        progress.Status = $"Success!\nChecked {status.successes} mod(s).\n{status.failures} mod(s) cannot be automatically checked.\n";
        SoundUtils.PlaySound(UISound.Select);
        yield return new WaitForSeconds(1f);
        progress.Complete();
    }

    public static IEnumerator SearchModsByGUID(string guid, SubmodicaSearchResult result, GenericStatusReport status)
    {
        Dictionary<string, string> postData = new()
        {
            { "token", key },
            { "guid", guid }
        };
        string text = "";
        using (var request = UnityWebRequest.Post(getByGUIDUrl, postData))
        {
            request.timeout = 2;
            var operation = request.SendWebRequest();
            yield return operation;
            if (!string.IsNullOrEmpty(request.error))
            {
                status.failures++;
                yield break;
            }
            if (request.responseCode != 200)
            {
                status.failures++;
                yield break;
            }
            text = request.downloadHandler.text;
        }

        if (string.IsNullOrEmpty(text))
        {
            status.failures++;
            yield break;
        }

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
            status.failures++;
            yield break;
        }
        if (!result.ValidResults)
        {
            status.failures++;
            yield break;
        }

        for (int i = 0; i < result.Mods.Length; i++)
        {
            if (result.Mods[i] != null)
            {
                result.Mods[i].SetGUID(guid);
                yield return result.Mods[i].LoadData();
            }
        }
        status.successes++;
    }
}