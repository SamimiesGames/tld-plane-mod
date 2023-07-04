using MelonLoader.TinyJSON;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TLD_PlaneMod;

public enum PlaneModDataLoadState
{
    UnLoaded,
    Loaded
}

public class PlaneModDataManager
{
    public static PlaneModDataManager Singleton;
    public PlaneModDataFixer dataFixer;
    public PlaneModData planeModData;

    public PlaneModDataLoadState dataLoadState;
    public string lastSceneGUID;

    public string CurrentDataPath
    {
        get
        {
            string path = $"{PlaneModSettings.BASE_DATAPATH}";
            /*
            if (string.IsNullOrEmpty(SaveGameSystem.m_CurrentSaveName))
            {
                PlaneModLogger.Warn($"[PlaneModDataManager] CurrentDataPath is null!");
                return null;
            }
            else
            {
                path += $"\\{SaveGameSystem.m_CurrentSaveName}";
            }
            */
            
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
        dataLoadState = PlaneModDataLoadState.UnLoaded;
        
        PlaneModLogger.Msg($"[PlaneModDataManager] Initialized");
    }

    public void UpdateModelStreaming(float timeDelta)
    {
        if (!SceneManager.GetActiveScene().isLoaded || SceneManager.GetActiveScene().guid == lastSceneGUID) return;
        
        if(dataLoadState == PlaneModDataLoadState.UnLoaded) LoadData();
        
        PlaneModLogger.Msg($"[PlaneModDataManager] UpdateModelStreaming dataLoadState={dataLoadState}, guid={SceneManager.GetActiveScene().guid}");

        AircraftManager.Singleton.UnLoadAll();
        
        lastSceneGUID = SceneManager.GetActiveScene().guid;
    }

    public void UpdateAircraftData()
    {
        PlaneModLogger.MsgVerbose($"[PlaneModDataManager] UpdateAircraftData airCrafts.Count={AircraftManager.Singleton.aircrafts.Count}");
        int dataInstancesUpdated = 0;
        foreach (var aircraft in AircraftManager.Singleton.aircrafts)
        {
            bool found = false;
            PlaneModAircraftData aircraftData = null;
            foreach (var data in planeModData.aircraftData)
            {
                if (data.guid == aircraft.guid)
                {
                    aircraftData = data;
                    found = true;
                }
            }

            if (!found) continue;

            aircraftData.position = new float[3]
            {
                aircraft.planeGameObject.transform.position.x,
                aircraft.planeGameObject.transform.position.y,
                aircraft.planeGameObject.transform.position.z,
            };
            aircraftData.rotation = new float[4]
            {
                aircraft.planeGameObject.transform.rotation.x,
                aircraft.planeGameObject.transform.rotation.y,
                aircraft.planeGameObject.transform.rotation.z,
                aircraft.planeGameObject.transform.rotation.w,
            };
            aircraftData.guidance = new float[3]
            {
                aircraft.guidance.x,
                aircraft.guidance.y,
                aircraft.guidance.z,
            };
            aircraftData.angularVelocity = new float[3]
            {
                aircraft.angularVelocity.x,
                aircraft.angularVelocity.y,
                aircraft.angularVelocity.z
            };
            aircraftData.velocity = new float[3]
            {
                aircraft.velocity.x,
                aircraft.velocity.y,
                aircraft.velocity.z
            };
            dataInstancesUpdated++;
        }
        
        PlaneModLogger.MsgVerbose($"[PlaneModDataManager] dataInstancesUpdated={dataInstancesUpdated}");
    }

    public void SaveData()
    {
        if (CurrentDataPath == null)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] CurrentSaveSlotInfo is null");
            PlaneModLogger.Warn($"[PlaneModDataManager] Aborted SaveData");
            return;
        }
        else
        {
            if(!File.Exists(CurrentDataPath)) PlaneModDataUtility.WriteDataWithBase(CurrentDataPath, PlaneModSettings.BASE_DATAPATH);
        }
        PlaneModLogger.Msg($"[PlaneModDataManager] SaveData");
        try
        {
            if (!File.Exists(CurrentDataPath))
            {
                PlaneModLogger.WarnMissingFile(CurrentDataPath);
                PlaneModLogger.Warn($"[PlaneModDataManager] Aborted SaveData");
                return;
            }

            UpdateAircraftData();
            PlaneModLogger.MsgVerbose($"[PlaneModDataManager] SaveData aircraftData to be saved {planeModData.aircraftData.Length}");
            string jsonData = JSON.Dump(planeModData.aircraftData);
            PlaneModDataUtility.WriteData(CurrentDataPath, jsonData);
        }
        catch (FileNotFoundException)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] {CurrentDataPath} not found!");
        }
        catch (Exception)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] Something went wrong while saving in {CurrentDataPath}. This could be that data is stored in a wrong datamodel!");
        }
    }
    
    public void LoadData()
    {
        if (CurrentDataPath == null)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] CurrentSaveSlotInfo is null");
            dataLoadState = PlaneModDataLoadState.UnLoaded;
            PlaneModLogger.Warn($"[PlaneModDataManager] Aborted LoadData");
            return;
        }
        else
        {
            if(!File.Exists(CurrentDataPath)) PlaneModDataUtility.WriteDataWithBase(CurrentDataPath, PlaneModSettings.BASE_DATAPATH);
        }
        PlaneModLogger.Msg($"[PlaneModDataManager] LoadData");
        try
        {
            if (!File.Exists(CurrentDataPath))
            {
                PlaneModLogger.WarnMissingFile(CurrentDataPath);
                PlaneModLogger.Warn($"[PlaneModDataManager] Aborted LoadData");
                dataLoadState = PlaneModDataLoadState.UnLoaded;
                return;
            }

            PlaneModAircraftData[] data = PlaneModDataUtility.ReadJson<PlaneModAircraftData[]>(CurrentDataPath);
            planeModData.aircraftData = data;
            
            PlaneModLogger.Msg($"[PlaneModDataManager] planeModData.aircraftData.Length={planeModData.aircraftData.Length}");
            
            dataLoadState = PlaneModDataLoadState.Loaded;
        }
        catch (FileNotFoundException)
        {
            dataLoadState = PlaneModDataLoadState.UnLoaded;
            PlaneModLogger.Warn($"[PlaneModDataManager] {CurrentDataPath} not found!");
        }
        catch (Exception)
        {
            dataLoadState = PlaneModDataLoadState.UnLoaded;
            PlaneModLogger.Warn($"[PlaneModDataManager] Something went wrong while loading {CurrentDataPath}. This could be that data is stored in a wrong datamodel!");
        }
    }
}