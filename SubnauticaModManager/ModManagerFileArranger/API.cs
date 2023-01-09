using System.Diagnostics;

namespace ModManagerFileArranger;

public static class API
{
    public static void Run(string exePath, string instructionsPath, SNPlatform platform)
    {
        var process = new Process();
        process.StartInfo.FileName = exePath;
        process.StartInfo.Arguments = "\"" + instructionsPath + "\" " + (int)platform;
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;
        process.Start();
    }
}
