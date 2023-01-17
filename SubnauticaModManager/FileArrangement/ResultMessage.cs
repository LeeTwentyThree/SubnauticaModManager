namespace FileArranger;

public struct ResultMessage
{
    public Result result;
    public string message;

    public ResultMessage(Result result, string message)
    {
        this.result = result;
        this.message = message;
    }

    public ResultMessage(Result result)
    {
        this.result = result;
        message = "None";
    }

    public override string ToString()
    {
        return $"Result: {result}\nMessage: {message}";
    }
}