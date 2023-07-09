using System.Text.Json;
using MelonLoader.TinyJSON;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TLD_PlaneMod;

public class PlaneModAssetDefinition
{
    public string prefabName;
    public string worldCollider;
    public string[] landingGear;

    public Dictionary<string, float> floatData;
    public Dictionary<string, int> intData;
    public Dictionary<string, string> stringData;
    public Dictionary<string, bool> booleanData;
    
    public float mass;
    public float enginePower;
    public float acceleration;
    public float maxRPM;
    
    public float maxAltitude;
    
    public float maxSpeed;
    public float minSpeed;
    
    public float yawSpeed;
    public float pitchSpeed;
    public float rollSpeed;

    public float carryCapacity;
    
    public float fuelCapacity;
    public float fuelConsumption;

    public string guid;

    public override string ToString()
    {
        return $"PlaneModAssetDefinition (dump)\n" +
               $"worldCollider={worldCollider}\n" +
               $"landingGear={landingGear}\n" +
               $"floatData={floatData}\n" +
               $"intData={intData}\n" +
               $"stringData={stringData}\n" +
               $"booleanData={booleanData}\n" +
               $"mass={mass}\n" +
               $"enginePower={enginePower}\n" +
               $"maxRPM={maxRPM}\n" +
               $"acceleration={acceleration}\n" +
               $"maxAltitude={maxAltitude}\n" +
               $"maxSpeed={maxSpeed}\n" +
               $"minSpeed={minSpeed}\n" +
               $"yawSpeed={yawSpeed}\n" +
               $"pitchSpeed={pitchSpeed}\n" +
               $"rollSpeed={rollSpeed}\n" +
               $"carryCapacity={carryCapacity}\n" +
               $"fuelCapacity={fuelCapacity}\n" +
               $"fuelConsumption={fuelConsumption}\n" +
               $"guid={guid}\n";
    }
}

public class PlaneModAssetDefinitions
{
    public Dictionary<string, PlaneModAssetDefinition> aircraftAssetDefinitions;
}

public class PlaneModAssetManager
{
    public static PlaneModAssetManager Singleton;
    public PlaneModAssetDefinitions assetDefinitions;
    
    public PlaneModAssetManager()
    {
        if (Singleton != null) return;
        Singleton = this;

        LoadAssetDefinitions();
        
        PlaneModLogger.Msg($"[PlaneModAssetManager] Initialized");
    }

    public GameObject SpawnPlane(string prefabName, Vector3 position, Quaternion rotation, bool createNewDataInstance=true)
    {
        PlaneModLogger.Msg($"[PlaneModAssetManager] SpawnPlane prefabName={prefabName}, position={position}, createNewDataInstance={createNewDataInstance}");

        PlaneModAssetDefinition definition = FindAircraftDefinition(prefabName);
        GameObject prefab = UnityBundleManager.Singleton.GetPrefabFromAssetBundle(PlaneModSettings.PLANEMOD_BUNDLENAME, definition.prefabName);

        PlaneAutoRigger planeAutoRigger = new PlaneAutoRigger(definition);
        GameObject plane = planeAutoRigger.Rig(prefab, position, rotation, false, !createNewDataInstance ? 0.5f : 0);

        Aircraft aircraft = AircraftManager.Singleton.aircrafts[AircraftManager.Singleton.aircrafts.Count-1];
        
        if (createNewDataInstance)
        {
            PlaneModDataManager.Singleton.CreateNewDataInstance(aircraft, prefabName);
        }

        if (aircraft.planeGameObject == null)
        {
            PlaneModLogger.Warn($"[PlaneModAssetManager] PLANE WAS RIGGED WITH OUT A GAMEOBJECT!");
        }
        

        return plane;
    }

    public void LoadAssetDefinitions()
    {
        PlaneModLogger.Msg($"[PlaneModAssetManager] LoadAssetDefinitions");
        
        if (File.Exists(PlaneModSettings.ASSET_DEFINITION_PATH))
        {
            assetDefinitions = new PlaneModAssetDefinitions();
            assetDefinitions.aircraftAssetDefinitions = PlaneModDataUtility.ReadJson<Dictionary<string, PlaneModAssetDefinition>>(PlaneModSettings.ASSET_DEFINITION_PATH);

            PlaneModLogger.Msg($"[PlaneModAssetManager] Loaded {assetDefinitions.aircraftAssetDefinitions.Keys.Count} Asset Definitions");
        }
        else
        {
            PlaneModLogger.WarnMissingFile(PlaneModSettings.ASSET_DEFINITION_PATH);
        }
    }

    private PlaneModAssetDefinition FindAircraftDefinition(string prefabName)
    {
        PlaneModLogger.Msg($"[PlaneModAssetManager] FindAircraftDefinition prefabName={prefabName}");
        PlaneModAssetDefinition def = null;
        foreach (var lDef in assetDefinitions.aircraftAssetDefinitions.Keys)
        {
            if (lDef == prefabName)
            {
                def = assetDefinitions.aircraftAssetDefinitions[lDef];
                return def;
            }
        }

        if (def == null)
        {
            PlaneModLogger.Warn($"[PlaneModAssetManager] Couldn't find PlaneModAircraftDefinition!");
        }

        return def;
    }
}