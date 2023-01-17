using FileArranger.Instructions;
using Newtonsoft.Json;

namespace FileArranger;

[Serializable]
public class InstructionSet
{
    public Instruction[] instructions;

    private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All, Formatting = Formatting.Indented };

    private string _path;

    public InstructionSet(string jsonFilePath)
    {
        _path = jsonFilePath;
    }

    public Result SaveToDisk()
    {
        if (instructions == null) return Result.NoInstructions;
        if (string.IsNullOrEmpty(_path)) return Result.InvalidPath;

        try
        {
            File.WriteAllText(_path, SerializeToString());
        }
        catch
        {
            return Result.Exception;
        }

        return Result.Success;
    }

    public ResultMessage[] ExecuteInstructions()
    {
        if (string.IsNullOrEmpty(_path)) return new ResultMessage[] { new ResultMessage(Result.InvalidPath) };
        if (!File.Exists(_path)) return new ResultMessage[] { new ResultMessage(Result.FileNotFound) };

        try
        {
            var contents = File.ReadAllText(_path);
            Deserialize(contents);
        }
        catch (Exception e)
        {
            return new ResultMessage[] { new ResultMessage(Result.Exception, e.ToString()) };
        }

        if (instructions == null) return new ResultMessage[] { new ResultMessage(Result.NoInstructions) };
        return ExecuteAll();
    }

    private string SerializeToString()
    {
        return JsonConvert.SerializeObject(instructions, _serializerSettings);
    }

    private void Deserialize(string from)
    {
        instructions = JsonConvert.DeserializeObject<Instruction[]>(from, _serializerSettings);
    }

    private ResultMessage[] ExecuteAll()
    {
        ResultMessage[] results = new ResultMessage[instructions.Length];
        for (int i = 0; i < instructions.Length; i++)
        {
            if (instructions[i] == null)
            {
                results[i] = new ResultMessage(Result.NoInstructions);
            }
            else
            {
                try
                {
                    results[i] = new ResultMessage(instructions[i].Execute());
                }
                catch (Exception e)
                {
                    results[i] = new ResultMessage(Result.Exception, e.ToString());
                }
            }
        }
        return results;
    }
}