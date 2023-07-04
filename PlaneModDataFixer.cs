namespace TLD_PlaneMod;

public class PlaneModDataFixer
{
    public PlaneModDataFixer() { }

    public void FixMissingFile(string dataPath)
    {
        if(!File.Exists(dataPath))
        {
            Melon<PlaneMod>.Logger.Msg($"[PlaneModDataFixer] FixMissingFile {dataPath}");
            File.Create(dataPath).Close();
        }
    }

    public PlaneModData FixMissingOrBrokenData(PlaneModData planeModData)
    {
        int brokenDataInstancesFound = 0;
        
        Melon<PlaneMod>.Logger.Msg($"[PlaneModDataFixer] FixMissingOrBrokenData brokenDataInstancesFound={brokenDataInstancesFound}");

        return planeModData;
    }
}