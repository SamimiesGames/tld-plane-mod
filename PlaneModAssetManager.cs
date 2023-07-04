using System.Text.Json;
using MelonLoader.TinyJSON;

namespace TLD_PlaneMod;

public class PlaneModAssetDefinition
{
    public string prefabName;
    public string worldCollider;
    public string[] landingGear;
    
    public float mass;
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

    public GameObject SpawnPlane(string prefabName, Vector3 position)
    {
        PlaneModLogger.Msg($"[PlaneModAssetManager] GetPlane prefabName={prefabName}");

        PlaneModAssetDefinition definition = FindAircraftDefinition(prefabName);
        GameObject prefab = UnityBundleManager.Singleton.GetPrefabFromAssetBundle(PlaneModSettings.PLANEMOD_BUNDLENAME, definition.prefabName);

        PlaneAutoRigger planeAutoRigger = new PlaneAutoRigger(definition);

        GameObject plane = planeAutoRigger.Rig(prefab, position);
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
                PlaneModLogger.Msg($"[PlaneModAssetManager] AssetDefinition (dump) = \n " +
                                   $"prefabName={def.prefabName}\n" +
                                   $"mass={def.mass}\n" +
                                   $"landingGear={def.landingGear}\n" +
                                   $"worldCollider={def.worldCollider}\n"
                                   );
                
            }
            /*
            foreach (var key in )
            {
                PlaneModLogger.Msg($"[PlaneModAssetManager] AssetDefinition={key}");
                PlaneModAssetDefinition definition = new PlaneModAssetDefinition();
                definition.prefabName = json[key][]   
                
            }
            */
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