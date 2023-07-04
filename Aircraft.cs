namespace TLD_PlaneMod
{
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
        }

        public void Update(float timeDelta)
        {
            UpdateSpeed(timeDelta);
            UpdateRotation(timeDelta);

            velocity = Vector3.Lerp(velocity, planeGameObject.transform.forward * speed, 2 * timeDelta);
            rigidbody.velocity = velocity;
            
            planeGameObject.transform.Rotate(planeGameObject.transform.up * angularVelocity.x);
            planeGameObject.transform.Rotate(-planeGameObject.transform.forward * angularVelocity.y);
            planeGameObject.transform.Rotate(planeGameObject.transform.right * angularVelocity.z);
        }

        private void UpdateSpeed(float timeDelta)
        {
            speed += maxSpeed * (engine.PowerOutput / engine.horsePower) * timeDelta;
        }

        private void UpdateRotation(float timeDelta)
        {
            angularVelocity.x = Mathf.Lerp( angularVelocity.x,  guidance.x * pitchSpeed, pitchSpeed * timeDelta);
            angularVelocity.y = Mathf.Lerp( angularVelocity.y,  guidance.y * yawSpeed, yawSpeed * timeDelta);
            angularVelocity.z = Mathf.Lerp( angularVelocity.z,  guidance.z * rollSpeed, rollSpeed * timeDelta);
        }
    }
}
