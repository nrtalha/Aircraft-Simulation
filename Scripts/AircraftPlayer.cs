using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Aircraft

{
    public class AircraftPlayer : AircraftAgent
    {

        public InputAction pitchInput;
        public InputAction yawInput;
        public InputAction boostInput;
        public InputAction pauseInput;

        public override void Initialize()
        {
            base.Initialize();

            pitchInput.Enable();
            yawInput.Enable();
            boostInput.Enable();
            pauseInput.Enable();
        }


        //takes player input and converts into action array. 
        public override void Heuristic(float[] actionsOut)
        {

            // here the pitch: 1= up, 0= stays the same, -1= down
            float pitchValue = Mathf.Round(pitchInput.ReadValue<float>());

            // here the yaw: 1= turn right, 0= stays the same, -1= turn left

            float yawValue = Mathf.Round(yawInput.ReadValue<float>());

            // here the yaw: 1= turn right, 0= stays the same, -1= turn left

            float boostValue = Mathf.Round(boostInput.ReadValue<float>());

           // convert -1 (down) to discrete value 2
            if(pitchValue==-1f)

            {
                pitchValue = 2f;
            }

            // convert -1 (turn left) to discrete value 2
            if (yawValue==-1f)
            {
                yawValue = 2f;
            }

            actionsOut[0] = pitchValue;
            actionsOut[1] = yawValue;
            actionsOut[2] = boostValue;

        }

        // when agents gets destroyed, the inputs will get destroyed as well. 
        private void OnDestroy()
        {
            pitchInput.Disable();
            yawInput.Disable();
            boostInput.Disable();
            pauseInput.Disable();
        }
    }

}
