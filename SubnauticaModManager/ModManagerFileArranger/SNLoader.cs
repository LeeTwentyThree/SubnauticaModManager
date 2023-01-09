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
            case SNPlatform.Epic:
                Process.Start(@"com.epicgames.launcher://apps/jaguar%3A3257e06c28764231acd93049f3774ed6%3AJaguar?action=launch&silent=true");
                return;
        }
    }
}
