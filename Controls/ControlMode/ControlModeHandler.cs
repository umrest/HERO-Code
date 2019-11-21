using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    class ControlModeHandler {

        public static class ControlMode {
            public const int TEST = -1;

            public const int DISABLED = 0;
            public const int TELEOP = 1;
            public const int AUTONOMOUS = 2;
        }

        //Stores the current control mode of the rover
        private int mode;

        public ControlModeHandler() {
            mode = ControlMode.DISABLED;
        }

        public void updateControllerValues(ref Controller controller, ref SerialCommsHandler serial) {
            switch (mode) {

                //Use sensors and algorithms to apply values to the controller instead of a human operator
                case ControlMode.AUTONOMOUS:
                    AutonomousMode(ref controller, ref serial);
                    break;

                //Read in the human-controlled joystick over the serial connection
                case ControlMode.TELEOP:
                    TeleopMode(ref controller, ref serial);
                    break;

                //For testing new code/functions
                case ControlMode.TEST:
                    TestMode(ref controller, ref serial);
                    break;

                //Do nothing
                case ControlMode.DISABLED:
                    break;

                //Invalid mode - print an error message
                default:
                    Debug.Print("ERROR - Invalid Mode: " + mode.ToString());
                    break;

            }
        }

        private void AutonomousMode(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {

        }

        //Update logitechController values with those sent wirelessly from the driver station
        private void TeleopMode(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {

            NUC_SerialConnection.UpdateJoystickValues(ref logitechController);
        }

        //For running test code, testing new functions
        private void TestMode(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            int yaw = NUC_SerialConnection.GetVisionOrientation().yaw;

            logitechController.AXES.LEFT_Y = ((float)yaw) / 90.0;
            logitechController.AXES.RIGHT_Y = -((float)yaw) / 90.0;
            logitechController.BUTTONS.LB = true;
        }




        // ---------------------- Getters and Setters ----------------------

        public int GetMode() {
            return mode;
        }

        public void SetMode(int MODE) {
            this.mode = MODE;
        }


    }
}
