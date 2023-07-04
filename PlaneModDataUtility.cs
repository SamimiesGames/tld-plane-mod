using MelonLoader.TinyJSON;

namespace TLD_PlaneMod;

public class PlaneModDataUtility
{
    public static T ReadJson<T>(string path)
    {
        string content = ReadText(path);
        T type = JSON.Load(content).Make<T>();
        
        return type;
    }
    
    public static Variant ReadJsonRaw(string path)
    {
        string content = ReadText(path);
        Variant variant = JSON.Load(content);
        
        return variant;
    }
    
    public static string ReadText(string path)
    {
        string content = File.ReadAllText(path);
        return content;
    }

    public static void WriteData(string path, string data)
    {
        PlaneModLogger.MsgVerbose($"[PlaneModDataUtility] WriteData path={path}, data.Length={data.Length}");
        using (StreamWriter writer = new StreamWriter(path, false))
        {
            writer.Write(data);
            writer.Close();
        }
    }
    
    public static void WriteDataWithBase(string path, string basePath)
    {
        PlaneModLogger.MsgVerbose($"[PlaneModDataUtility] WriteDataWithBase path={path}, basePath={basePath}");
        
        string baseContent = ReadText(basePath);
        if(!File.Exists(path)) File.Create(path).Close();
        
        WriteData(path, baseContent);
    }
}