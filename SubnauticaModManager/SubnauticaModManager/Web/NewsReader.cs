using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

namespace SubnauticaModManager.Web;

internal static class NewsReader
{
    private const string newsUrl = "https://raw.githubusercontent.com/LeeTwentyThree/SubnauticaModManager/main/SubnauticaModManager/news.json";

    public const float errorDisplayDuration = 3f;

    public static IEnumerator ReadNews(LoadingProgress loadingProgress, NewsRequest result)
    {
        SoundUtils.PlaySound(UISound.Select);
        string text = "";
        using (var request = UnityWebRequest.Get(newsUrl))
        {
            request.timeout = 7;
            var operation = request.SendWebRequest();
            loadingProgress.Status = "Loading news...";
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
            loadingProgress.Status = "Error: No data loaded!";
            yield return new WaitForSeconds(errorDisplayDuration);
            loadingProgress.Complete();
            yield break;
        }

        var parsed = JObject.Parse(text, new JsonLoadSettings());
        JArray newsEntriesJson = parsed.Value<JArray>("data");
        NewsEntry[] newsEntries = newsEntriesJson.ToObject<NewsEntry[]>();

        result.data = newsEntries;

        loadingProgress.Status = "Success!";
        loadingProgress.Progress = 1f;
        yield return new WaitForSeconds(0.5f);
        loadingProgress.Complete();
    }

    private static IEnumerator LoadingError(string error, LoadingProgress loadingProgress, float duration = errorDisplayDuration)
    {
        loadingProgress.Status = error;
        yield return new WaitForSeconds(duration);
        loadingProgress.Complete();
    }
}
