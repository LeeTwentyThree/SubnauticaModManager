﻿namespace FileArranger;

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
}
