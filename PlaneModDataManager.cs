using MelonLoader.TinyJSON;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TLD_PlaneMod;

public enum PlaneModDataLoadState
{
    UnLoaded,
    Loaded,
    Saved
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
    /*
    Only call UpdateModelStreaming call from MelonMod.OnSceneWasLoaded 
    */
    public void UpdateModelStreaming(string sceneGUID)
    {
        // TODO: Find out a way to identify scene change!
        if (sceneGUID == lastSceneGUID) return;
        lastSceneGUID = sceneGUID;

        if(dataLoadState == PlaneModDataLoadState.UnLoaded) LoadData();
        
        PlaneModLogger.Msg($"[PlaneModDataManager] UpdateModelStreaming dataLoadState={dataLoadState}, sceneGUID={sceneGUID}");

        AircraftManager.Singleton.UnLoadAll();
        LoadAircraftForScene(sceneGUID);
    }

    public void LoadAircraftForScene(string sceneGUID)
    {
        foreach (var data in planeModData.aircraftData)
        {
            if (data.sceneGUID == sceneGUID)
            {
                PlaneModLogger.Msg($"[PlaneModDataManager] LoadAircraftForScene asset={data.asset}, sceneGUID={sceneGUID}");
                
                Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
                Vector3 velocity = new Vector3(data.velocity[0], data.velocity[1], data.velocity[2]);
                Vector3 angularVelocity = new Vector3(data.angularVelocity[0], data.angularVelocity[1], data.angularVelocity[2]);
                Vector3 guidance = new Vector3(data.guidance[0], data.guidance[1], data.guidance[2]);
                
                GameObject plane = PlaneModAssetManager.Singleton.SpawnPlane(data.asset, position, false);
                Aircraft aircraft = AircraftManager.Singleton.aircrafts[AircraftManager.Singleton.aircrafts.Count - 1];

                aircraft.planeGameObject = plane;
                
                plane.transform.position = position;
                aircraft.velocity = velocity;
                aircraft.angularVelocity = angularVelocity;
                aircraft.guidance = guidance;
                aircraft.engine.rpm = data.rpm;
                aircraft.engine.fuel = data.fuel;
                aircraft.guid = data.guid;
            }
        }
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
                    break;
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

    public void CreateNewDataInstance(Aircraft aircraft, string prefabName)
    {
        PlaneModAircraftData data = new PlaneModAircraftData();
        
        data.guid = aircraft.guid;
        data.asset = prefabName;
        data.position = new float[3]
        {
            aircraft.planeGameObject.transform.position.x,
            aircraft.planeGameObject.transform.position.y,
            aircraft.planeGameObject.transform.position.z,
        };
        data.rotation = new float[4]
        {
            aircraft.planeGameObject.transform.rotation.x,
            aircraft.planeGameObject.transform.rotation.y,
            aircraft.planeGameObject.transform.rotation.z,
            aircraft.planeGameObject.transform.rotation.w,
        };
        data.guidance = new float[3]
        {
            aircraft.guidance.x,
            aircraft.guidance.y,
            aircraft.guidance.z,
        };
        data.angularVelocity = new float[3]
        {
            aircraft.angularVelocity.x,
            aircraft.angularVelocity.y,
            aircraft.angularVelocity.z
        };
        data.velocity = new float[3]
        {
            aircraft.velocity.x,
            aircraft.velocity.y,
            aircraft.velocity.z
        };
        data.fuel = aircraft.engine.fuel;
        data.rpm = aircraft.engine.rpm;
        data.sceneGUID = lastSceneGUID;

        List<PlaneModAircraftData> list = planeModData.aircraftData.ToList();
        list.Add(data);
        planeModData.aircraftData = list.ToArray();
        
        PlaneModLogger.Msg($"[PlaneModDataManager] Created new data instance");
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

            try
            {
                UpdateAircraftData();
            }
            catch (NullReferenceException)
            {
                PlaneModLogger.Warn($"[PlaneModDataManager] NullReferenceException in UpdateAircraftData!");;
                PlaneModLogger.Warn($"[PlaneModDataManager] Aborted SaveData");
                return;
            }
            PlaneModLogger.MsgVerbose($"[PlaneModDataManager] SaveData aircraftData to be saved {planeModData.aircraftData.Length}");
            
            string jsonData = JSON.Dump(planeModData.aircraftData);
            PlaneModDataUtility.WriteData(CurrentDataPath, jsonData);
            
            dataLoadState = PlaneModDataLoadState.Saved;
        }
        catch (FileNotFoundException)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] {CurrentDataPath} not found!");
        }
        catch (Exception e)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] Unknown error {e}!");
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
        catch (Exception e)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] Unknown error {e}!");
        }
    }
}