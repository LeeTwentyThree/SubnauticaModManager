using System.Diagnostics;

namespace ModManagerFileArranger;

internal static class SNLoader
{
    public static void LoadGame(SNPlatform platform)
    {
        switch (platform)
        {
            default: return;
            case SNPlatform.Steam:
                Process.Start(@"steam://rungameid/264710");
                return;
        }
    }
}
