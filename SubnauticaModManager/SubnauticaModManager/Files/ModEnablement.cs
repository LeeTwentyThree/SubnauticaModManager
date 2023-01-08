using FileArranger.Instructions;

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
            if (entries[i].pluginData != null && entries[i].pluginData.GUID == mod.GUID)
            {
                return entries[i].enabled;
            }
        }
        return mod.Installed;
    }

    public static List<Instruction> GetInstructionsForEnablement()
    {
        var list = new List<Instruction>();
        foreach (var entry in entries)
        {
            if (entry.pluginData == null) continue;
            if (entry.pluginData.Installed && entry.enabled) continue;
            if (!entry.pluginData.Installed && !entry.enabled) continue;
            var sourceDirectory = entry.pluginData.ContainingFolder;
            var sourceDirectoryName = new DirectoryInfo(sourceDirectory).Name;
            string targetDirectory;
            if (entry.enabled)
            {
                targetDirectory = Path.Combine(FileManagement.BepInExPluginsFolder, sourceDirectoryName);
            }
            else
            {
                targetDirectory = Path.Combine(FileManagement.DisabledModsFolder, sourceDirectoryName);
            }
            list.Add(new OverwriteDirectory(sourceDirectory, targetDirectory));
        }
        return list;
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
