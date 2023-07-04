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
        
        Melon<PlaneMod>.Logger.Msg($"[AircraftManager] Initialized");
    }

    public void Update(float timeDelta)
    {
        UpdateAircraft(timeDelta);
    }

    private void UpdateAircraft(float timeDelta)
    {
        foreach (var aircraft in aircrafts) aircraft.Update(timeDelta);
    }

    public void AddNewAircraft(Aircraft aircraft)
    {
        Melon<PlaneMod>.Logger.Msg($"[AircraftManager] AddNewAircraft aircraft={aircraft.planeGameObject.name}");
        aircrafts.Add(aircraft);
    }

    public void RemoveAircraft(Aircraft aircraft)
    {
        Melon<PlaneMod>.Logger.Msg($"[AircraftManager] RemoveAircraft aircraft={aircraft.planeGameObject.name}");
        aircrafts.Remove(aircraft);
    }
}