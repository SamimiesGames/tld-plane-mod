using System.Text.Json;
using MelonLoader.TinyJSON;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace TLD_PlaneMod;

public class PlaneModAssetDefinition
{
    public string prefabName;
    public string worldCollider;
    public string[] landingGear;
    
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

    public GameObject SpawnPlane(string prefabName, Vector3 position, bool createNewDataInstance=true)
    {
        PlaneModLogger.Msg($"[PlaneModAssetManager] SpawnPlane prefabName={prefabName}, position={position}, createNewDataInstance={createNewDataInstance}");

        PlaneModAssetDefinition definition = FindAircraftDefinition(prefabName);
        GameObject prefab = UnityBundleManager.Singleton.GetPrefabFromAssetBundle(PlaneModSettings.PLANEMOD_BUNDLENAME, definition.prefabName);

        PlaneAutoRigger planeAutoRigger = new PlaneAutoRigger(definition);

        GameObject plane = planeAutoRigger.Rig(prefab, position);

        if (createNewDataInstance)
        {
            PlaneModAircraftData data = new PlaneModAircraftData();

            Aircraft aircraft = AircraftManager.Singleton.aircrafts[AircraftManager.Singleton.aircrafts.Count-1];
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
            data.sceneGUID = SceneManager.GetActiveScene().guid;

            PlaneModDataManager.Singleton.planeModData.aircraftData.AddItem(data);
            PlaneModLogger.Msg($"[PlaneModAssetManager] SpawnPlane Created new data instance");
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
            foreach (var def in assetDefinitions.aircraftAssetDefinitions.Values)
            {
                PlaneModLogger.MsgVerbose($"[PlaneModAssetManager] {def}");
            }
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