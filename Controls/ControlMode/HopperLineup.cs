using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    class HopperLineup {



        public static float TurnLoop(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {

            const float BEGIN_RAMP_DOWN_AT = 15; //DEGREES


            float yaw = NUC_SerialConnection.GetVisionOrientation().yaw;

            float driveSpeed = Controls.ControlAlgorithms.P_Loop(yaw, BEGIN_RAMP_DOWN_AT, 0);

            logitechController.AXES.LEFT_Y = -driveSpeed;
            logitechController.AXES.RIGHT_Y = driveSpeed;

            return driveSpeed;
        }

        public static float DistanceLoop(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {

            const float BEGIN_RAMP_DOWN_AT = 26; //INCHES
            const float STOPPING_DISTANCE = 20; //INCHES



            float dist = NUC_SerialConnection.GetVisionLocation().z;


            float driveSpeed = Controls.ControlAlgorithms.P_Loop(dist, BEGIN_RAMP_DOWN_AT, STOPPING_DISTANCE);

            logitechController.AXES.LEFT_Y = driveSpeed;
            logitechController.AXES.RIGHT_Y = driveSpeed;

            //Debug.Print(driveSpeed.ToString());

            return driveSpeed;
        }

        public static float HorizontalDisplacementLoop(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            float dist = NUC_SerialConnection.GetVisionLocation().x;

            const float BEGIN_RAMP_DOWN_AT = 20;

            float driveSpeed = Controls.ControlAlgorithms.P_Loop(dist, BEGIN_RAMP_DOWN_AT);

            logitechController.AXES.LEFT_Y = driveSpeed;
            logitechController.AXES.RIGHT_Y = driveSpeed;

            Debug.Print(driveSpeed.ToString());

            return driveSpeed;
        }

        public static void HopperLineupLoop(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {

            const float DISTANCE_SCALAR = 1;
            const float YAW_SCALAR = 2f;
            const float X_SCALAR = 0.6f;


            float driveSpeed = DISTANCE_SCALAR * DistanceLoop(ref logitechController, ref NUC_SerialConnection);
            driveSpeed = TypeConverter.Constrain(driveSpeed, -1.0f, 1.0f);

            

            float turnVal = YAW_SCALAR * TurnLoop(ref logitechController, ref NUC_SerialConnection) * (1-driveSpeed);
            float horizontalVal = X_SCALAR * HorizontalDisplacementLoop(ref logitechController, ref NUC_SerialConnection);



            logitechController.AXES.LEFT_Y = -TypeConverter.Constrain(driveSpeed - turnVal + horizontalVal, -0.15f, 1);
            logitechController.AXES.RIGHT_Y = -TypeConverter.Constrain(driveSpeed + turnVal - horizontalVal, -0.15f, 1);

            //Debug.Print(logitechController.AXES.LEFT_Y.ToString() + ", " + logitechController.AXES.RIGHT_Y.ToString());

        }







    }
}
