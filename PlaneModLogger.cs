namespace TLD_PlaneMod;

public static class PlaneModLoggerSettings
{
    public static bool SILENT = false;
    public static bool VERBOSE = true;
}

public class PlaneModLogger
{
    public static void MsgHUD(string message)
    {
        Panel_HUD HUD;
        InterfaceManager.TryGetPanel<Panel_HUD>(out HUD);
        
        if (!HUD) Warn($"[PlaneModLogger] Panel_HUD NOT FOUND!");
        
        HUDMessage.HUDMessageInfo messageInfo = new HUDMessage.HUDMessageInfo();
        messageInfo.m_Text = message;
        messageInfo.m_DisplayTime = 5f;
        HUDMessage.ShowMessage(HUD, messageInfo);
    }
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