using System;

namespace SubnauticaModManager.Files;

internal class PluginData
{
    public string dllPath;

    public string ContainingFolder => Path.Combine(dllPath, "../");

    public string GUID;
    public Version Version;
    public string Name;
    public readonly PluginLocation Location;

    public PluginData(string dllPath, string gUID, Version version, string name, PluginLocation location)
    {
        this.dllPath = dllPath;
        GUID = gUID;
        Version = version;
        Name = name;
        Location = location;
    }

    public bool IsValid => !string.IsNullOrEmpty(dllPath) && File.Exists(dllPath);

    public bool Installed => Location == PluginLocation.Plugins;

    public string StatusText
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

    public bool Equals(PluginData other)
    {
        if (other == null) return false;
        return other.GUID == GUID;
    }
}
