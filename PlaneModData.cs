namespace TLD_PlaneMod;
public class PlaneModAircraftData
{
    public string asset;
    public int sceneID;
    
    public float[] position;
    public float[] rotation;
    public float[] velocity;
    public float[] guidance;

    public float rpm;
    public float fuel;
    public string guid;
}
public class PlaneModData
{
    public List<PlaneModAircraftData> aircraftData;
}