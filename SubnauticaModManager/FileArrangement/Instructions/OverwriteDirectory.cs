namespace FileArranger.Instructions;

[Serializable]
public class OverwriteDirectory : Instruction
{
    public string target;

    public string destination;

    public override Result Execute()
    {
        
    }

    public OverwriteDirectory(string target, string destination)
    {
        this.target = target;
        this.destination = destination;
    }
}
