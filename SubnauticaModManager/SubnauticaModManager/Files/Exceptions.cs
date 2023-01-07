using System;

namespace SubnauticaModManager.Files;

public class NoPluginsException : Exception
{
    public NoPluginsException()
    {
    }

    public override string Message => "The specified mod folder does not contain a valid plugin file!";
}