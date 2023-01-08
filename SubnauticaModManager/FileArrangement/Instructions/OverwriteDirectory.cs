namespace FileArranger.Instructions;

[Serializable]
public class OverwriteDirectory : Instruction
{
    public string target;

    public string destination;

    public override Result Execute()
    {
        target = Path.GetFullPath(target);
        destination = Path.GetFullPath(destination);
        if (!Directory.Exists(target)) return Result.InvalidPath;
        if (!Directory.Exists(destination))
        {
            Directory.Move(target, destination);
            return Result.Success;
        }
        Utils.MoveDirectoryAndOverwriteAllFiles(target, destination);
        return Result.Success;
    }

    public OverwriteDirectory(string target, string destination)
    {
        this.target = target;
        this.destination = destination;
    }
}
