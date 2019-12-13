using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    class HopperLineup {



        public static float TurnLoop(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            float yaw = NUC_SerialConnection.GetVisionOrientation().yaw;

            float driveSpeed = Controls.ControlAlgorithms.P_Loop(yaw, 90.0f, 0);

            logitechController.AXES.LEFT_Y = -driveSpeed;
            logitechController.AXES.RIGHT_Y = driveSpeed;

            return driveSpeed;
        }

        public static float DistanceLoop(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            float dist = NUC_SerialConnection.GetVisionLocation().z;


            float driveSpeed = Controls.ControlAlgorithms.P_Loop(dist, 20.0f, 8.0f);

            logitechController.AXES.LEFT_Y = driveSpeed;
            logitechController.AXES.RIGHT_Y = driveSpeed;

            Debug.Print(driveSpeed.ToString());

            return driveSpeed;
        }

        public static float HorizontalDisplacementLoop(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            float dist = NUC_SerialConnection.GetVisionLocation().x;


            float driveSpeed = Controls.ControlAlgorithms.P_Loop(dist, 30.0f);

            logitechController.AXES.LEFT_Y = driveSpeed;
            logitechController.AXES.RIGHT_Y = driveSpeed;

            Debug.Print(driveSpeed.ToString());

            return driveSpeed;
        }

        public static void HopperLineupLoop(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            float driveSpeed = DistanceLoop(ref logitechController, ref NUC_SerialConnection);

            float turnVal = TurnLoop(ref logitechController, ref NUC_SerialConnection);
            float horizontalVal = HorizontalDisplacementLoop(ref logitechController, ref NUC_SerialConnection);



            logitechController.AXES.LEFT_Y = TypeConverter.Constrain(driveSpeed - turnVal + horizontalVal, 0.1f, 1);
            logitechController.AXES.RIGHT_Y = TypeConverter.Constrain(driveSpeed + turnVal - horizontalVal, 0.1f, 1);

        }







    }
}
