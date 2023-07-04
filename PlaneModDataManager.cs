namespace TLD_PlaneMod;

public class PlaneModDataManager
{
    public static PlaneModDataManager Singleton;
    public PlaneModData planeModData;

    public PlaneModDataManager()
    {
        if (Singleton != null) return;
        Singleton = this;

        Melon<PlaneMod>.Logger.Msg($"[PlaneModDataManager] Initialized");
    }

    public void LoadData()
    {
        Melon<PlaneMod>.Logger.Msg($"[PlaneModDataManager] LoadData");
        //planeModData = JsonUtility.FromJson<PlaneModData>(PlaneModDataUtility.FormatPlaneDataPath("0285052"));
    }
}