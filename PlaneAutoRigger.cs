namespace TLD_PlaneMod;

public class PlaneAutoRigger
{
    public PlaneModAssetDefinition definition;

    private GameObject _recursiveFoundGameObject;
    
    public PlaneAutoRigger(PlaneModAssetDefinition aDefinition)
    {
        definition = aDefinition;
    }

    public GameObject Rig(GameObject prefab, Vector3 position)
    {
        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Rig prefab={prefab.name}");

        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Instantiate");
        GameObject gameObject = GameObject.Instantiate(prefab);

        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Rig Transform");
        gameObject.transform.position = position;


        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Rig worldCollider");
        FindGameObjectByName(gameObject, definition.worldCollider);

        if (_recursiveFoundGameObject)
        {
            RigGameObjectWithConvexMeshCollider(_recursiveFoundGameObject);
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

        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Fixing shaders");

        Shader standardShader = Shader.Find("Standard");
        new ShaderPostFixer(gameObject, standardShader);

        Engine engine = new Engine(
            definition.enginePower, definition.maxRPM,
            definition.acceleration, definition.fuelCapacity,
            definition.fuelCapacity
        );

        if(string.IsNullOrEmpty(definition.guid)) definition.guid = Guid.NewGuid().ToString();
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
            definition.guid
        );
        
        AircraftManager.Singleton.AddNewAircraft(aircraft);
        
        PlaneModLogger.MsgVerbose($"[PlaneAutoRigger] Rigging is partial");

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