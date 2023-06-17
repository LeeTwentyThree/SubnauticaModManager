using System;
using System.Collections;
using Newtonsoft.Json;
using SubnauticaModManager.Web.Images;

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

    // The GUID is not loaded by default. However this field is assigned when you search a mod with a specific GUID. It's a bit hacky.
    // As of writing this, it is only used for version checking.
    private string guid;
    
    public void SetGUID(string guid)
    {
        this.guid = guid;
    }

    public bool TryGetGUID(out string guid)
    {
        guid = this.guid;
        if (this.guid != null)
        {
            return true;
        }
        return false;
    }

    public string GetVersionWithTimestampsString()
    {
        return Translation.TranslateFormat("SubmodicaVersionWithTimestamp", LatestVersion, DateUpdated, DateCreated);
    }

    public Version LatestVersionNumber
    {
        get
        {
            if (string.IsNullOrEmpty(latest_version)) return Version.Zero;
            if (Version.TryParse(latest_version, out Version version)) return version;
            return Version.Zero;
        }
    }

    private string FormatInteger(int value)
    {
        if (value > 1000)
        {
            float thousandsPlace = value / 1000f;
            return Translation.TranslateFormat("ThousandsPlaceFormat", thousandsPlace.ToString("F1"));
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
            var cacheEntry = new ImageCache.CacheEntry();
            yield return ImageCache.GetOrAddWebpToCache(profile_image, cacheEntry);
            ModImageSprite = cacheEntry.sprite;
        }
    }

    public bool IsForCorrectVersion()
    {
        return subnautica_compatibility == "2.0";
    }

    /// <summary>
    /// Constructor to be used for manual creation of mods. This is generally done for testing purposes.
    /// </summary>
    public SubmodicaMod(string url, string profile_image, string title, string creator, string tagline,int views, int downloads, int likes, string latest_version,
        string subnautica_compatibility, string created_at, string updated_at)
    {
        this.url = url;
        this.profile_image = profile_image;
        this.title = title;
        this.creator = creator;
        this.tagline = tagline;
        this.views = views;
        this.downloads = downloads;
        this.likes = likes;
        this.latest_version = latest_version;
        this.subnautica_compatibility = subnautica_compatibility;
        this.created_at = created_at;
        this.updated_at = updated_at;
    }
}
