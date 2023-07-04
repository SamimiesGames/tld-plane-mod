using MelonLoader.TinyJSON;

namespace TLD_PlaneMod;

public class PlaneModDataManager
{
    public static PlaneModDataManager Singleton;
    public PlaneModDataFixer dataFixer;
    public PlaneModData planeModData;

    public string CurrentDataPath
    {
        get
        {
            SaveSlotInfo saveSlotInfo = SaveGameSlotHelper.GetCurrentSaveSlotInfo();
            if (saveSlotInfo == null) return null;
            
            string path = $"{PlaneModSettings.BASE_DATAPATH}_{saveSlotInfo.m_SaveSlotName}";

            return path;
        }
    }

    public PlaneModDataManager()
    {
        if (Singleton != null) return;
        Singleton = this;
        
        PlaneModLogger.Msg($"[PlaneModDataManager] Setup");
        
        dataFixer = new PlaneModDataFixer();
        planeModData = new PlaneModData();

        PlaneModLogger.Msg($"[PlaneModDataManager] Initialized");
    }

    public void LoadData()
    {
        if (CurrentDataPath == null)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] Couldn't create CurrentDataPath, because CurrentSaveSlotInfo is null");
            PlaneModLogger.Warn($"[PlaneModDataManager] Aborted LoadData");
            return;
        }
        else
        {
            if(!File.Exists(CurrentDataPath)) PlaneModDataUtility.WriteDataWithBase(CurrentDataPath, PlaneModSettings.BASE_DATAPATH);
        }
        PlaneModLogger.MsgVerbose($"[PlaneModDataManager] LoadData");
        try
        {
            if (!File.Exists(CurrentDataPath))
            {
                PlaneModLogger.WarnMissingFile(CurrentDataPath);
                PlaneModLogger.Warn($"[PlaneModDataManager] Aborted LoadData");
                return;
            }
            
            planeModData = PlaneModDataUtility.ReadJson<PlaneModData>(CurrentDataPath);

            if (planeModData == null)
            {
                PlaneModLogger.Warn(
                    $"[PlaneModDataManager] Failed to decode data from {CurrentDataPath}");
                return;
            }
        }
        catch (FileNotFoundException)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] {CurrentDataPath} not found!");
        }
        catch (Exception)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] Something went wrong while loading {CurrentDataPath}");
        }
    }
}