namespace TLD_PlaneMod;

public class PlaneModManager
{
    public static PlaneModManager Singleton;
    public string lastRegionName;
    
    
    public PlaneModManager()
    {
        if (Singleton != null) return;
        Singleton = this;
        
        PlaneModLogger.Msg($"[PlaneModManager] Setup");
        
        new AircraftManager();
        new PlaneModDataManager();
        new PlaneModAssetManager();
        
        UnityBundleManager.Singleton.LoadAssetBundle(
            PlaneModSettings.PLANEMOD_BUNDLENAME, 
            PlaneModSettings.ASSETBUNDLE_PATH
        );
        
        PlaneModLogger.Msg($"[PlaneModManager] Initialized");
    }

    public void Update()
    {
        if (GameManager.GetUniStorm().m_CurrentRegion)
        {
            string regionName = GameManager.m_ActiveScene;

            if (regionName != lastRegionName)
            {
                PlaneModDataManager.Singleton.UpdateModelStreaming(regionName);
                lastRegionName = regionName;
            }
        }
        AircraftManager.Singleton.Update(Time.deltaTime);
    }
}