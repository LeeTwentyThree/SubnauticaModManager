namespace FileArranger.Instructions;

[Serializable]
public class MoveDirectory : Instruction
{
    public string target;

    public string destination;

    public override Result Execute()
    {
        Directory.Move(target, destination);
        return Result.Success;
    }

    public MoveDirectory(string target, string destination)
    {
        this.target = target;
        this.destination = destination;
    }
}
