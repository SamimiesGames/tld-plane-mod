namespace TLD_PlaneMod;

public class AircraftManager
{
    public static AircraftManager Singleton;
    public List<Aircraft> aircrafts;

    public AircraftManager()
    {
        if (Singleton != null) return;

        Singleton = this;
        aircrafts = new List<Aircraft>();
        
        PlaneModLogger.Msg($"[AircraftManager] Initialized");
    }

    public Aircraft GetAircraftByGUID(string guid)
    {
        foreach (var aircraft in aircrafts)
        {
            if (aircraft.guid == guid)
            {
                return aircraft;
            }
        }

        return null;
    }

    public void Update(float timeDelta)
    {
        EliminateNulls();
        UpdateAircraft(timeDelta);
    }

    private void EliminateNulls()
    {
        int eliminatedNulls = 0;
        
        List<Aircraft> aircraftReconstructed = new List<Aircraft>();
        
        foreach (var aircraft in new List<Aircraft>(aircrafts))
        {
            if (aircraft.planeGameObject == null)
            {
                eliminatedNulls++;
                continue;
            }

            aircraftReconstructed.Add(aircraft);
        }

        if(eliminatedNulls > 0) aircrafts = aircraftReconstructed;
        
        if(eliminatedNulls > 0) PlaneModLogger.Warn($"[AircraftManager] EliminateNulls eliminatedNulls={eliminatedNulls}");
    }

    private void UpdateAircraft(float timeDelta)
    {
        foreach (var aircraft in aircrafts) aircraft.Update(timeDelta);
    }

    public void AddNewAircraft(Aircraft aircraft)
    {
        PlaneModLogger.Msg($"[AircraftManager] AddNewAircraft aircraft={aircraft.planeGameObject.name}");
        aircrafts.Add(aircraft);
    }

    public void RemoveAircraft(Aircraft aircraft, bool deleteDataInstance = false)
    {
        if (deleteDataInstance)
        {
            PlaneModDataManager.Singleton.DeleteDataInstance(aircraft.guid);
        }
        PlaneModDataManager.Singleton.UpdateAircraftData();
        
        if (aircraft.planeGameObject == null)
        {
            PlaneModLogger.Warn($"[AircraftManager] RemoveAircraft (from presentation) Failed, because aircraft.planeGameObject is null!");
            return;
        }
        PlaneModLogger.Msg($"[AircraftManager] RemoveAircraft (from presentation) aircraft={aircraft.planeGameObject.name}");
        
        aircrafts.Remove(aircraft);
        
        GameObject.Destroy(aircraft.planeGameObject);
    }

    public void UnLoadAll()
    {
        PlaneModLogger.Msg($"[AircraftManager] UnLoadAll");

        foreach (var aircraft in new List<Aircraft>(aircrafts))
        {
            RemoveAircraft(aircraft);
        }
    }
}