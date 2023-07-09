namespace TLD_PlaneMod;

public class Engine
{
    public float horsePower;
    public float maxRPM;
    public float acceleration;

    public float fuel;
    public float fuelCapacity;

    public float rpm;

    public Engine(float aHorsePower, float aMaxRPM, float aAcceleration, float aFuel, float aFuelCapacity)
    {
        horsePower = aHorsePower;
        maxRPM = aMaxRPM;
        acceleration = aAcceleration;

        fuel = aFuel;
        fuelCapacity = aFuelCapacity;
    }

    public float PowerOutput
    {
        get
        {
            return horsePower * (rpm / maxRPM);
        }
    }

    public float RPMRatio
    {
        get
        {
            return rpm / maxRPM;
        }
    }

    public void Throttle(float normal, float delta)
    {
        rpm += (normal * acceleration) * delta;
        rpm = Mathf.Clamp(rpm, 0, maxRPM);
    }

}