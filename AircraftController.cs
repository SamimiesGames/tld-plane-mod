namespace TLD_PlaneMod;

public class AircraftController : AircraftComponent
{
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);

        if (Input.GetKey(KeyCode.W))
        {
            aircraft.engine.Throttle(1, deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            aircraft.engine.Throttle(-1, deltaTime);
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
            {
                aircraft.guidance.z += 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                aircraft.guidance.z -= 1;
            }
        }
        else
        {
            aircraft.guidance.z = 0;
        }
        
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        
        aircraft.guidance.x += mouseY;
        
        /*
        if (mouseX != 0)
        {
            aircraft.guidance.y += mouseY;
        }
        */
    }
}