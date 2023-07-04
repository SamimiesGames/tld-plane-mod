namespace TLD_PlaneMod;

public class PlaneModAircraftData
{
    public string asset;
    public string sceneGUID;

    public float[] position;
    public float[] rotation;
    public float[] velocity;
    public float[] angularVelocity;
    public float[] guidance;

    public float rpm;
    public float fuel;
    public string guid;
}

public class PlaneModData
{
    public PlaneModAircraftData[] aircraftData;

    public PlaneModData()
    {
        aircraftData = new PlaneModAircraftData[] { };
    }
}