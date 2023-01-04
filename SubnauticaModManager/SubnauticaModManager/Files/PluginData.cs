using System;

namespace SubnauticaModManager.Files;

internal class PluginData
{
    public string dllPath;

    public string FolderPath => Path.Combine(dllPath, "../");

    public string GUID;
    public Version Version;
    public string Name;
    public PluginLocation Location;

    public PluginData(string dllPath, string gUID, Version version, string name, PluginLocation location)
    {
        this.dllPath = dllPath;
        GUID = gUID;
        Version = version;
        Name = name;
        Location = location;
    }

    public bool IsValid => !string.IsNullOrEmpty(dllPath) && File.Exists(dllPath);

    public string EnabledStateText
    {
        get
        {
            if (Location == PluginLocation.Plugins)
            {
                return "Enabled";
            }
            return "Disabled";
        }
    }
}
