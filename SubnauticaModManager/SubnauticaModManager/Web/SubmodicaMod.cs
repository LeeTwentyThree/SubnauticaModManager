using System.Collections;
using UnityEngine.Networking;
using Newtonsoft.Json;
using SubnauticaModManager.Web.Images;
using System.Drawing;

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
            using (var request = UnityWebRequest.Get(profile_image))
            {
                yield return request.SendWebRequest();
                var data = request.downloadHandler.data;
                if (data != null)
                {
                    WebP image = new WebP();
                    var bitmap = image.Decode(data, new WebPDecoderOptions(), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    if (bitmap != null && bitmap.Width > 0 && bitmap.Height > 0)
                    {
                        Texture2D tex = new Texture2D(2, 2);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitmap.Save(ms, bitmap.RawFormat);
                            tex.LoadImage(ms.ToArray());
                        }
                        tex.Apply();
                        ModImageSprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    }
                }
            }
        }
    }
}
