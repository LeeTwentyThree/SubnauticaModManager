using System;
using System.Security.Cryptography;

namespace SubnauticaModManager.Web;

internal static class SubmodicaGUIDs
{
    public static string GetChecksum(string filename)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filename))
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
