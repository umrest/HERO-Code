using System;
using Microsoft.SPOT;

namespace HERO_Code {
    class DashboardStateDecoder {

        private const byte ENABLE_KEY_START_VALUE = 20;

        private bool enabled;
        private byte controlMode;


        public void DecodeDashboardStateData(byte[] data) {

            //Search for the 8 byte consecutive string in order to enable the robot
            //If any of the 8 are incorrect, DO NOT enable
            enabled = true;

            for (int i = 0; i < 8; i++) {
                if (data[i] != (ENABLE_KEY_START_VALUE + i)) {
                    enabled = false;
                    break;
                }
            }

            //Don't set an active control mode if the robot is not enabled.
            if (!enabled) {
                controlMode = ControlModeHandler.ControlMode.DISABLED;
                return;
            }

            //Read in the robot control state byte
            controlMode = data[8];

          //  Debug.Print(controlMode.ToString());

        }

        //Returns robot enabled status
        public bool IsEnabled() {
            return enabled;
        }

        public short GetControlMode() {
            return controlMode;
        }
    }
}
