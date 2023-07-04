namespace TLD_PlaneMod;

public class PlaneModDataUtility
{
    public static string FormatPlaneDataPath(string aPlaneGUID)
    {
        string dataPath = string.Format(PlaneModSettings.PLANE_DATAPATH_WithGUID, aPlaneGUID);
        return dataPath;
    }
}