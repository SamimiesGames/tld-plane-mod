namespace TLD_PlaneMod;

public class PlaneModManager
{
    public static PlaneModManager Singleton;
    
    public PlaneModManager()
    {
        if (Singleton != null) return;
        Singleton = this;
        
        Melon<PlaneMod>.Logger.Msg($"[PlaneModManager] Setting up");
        
        new AircraftManager();
        new PlaneModDataManager();
        
        Melon<PlaneMod>.Logger.Msg($"[PlaneModManager] Initialized");
    }
}