using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    class HopperLineup {



        private static float P_Loop(float pos, float maxPos, float desiredPos = 0) {
            float P = 1.0f / (maxPos - desiredPos);          

            pos = TypeConverter.Constrain(pos, -maxPos + desiredPos, maxPos + desiredPos);

            Debug.Print(P.ToString() + ", " + pos.ToString() + ", " + ((pos - desiredPos) * P).ToString());

            return (pos - desiredPos) * P;
        }

        public static float TurnLoop(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            float yaw = NUC_SerialConnection.GetVisionOrientation().yaw;

            float driveSpeed = HopperLineup.P_Loop(yaw, 90.0f, 0);

            logitechController.AXES.LEFT_Y = -driveSpeed;
            logitechController.AXES.RIGHT_Y = driveSpeed;

            return driveSpeed;
        }
        public static float DistanceLoop(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            float dist = NUC_SerialConnection.GetVisionLocation().z;


            float driveSpeed = HopperLineup.P_Loop(dist, 30.0f, 8.0f);

            logitechController.AXES.LEFT_Y = driveSpeed;
            logitechController.AXES.RIGHT_Y = driveSpeed;

            Debug.Print(driveSpeed.ToString());

            return driveSpeed;
        }

        public static void HopperLineupLoop(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            float driveSpeed = DistanceLoop(ref logitechController, ref NUC_SerialConnection);
            float turnVal = TurnLoop(ref logitechController, ref NUC_SerialConnection);

            turnVal = turnVal * 0.5f;

            logitechController.AXES.LEFT_Y = driveSpeed - turnVal;
            logitechController.AXES.RIGHT_Y = driveSpeed + turnVal;

        }

        





    }
}
