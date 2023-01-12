namespace SubnauticaModManager.Web;

public class SubmodicaSearchResult
{
    private Data data;

    public void SetData(Data newData)
    {
        data = newData;
    }

    public bool Success => data.success;
    public string ResultMessage => data.message;
    public SubmodicaMod[] Mods => data.mods;

    public bool ValidResults => data != null && data.success == true && data.mods != null && data.mods.Length > 0;

    [System.Serializable]
    public class Data
    {
        public bool success;
        public string message;
        public SubmodicaMod[] mods;
    }
}
