using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubnauticaModManager.Files;

internal static class ModEnablement
{
    private static List<Entry> entries = new List<Entry>();

    public static void SetModEnable(PluginData mod, bool enabled)
    {
        ModArrangement.UrgeGameRestart(false);
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].pluginData.GUID == mod.GUID)
            {
                entries[i].enabled = enabled;
                return;
            }
        }
        entries.Add(new Entry(mod, enabled));
    }

    public static bool GetEnableState(PluginData mod)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i].pluginData.GUID == mod.GUID)
            {
                return entries[i].enabled;
            }
        }
        return mod.Installed;
    }

    public class Entry
    {
        public PluginData pluginData;
        public bool enabled;

        public Entry(PluginData mod, bool newEnabledState)
        {
            this.pluginData = mod;
            this.enabled = newEnabledState;
        }
    }
}
