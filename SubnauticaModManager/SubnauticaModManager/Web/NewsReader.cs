using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

namespace SubnauticaModManager.Web;

internal static class NewsReader
{
    private const string newsUrl = "https://raw.githubusercontent.com/LeeTwentyThree/SubnauticaModManager/main/SubnauticaModManager/news.json";

    public const float errorDisplayDuration = 0.5f;

    public static IEnumerator ReadNews(LoadingProgress loadingProgress, NewsRequest result)
    {
        SoundUtils.PlaySound(UISound.Select);
        string text = "";
        using (var request = UnityWebRequest.Get(newsUrl))
        {
            request.timeout = 2;
            var operation = request.SendWebRequest();
            loadingProgress.Status = Translation.Translate("NewsLoading");
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
                yield return LoadingError(Translation.TranslateFormat("ErrorCode", request.responseCode), loadingProgress);
                yield break;
            }
            text = request.downloadHandler.text;
            yield return new WaitForSeconds(0.1f);
        }

        if (string.IsNullOrEmpty(text))
        {
            loadingProgress.Status = Translation.Translate("NoDataLoaded");
            yield return new WaitForSeconds(errorDisplayDuration);
            loadingProgress.Complete();
            yield break;
        }

        var parsed = JObject.Parse(text, new JsonLoadSettings());
        JArray newsEntriesJson = parsed.Value<JArray>("data");
        NewsEntry[] newsEntries = newsEntriesJson.ToObject<NewsEntry[]>();

        result.data = newsEntries;

        loadingProgress.Status = Translation.Translate("Success");
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
