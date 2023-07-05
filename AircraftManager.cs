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

    public void Update(float timeDelta)
    {
        //EliminateNulls();
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
        
        if(eliminatedNulls > 0) PlaneModLogger.Msg($"[AircraftManager] EliminateNulls eliminatedNulls={eliminatedNulls}");
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

    public void RemoveAircraft(Aircraft aircraft)
    {
        PlaneModLogger.Msg($"[AircraftManager] RemoveAircraft aircraft={aircraft.planeGameObject.name}");
        PlaneModDataManager.Singleton.UpdateAircraftData();
        
        aircrafts.Remove(aircraft);
        
        GameObject.Destroy(aircraft.planeGameObject);
    }

    public void UnLoadAll()
    {
        PlaneModLogger.Msg($"[AircraftManager] UnLoadAll");

        foreach (var aircraft in aircrafts)
        {
            RemoveAircraft(aircraft);
        }
    }
}