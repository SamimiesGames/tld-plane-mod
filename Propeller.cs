namespace TLD_PlaneMod;

public class Propeller : AircraftComponent
{
    public Transform propellerTransform;
    public Engine engine;
    public float maxPropellerRPM;
    public float rpm;

    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
    
        rpm = engine.RPMRatio * maxPropellerRPM;
        float rotation = 360 * (rpm / 360);
        
        propellerTransform.transform.Rotate(Vector3.up * (rotation * deltaTime));
    }
}