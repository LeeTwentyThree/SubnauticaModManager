using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubnauticaModManager.Files;

internal class PluginData
{
    public string dllPath;

    public string FolderPath => Path.Combine(dllPath, "../");

    public string GUID;
    public string Version;
    public string Name;

    public PluginData(string dllPath, string gUID, string version, string name)
    {
        this.dllPath = dllPath;
        GUID = gUID;
        Version = version;
        Name = name;
    }
}
