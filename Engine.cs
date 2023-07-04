namespace TLD_PlaneMod;

public class Engine
{
    public float horsePower;
    public float maxRPM;
    public float acceleration;

    public float rpm;

    public Engine(float aHorsePower, float aMaxRPM, float aAcceleration)
    {
        horsePower = aHorsePower;
        maxRPM = aMaxRPM;
        acceleration = aAcceleration;
    }

    public float PowerOutput
    {
        get
        {
            return horsePower * (rpm / maxRPM);
        }
    }

    public void Throttle(float normal, float delta)
    {
        rpm += (normal * acceleration) * delta;
        rpm = Mathf.Clamp(rpm, 0, maxRPM);
    }
    
}