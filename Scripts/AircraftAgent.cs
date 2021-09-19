using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

namespace Aircraft
{
    public class AircraftAgent : Agent
    {

        // movement variables
        public float thrust = 100000f;
        public float pitchSpeed = 100f;
        public float yawSpeed = 100f;
        public float rollSpeed = 100f;
        public float boostSpeed = 2;
        public int NextCheckPointIndex { get;  set; }

        // training area components 
        private AircraftArea area;
        new private Rigidbody rigidbody;
        private TrailRenderer trail;

        //airplane controls

        private float pitchChange = 0f;
        private float smoothPitchChnage = 0f;
        private float MaxpitchAngle = 45f;
        private float yawChange = 0f;
        private float smoothYawChange = 0f;
        private float rollChange = 0f;
        private float smoothRollChange = 0f;
        private float maxRollAngle = 45f;
        private bool boost;


        public override void Initialize()
        {
            area = GetComponentInParent<AircraftArea>();
            rigidbody = GetComponent<Rigidbody>();
            trail = GetComponent<TrailRenderer>();
        }

        public override void OnActionReceived(float[] vectorAction)
        {
            pitchChange = vectorAction[0]; // the agent will go up, or stay in position. 
            if (pitchChange==2) 
            {
                pitchChange = -1f; // the agent will move downward. 
            }
            yawChange = vectorAction[1]; // the agent will turn right or stay in position.
            if (yawChange==2)
            {
                yawChange = -1f; // if the choice is 2, then the agent will turn left. 
            }

            boost = vectorAction[2] == 1;

            if (boost && !trail.emitting) trail.Clear();
            trail.emitting = boost;

            ProcessMovement(); 

        }

        private void ProcessMovement()
        {
            float boostmodifier = boost ? boostSpeed : 1f;

            rigidbody.AddForce(transform.forward * thrust * boostmodifier, ForceMode.Force); // how fast the agent can move forward. 

            Vector3 currRotation = transform.rotation.eulerAngles;

            float rollAngle = currRotation.z > 180f ? currRotation.z - 360f : currRotation.z; // if the rolling angle is greater than 180, then this code will subtract the angle by 360. 

            if (yawChange== 0f)
            {
                rollChange = -rollAngle / maxRollAngle;

            }
            else
            {
                rollChange = -yawChange;
            }

            // this whole code deals with the rate of turning angle to smooth out the movement. 

            smoothPitchChnage = Mathf.MoveTowards(smoothPitchChnage, pitchChange, 2f * Time.fixedDeltaTime);
            smoothYawChange = Mathf.MoveTowards(smoothYawChange, yawChange, 2f * Time.fixedDeltaTime);
            smoothRollChange = Mathf.MoveTowards(smoothRollChange, rollChange, 2f * Time.fixedDeltaTime);

            float pitch = currRotation.x + smoothPitchChnage * Time.fixedDeltaTime * pitchSpeed;

            if(pitch>180f)
            {
                pitch -= 360f; // keeping the pitch between -180f and 180f
            }

            pitch = Mathf.Clamp(pitch, -MaxpitchAngle, MaxpitchAngle);

            float yaw = currRotation.y + smoothYawChange * Time.fixedDeltaTime * yawSpeed;

            float roll = currRotation.z + smoothRollChange * Time.fixedDeltaTime * rollSpeed;

            if (roll > 180f)
            {
                roll -= 360f; // keeping the pitch between -180f and 180f
            }

            roll = Mathf.Clamp(roll, -maxRollAngle, maxRollAngle);

            transform.rotation = Quaternion.Euler(pitch, yaw, roll);
        }

    }

}

