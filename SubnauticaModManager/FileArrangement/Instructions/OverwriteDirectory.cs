namespace FileArranger.Instructions;

[Serializable]
public class OverwriteDirectory : Instruction
{
    public string target;

    public string destination;

    public override Result Execute()
    {
        if (!Directory.Exists(destination))
        {
            Directory.Move(target, destination);
            return Result.Success;
        }
        return Result.NotImplemented;
    }

    public OverwriteDirectory(string target, string destination)
    {
        this.target = target;
        this.destination = destination;
    }
}
