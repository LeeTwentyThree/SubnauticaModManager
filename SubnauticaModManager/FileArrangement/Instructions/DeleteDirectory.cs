namespace FileArranger.Instructions;

[Serializable]
public class DeleteDirectory : Instruction
{
    public string directoryToDelete;

    public override Result Execute()
    {
        Directory.Delete(directoryToDelete, true);
        return Result.Success;
    }

    public DeleteDirectory(string fileToDelete)
    {
        this.directoryToDelete = fileToDelete;
    }
}
