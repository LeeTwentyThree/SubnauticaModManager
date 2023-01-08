namespace FileArranger;

public static class Utils
{
    public static bool IsFileLocked(FileInfo file)
    {
        try
        {
            using (FileStream stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
            {
                stream.Close();
            }
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return true;
        }

        //file is not locked
        return false;
    }

    public static void MoveDirectoryAndOverwriteAllFiles(string original, string targetDirectory)
    {
        if (!Directory.Exists(targetDirectory)) Directory.CreateDirectory(targetDirectory);

        foreach (string dirPath in Directory.GetDirectories(original, "*", SearchOption.AllDirectories))
        {
            var directoryLocation = dirPath.Replace(original, targetDirectory);
            if (!Directory.Exists(directoryLocation)) Directory.CreateDirectory(directoryLocation);
        }

        foreach (string newPath in Directory.GetFiles(original, "*.*", SearchOption.AllDirectories))
        {
            File.Copy(newPath, newPath.Replace(original, targetDirectory), true);
        }
    }
}
