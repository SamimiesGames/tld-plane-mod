namespace TLD_PlaneMod;

public class PlaneModManager
{
    public static PlaneModManager Singleton;
    
    public PlaneModManager()
    {
        if (Singleton != null) return;
        Singleton = this;
        
        PlaneModLogger.Msg($"[PlaneModManager] Setup");
        
        new AircraftManager();
        new PlaneModDataManager();
        new PlaneModAssetManager();
        
        PlaneModLogger.Msg($"[PlaneModManager] Initialized");
    }
}