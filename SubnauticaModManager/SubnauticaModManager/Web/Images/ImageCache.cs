using System.Collections;
using UnityEngine.Networking;

namespace SubnauticaModManager.Web.Images;

internal static class ImageCache
{
    public const int standardIconWidth = 256;

    private static Dictionary<string, CacheEntry> cacheEntries = new Dictionary<string, CacheEntry>();

    public static string GetFileNameFromImageURL(string imageUrl, string extension = "png") // example for a valid imageUrl argument : "https://[sitename].net/mods/93-1671231004.webp"
    {
        if (imageUrl == null) throw new System.NullReferenceException();
        var split = imageUrl.Split('/');
        if (split.Length < 1) throw new System.ArgumentException();
        var fileName = split.Last();
        if (!string.IsNullOrEmpty(extension)) fileName = fileName.Replace("webp", extension);
        return fileName;
    }

    public static string GetPathInCache(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) throw new System.ArgumentException("Empty string used for image file name in cache!");
        return Path.Combine(FileManagement.ImageCacheFolder, fileName);
    }

    public static bool TryGetCachedWebImage(string url, out CacheEntry found)
    {
        var fileName = GetFileNameFromImageURL(url);
        if (cacheEntries.TryGetValue(fileName, out var existingEntry))
        {
            found = existingEntry;
            return true;
        }
        var path = GetPathInCache(fileName);
        if (File.Exists(path))
        {
            found = new CacheEntry();
            found.texture = new Texture2D(2, 2);
            found.texture.LoadImage(File.ReadAllBytes(path));
            found.sprite = Sprite.Create(found.texture, new Rect(0.0f, 0.0f, found.texture.width, found.texture.height), new Vector2(0.5f, 0.5f));
            cacheEntries.Add(fileName, found);
            return true;
        }
        found = null;
        return false;
    }

    public static IEnumerator GetOrAddWebpToCache(string url, CacheEntry result, int squareSize = standardIconWidth)
    {
        if (TryGetCachedWebImage(url, out CacheEntry found))
        {
            result.texture = found.texture;
            result.sprite = found.sprite;
            yield break;
        }
        else
        {
            using (var request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();
                try
                {
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
                            if (squareSize > 1)
                            {
                                //tex.Resize(squareSize, squareSize);
                            }
                            tex.Apply();
                            result.texture = tex;
                            result.sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));

                            var fileName = GetFileNameFromImageURL(url);

                            cacheEntries.Add(fileName, result);
                            tex.SavePNG(GetPathInCache(fileName));
                        }
                    }
                }
                catch
                {
                    Plugin.Logger.LogWarning($"Failed to load image with URL {url}.");
                    result.sprite = null;
                }
            }
        }
    }

    public class CacheEntry
    {
        public Texture2D texture;
        public Sprite sprite;
    }
}
