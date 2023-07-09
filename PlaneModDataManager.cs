using MelonLoader.TinyJSON;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TLD_PlaneMod;

public enum PlaneModDataLoadState
{
    UnLoaded,
    Loaded,
    Saved
}
public enum PlaneModDataManagementMode
{
    Normal,
    Development
}

public class PlaneModDataManager
{
    public static PlaneModDataManager Singleton;
    public PlaneModDataFixer dataFixer;
    public PlaneModData planeModData;

    public PlaneModDataLoadState dataLoadState;
    public PlaneModDataManagementMode dataManagementMode;
    public string lastSceneGUID;

    public string CurrentDataPath
    {
        get
        {
            string savePathSuffix = $"{SaveGameSystem.m_CurrentGameId}";
            string path = string.Format(PlaneModSettings.NEWSAVE_FORMATTABLE_DATAPATH, savePathSuffix);
            
            if (dataManagementMode == PlaneModDataManagementMode.Development)
            {
                path = PlaneModSettings.BASE_DATAPATH;
            }

            return path;
        }
    }
    
    public string StackTopPlaneGUID
    {
        get
        {
            PlaneModAircraftData data = planeModData.aircraftData[planeModData.aircraftData.Length-1];
            return data.guid;
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
    
    public void UpdateModelStreaming(string sceneIdentifier, bool force = false)
    {
        if (sceneIdentifier == lastSceneGUID && !force) return;
        lastSceneGUID = sceneIdentifier;

        if(dataLoadState == PlaneModDataLoadState.UnLoaded) LoadData();
        
        PlaneModLogger.Msg($"[PlaneModDataManager] UpdateModelStreaming dataLoadState={dataLoadState}, dataManagementMode={dataManagementMode}, sceneIdentifier={sceneIdentifier}");

        AircraftManager.Singleton.UnLoadAll();
        LoadAircraftForScene(sceneIdentifier);
    }

    public void LoadAircraftForScene(string sceneIdentifier)
    {
        foreach (var data in planeModData.aircraftData)
        {
            if (data.sceneGUID == sceneIdentifier)
            {
                PlaneModLogger.Msg($"[PlaneModDataManager] LoadAircraftForScene asset={data.asset}, sceneIdentifier={sceneIdentifier}");
                
                Vector3 position = new Vector3(data.position[0], data.position[1], data.position[2]);
                Vector3 velocity = new Vector3(data.velocity[0], data.velocity[1], data.velocity[2]);
                Vector3 angularVelocity = new Vector3(data.angularVelocity[0], data.angularVelocity[1], data.angularVelocity[2]);
                Vector3 guidance = new Vector3(data.guidance[0], data.guidance[1], data.guidance[2]);

                Quaternion rotation = new Quaternion(data.rotation[0], data.rotation[1], data.rotation[2], data.rotation[3]);
                
                GameObject plane = PlaneModAssetManager.Singleton.SpawnPlane(data.asset, position, rotation, false);
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

    public PlaneModAircraftData FindDataInstanceByGUID(string guid)
    {
        PlaneModAircraftData dataInstance = null;

        
        foreach (var data in planeModData.aircraftData)
        {
            if (data.guid == guid)
            {
                dataInstance = data;
                break;
            }
        }

        return dataInstance;
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
    
    public void DeleteDataInstance(string guid)
    {
        PlaneModAircraftData dataInstance = null;

        int index = 0;
        
        foreach (var data in planeModData.aircraftData)
        {
            if (data.guid == guid)
            {
                dataInstance = data;
                break;
            }

            index++;
        }

        if (dataInstance == null)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] DeleteDataInstance Failed, '{guid}' was not found!");
            return;
        }

        List<PlaneModAircraftData> list = planeModData.aircraftData.ToList();
        list.RemoveAt(index);
        planeModData.aircraftData = list.ToArray();
        PlaneModLogger.Msg($"[PlaneModDataManager] DeleteDataInstance guid={guid} index={index}");
    }

    public void SaveData()
    {
        if (CurrentDataPath == null)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] CurrentDataPath is null");
            PlaneModLogger.Warn($"[PlaneModDataManager] SaveData Failed!");
            return;
        }
        if (!File.Exists(CurrentDataPath))
        {
            PlaneModLogger.Msg($"[PlaneModDataManager] CurrentDataPath doesn't exists. Creating One!");
            PlaneModDataUtility.WriteDataWithBase(CurrentDataPath, PlaneModSettings.BASE_DATAPATH);
        }
        
        PlaneModLogger.Msg($"[PlaneModDataManager] SaveData dataManagementMode={dataManagementMode}");
        try
        {
            if (!File.Exists(CurrentDataPath))
            {
                PlaneModLogger.WarnMissingFile(CurrentDataPath);
                PlaneModLogger.Warn($"[PlaneModDataManager] SaveData Failed!");
                return;
            }

            try
            {
                UpdateAircraftData();
            }
            catch (NullReferenceException)
            {
                PlaneModLogger.Warn($"[PlaneModDataManager] NullReferenceException in UpdateAircraftData!");
                PlaneModLogger.Warn($"[PlaneModDataManager] SaveData Failed!");
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
            PlaneModLogger.Warn($"[PlaneModDataManager] SaveData Failed!");
        }
        catch (Exception e)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] Unknown error {e}!");
            PlaneModLogger.Warn($"[PlaneModDataManager] SaveData Failed!");
        }
    }
    
    public void LoadData()
    {
        if (CurrentDataPath == null)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] CurrentDataPath is null");
            PlaneModLogger.Warn($"[PlaneModDataManager] LoadData Failed!");
            dataLoadState = PlaneModDataLoadState.UnLoaded;
            return;
        }
        if (!File.Exists(CurrentDataPath))
        {
            PlaneModLogger.Msg($"[PlaneModDataManager] CurrentDataPath doesn't exist. Creating One!");
            PlaneModDataUtility.WriteDataWithBase(CurrentDataPath, PlaneModSettings.BASE_DATAPATH);
        }
        
        PlaneModLogger.Msg($"[PlaneModDataManager] LoadData dataManagementMode={dataManagementMode}");
        try
        {
            if (!File.Exists(CurrentDataPath))
            {
                PlaneModLogger.WarnMissingFile(CurrentDataPath);
                PlaneModLogger.Warn($"[PlaneModDataManager] LoadData Failed!");
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
            PlaneModLogger.Warn($"[PlaneModDataManager] LoadData Failed!");
        }
        catch (Exception e)
        {
            PlaneModLogger.Warn($"[PlaneModDataManager] Unknown error {e}!");
            PlaneModLogger.Warn($"[PlaneModDataManager] LoadData Failed!");
        }
    }
}