using VehicleDoor = Il2Cpp.VehicleDoor;

namespace TLD_PlaneMod;

public class PlaneAutoRigger
{
    public PlaneModAssetDefinition definition;

    private GameObject _recursiveFoundGameObject;
    
    public PlaneAutoRigger(PlaneModAssetDefinition aDefinition)
    {
        definition = aDefinition;
    }

    public GameObject Rig(GameObject prefab, Vector3 position, Quaternion rotation, bool createKinematic_RB, float yPadding = 0)
    {
        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Rig prefab={prefab.name}");

        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Instantiate");
        GameObject gameObject = GameObject.Instantiate(prefab);

        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Rig Transform");
        gameObject.transform.position = position + Vector3.up * yPadding;
        gameObject.transform.rotation = rotation;


        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Rig worldCollider");
        FindGameObjectByName(gameObject, definition.worldCollider);
        GameObject worldCollider = null;

        if (_recursiveFoundGameObject)
        {
            RigGameObjectWithConvexMeshCollider(_recursiveFoundGameObject);
            worldCollider = _recursiveFoundGameObject;
            _recursiveFoundGameObject = null;
        }

        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Rig LandingGear");
        foreach (var landingGear in definition.landingGear)
        {
            FindGameObjectByName(gameObject, landingGear);

            if (_recursiveFoundGameObject)
            {
                RigGameObjectWithConvexMeshCollider(_recursiveFoundGameObject);
                _recursiveFoundGameObject = null;
            }
        }

        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Rig Rigidbody");
        Rigidbody rigidBody = gameObject.AddComponent<Rigidbody>();
        rigidBody.drag = 0.5f;
        rigidBody.useGravity = true;
        rigidBody.mass = definition.mass;
        if (createKinematic_RB) rigidBody.isKinematic = true;

        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Fixing shaders");

        Shader standardShader = Shader.Find("Standard");
        new ShaderPostFixer(gameObject, standardShader);
        
        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Rig Aircraft Scripts");

        Engine engine = new Engine(
            definition.enginePower, definition.maxRPM,
            definition.acceleration, definition.fuelCapacity,
            definition.fuelCapacity
        );
        
        Aircraft aircraft = new Aircraft(
            gameObject,
            rigidBody,
            engine,
            definition.yawSpeed,
            definition.pitchSpeed,
            definition.rollSpeed,
            definition.maxSpeed,
            definition.minSpeed,
            definition.maxAltitude,
            Guid.NewGuid().ToString()
        );

        if (definition.stringData.ContainsKey("PROPULSION_Propeller"))
        {
            GameObject propellerGameObject = null;
            FindGameObjectByName(gameObject, definition.stringData["PROPULSION_Propeller"]);

            if (_recursiveFoundGameObject)
            {
                RigGameObjectWithConvexMeshCollider(_recursiveFoundGameObject);
                propellerGameObject = _recursiveFoundGameObject;
                _recursiveFoundGameObject = null;
            }

            if (propellerGameObject == null) 
            {
                PlaneModLogger.Warn($"[PlaneAutoRigger] PROPULSION_Propeller is missing!");
            }
            else
            {
                float propellerMaxRPM = 0;
                
                if(definition.floatData.ContainsKey("PROPULSION_Propeller_maxRPM"))
                {
                    propellerMaxRPM = definition.floatData["PROPULSION_Propeller_maxRPM"];
                }
                else
                {
                    PlaneModLogger.Warn($"[PlaneAutoRigger] PROPULSION_Propeller_maxRPM is missing!");
                }

                Propeller propeller = new Propeller();
                
                propeller.propellerTransform = propellerGameObject.transform;
                propeller.engine = engine;
                propeller.maxPropellerRPM = propellerMaxRPM;
                
                aircraft.AddComponent("PROPULSION_Propeller", propeller);
                PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Added PROPULSION_Propeller");
            }
        }

        AircraftController aircraftController = new AircraftController();
        aircraft.AddComponent("aircraftController", aircraftController);
        aircraft.SetComponentActive("aircraftController", false);
        
        AircraftManager.Singleton.AddNewAircraft(aircraft);

        return gameObject;
    }

    private GameObject FindGameObjectByName(GameObject subGameObject, string name)
    {
        if (!subGameObject) return null;

        if (subGameObject.name == name)
        {
            _recursiveFoundGameObject = subGameObject;
            return subGameObject;
        }

        int i = 0;

        while (i < subGameObject.transform.childCount)
        {
            FindGameObjectByName(subGameObject.transform.GetChild(i).gameObject, name);
            i++;
        }
        return null;
    }

    private void RigGameObjectWithConvexMeshCollider(GameObject gameObject)
    {
        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] RigGameObjectWithConvexMeshCollider gameObject={gameObject.name}");
        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.convex = true;
        meshCollider.gameObject.layer = 17;
    }
}