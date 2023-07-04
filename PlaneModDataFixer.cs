namespace TLD_PlaneMod;

public class PlaneModDataFixer
{
    public PlaneModDataFixer() { }

    public bool FixMissingFile(string dataPath)
    {
        if(!File.Exists(dataPath))
        {
            PlaneModLogger.Msg($"[PlaneModDataFixer] FixMissingFile {dataPath}");
            File.Create(dataPath).Close();
            
            return true;
        }

        return false;
    }

    public PlaneModData FixMissingOrBrokenData(PlaneModData planeModData)
    {
        int brokenDataInstancesFound = 0;
        
        PlaneModLogger.MsgVerbose($"[PlaneModDataFixer] FixMissingOrBrokenData brokenDataInstancesFound={brokenDataInstancesFound}");

        return planeModData;
    }
}