namespace TLD_PlaneMod
{
    public class AircraftComponent
    {
        public bool enabled;
        public Aircraft aircraft;
        public string name;

        public AircraftComponent()
        {
            aircraft = null;
            enabled = true;
            name = "NOT_SET";
        }

        public virtual void Start(Aircraft aAircraft, bool enabledByDefault=true, string aName="NOT_SET")
        {
            aircraft = aAircraft;
            enabled = enabledByDefault;
            name = aName;
        }
        
        public virtual void Update(float deltaTime) {}
    }
    public class Aircraft
    {
        public GameObject planeGameObject;
        public Rigidbody rigidbody;
        public Engine engine;

        public float yawSpeed;
        public float pitchSpeed;
        public float rollSpeed;

        public float maxSpeed;
        public float minSpeed;

        public float maxAltitude;

        public float speed;
        public Vector3 velocity;
        public Vector3 angularVelocity;

        public Vector3 guidance;

        public string guid;

        public Dictionary<string, AircraftComponent> aircraftComponents;


        public Aircraft(
            GameObject aPlaneGameObject, Rigidbody aRigidbody, Engine aEngine,
            float aYawSpeed, float aPitchSpeed, float aRollSpeed,
            float aMaxSpeed, float aMinSpeed, float aMaxAltitude,
            string aGuid
        )
        {
            planeGameObject = aPlaneGameObject;
            rigidbody = aRigidbody;
            
            engine = aEngine;

            yawSpeed = aYawSpeed;
            pitchSpeed = aPitchSpeed;
            rollSpeed = aRollSpeed;
            
            maxSpeed = aMaxSpeed;
            minSpeed = aMinSpeed;
            maxAltitude = aMaxAltitude;
            
            velocity = Vector3.zero;
            guidance = Vector3.zero;
            speed = 0;

            guid = aGuid;
            aircraftComponents = new Dictionary<string, AircraftComponent>();
        }

        public void AddComponent(string name, AircraftComponent aircraftComponent)
        {
            aircraftComponent.Start(this, true, name);
            
            aircraftComponents.Add(name, aircraftComponent);
        }

        public void SetComponentActive(string name, bool state)
        {
            aircraftComponents[name].enabled = state;
        }

        public AircraftComponent GetComponent(string name)
        {
            return aircraftComponents[name];
        }

        public void Update(float timeDelta)
        {
            if (planeGameObject == null) return;
            
            UpdateSpeed(timeDelta);
            UpdateAngularVelocity(timeDelta);
            UpdateAircraftComponents(timeDelta);

            velocity = Vector3.Lerp(velocity, planeGameObject.transform.forward * speed, 2 * timeDelta);
            rigidbody.velocity = -velocity;

            Vector3 direction = new Vector3(angularVelocity.x, angularVelocity.y, angularVelocity.z);
            
            direction.x = Mathf.Clamp(direction.x, -pitchSpeed * timeDelta, pitchSpeed * timeDelta);
            direction.y = Mathf.Clamp(direction.y, -yawSpeed * timeDelta, yawSpeed * timeDelta);
            direction.z = Mathf.Clamp(direction.z, -rollSpeed * timeDelta, rollSpeed * timeDelta);
            
            planeGameObject.transform.Rotate(direction);
        }

        private void UpdateSpeed(float timeDelta)
        {
            speed += maxSpeed * (engine.PowerOutput / engine.horsePower) * timeDelta;
            speed = Mathf.Clamp(speed, 0, maxSpeed * timeDelta);
        }

        private void UpdateAngularVelocity(float timeDelta)
        {
            angularVelocity.x = angularVelocity.x += guidance.x * (pitchSpeed * timeDelta);
            angularVelocity.x = angularVelocity.y += guidance.y * (yawSpeed * timeDelta);
            angularVelocity.x = angularVelocity.z += guidance.z * (rollSpeed * timeDelta);

            angularVelocity.x = Mathf.Clamp(angularVelocity.x, -pitchSpeed, pitchSpeed);
            angularVelocity.x = Mathf.Clamp(angularVelocity.y, -yawSpeed, yawSpeed);
            angularVelocity.x = Mathf.Clamp(angularVelocity.z, -rollSpeed, rollSpeed);
        }

        private void UpdateAircraftComponents(float timeDelta)
        {
            try
            {
                foreach (var component in aircraftComponents.Values)
                {
                    if(!component.enabled) continue;
                    component.Update(timeDelta);
                }
            }
            catch (Exception e)
            {
                PlaneModLogger.Warn($"Exception in UpdateAircraftComponents: {e}");
            }
        }
    }
}
