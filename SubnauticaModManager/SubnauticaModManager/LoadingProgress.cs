﻿namespace SubnauticaModManager;

public class LoadingProgress
{
    public static LoadingProgress current;

    public string Status { get; set; }
    public float Progress { get; set; }

    public LoadingProgress()
    {
        SetActive();
    }

    public void Complete()
    {
        current = null;
    }

    public void SetActive()
    {
        if (current != null && current != this)
        {
            Plugin.Logger.LogError("TWO INSTANCES OF THE LOADINGPROGRESS CLASS EXIST AT ONCE!");
        }
        current = this;
    }

    public static bool Busy { get { return current != null; } }

    public static void CancelAll()
    {
        current = null;
    }
}
