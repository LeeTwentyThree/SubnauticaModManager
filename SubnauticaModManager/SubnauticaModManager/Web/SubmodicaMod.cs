using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace SubnauticaModManager.Web;

[System.Serializable]
public class SubmodicaMod
{
    [JsonProperty]
    private string url;
    [JsonProperty]
    private string profile_image;
    [JsonProperty]
    private string title;
    [JsonProperty]
    private string creator;
    [JsonProperty]
    private string tagline;
    [JsonProperty]
    private int views;
    [JsonProperty]
    private int downloads;
    [JsonProperty]
    private int likes;
    [JsonProperty]
    private string latest_version;
    [JsonProperty]
    private string subnautica_compatibility;
    [JsonProperty]
    private string created_at;
    [JsonProperty]
    private string updated_at;

    public string Url => url;
    public Sprite ModImageSprite { get; private set; }
    public string Title => title;
    public string Creator => creator;
    public string Tagline => tagline;
    public string GetViewsString()
    {
        return FormatInteger(views);
    }
    public string GetDownloadsString()
    {
        return FormatInteger(downloads);
    }
    public string GetFavoritesString()
    {
        return FormatInteger(likes);
    }
    public string LatestVersion => latest_version;
    public string SubnauticaCompatibility => subnautica_compatibility;
    public string DateCreated => created_at;
    public string DateUpdated => updated_at;

    public string GetVersionWithTimestampsString()
    {
        return $"v{LatestVersion} - Last Updated {DateUpdated} (Added {DateCreated})";
    }

    private string FormatInteger(int value)
    {
        if (value > 1000)
        {
            float thousandsPlace = value / 1000f;
            return thousandsPlace.ToString("F1");
        }
        return value.ToString();
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(url);
    }

    public IEnumerator LoadData()
    {
        if (!string.IsNullOrEmpty(profile_image))
        {
            using (var request = UnityWebRequestTexture.GetTexture(profile_image, false))
            {
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
