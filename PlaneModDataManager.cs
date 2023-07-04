namespace TLD_PlaneMod;

public class PlaneModDataManager
{
    public static PlaneModDataManager Singleton;
    public PlaneModData planeModData;
    public PlaneModDataFixer dataFixer;

    public PlaneModDataManager()
    {
        if (Singleton != null) return;
        Singleton = this;
        Melon<PlaneMod>.Logger.Msg($"[PlaneModDataManager] Setup");

        dataFixer = new PlaneModDataFixer();
        dataFixer.FixMissingFile(PlaneModSettings.BASE_DATAPATH);
        
        LoadData();

        Melon<PlaneMod>.Logger.Msg($"[PlaneModDataManager] Initialized");
    }

    public void LoadData()
    {
        Melon<PlaneMod>.Logger.Msg($"[PlaneModDataManager] LoadData");
        try
        {
            planeModData = JsonUtility.FromJson<PlaneModData>(PlaneModSettings.BASE_DATAPATH);
            if (planeModData == null)
            {
                Melon<PlaneMod>.Logger.Warning($"[PlaneModDataManager] Failed to decode data from {PlaneModSettings.BASE_DATAPATH}");
                return;
            }
            else
            {
                planeModData = dataFixer.FixMissingOrBrokenData(planeModData);
            }
        }
        catch (Exception)
        {
            Melon<PlaneMod>.Logger.Warning($"[PlaneModDataManager] Something went wrong while loading {PlaneModSettings.BASE_DATAPATH}");
        }
    }
}