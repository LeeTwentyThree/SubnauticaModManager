using System;

namespace SubnauticaModManager.Files;

public class NoPluginException : Exception
{
    public NoPluginException()
    {
    }

    public override string Message => "The specified mod folder does not contain a valid plugin file!";
}