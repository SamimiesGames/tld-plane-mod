namespace TLD_PlaneMod;

public static class PlaneModLoggerSettings
{
    public static bool SILENT = false;
    public static bool VERBOSE = true;
}

public class PlaneModLogger
{
    public static void MsgVerbose(string message)
    {
        if (PlaneModLoggerSettings.SILENT) return;
        if (!PlaneModLoggerSettings.VERBOSE) return;
        
        Melon<PlaneMod>.Logger.Msg(message);
    }
    
    public static void Msg(string message)
    {
        if (PlaneModLoggerSettings.SILENT) return;
        
        Melon<PlaneMod>.Logger.Msg(message);
    }

    public static void WarnMissingFile(string fileName)
    {
        string message = $"{fileName} is missing!!! It's either not shipped or has been removed!";
        Warn(message);
    }
    
    public static void Warn(string message)
    {
        if (PlaneModLoggerSettings.SILENT) return;
        
        Melon<PlaneMod>.Logger.Warning(message);
    }
    
    public static void Error(string message)
    {
        if (PlaneModLoggerSettings.SILENT) return;
        
        Melon<PlaneMod>.Logger.Error(message);
    }
}