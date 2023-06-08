namespace SubnauticaModManager.Files;

// this class manages any potential custom downloads locations
internal static class CustomDownloadsFolder
{
    public static string CurrentModDownloadFolder
    {
        get
        {
            return FileManagement.DefaultModDownloadFolder;
        }
    }
}
