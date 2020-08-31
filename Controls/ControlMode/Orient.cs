using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace HERO_Code {
    class Orient {

        CTRE.Phoenix.Stopwatch stopwatch;

        private bool foundVisionTargets = false;
        private bool potentialMatch = false;

        private uint potentialMatchTime = 0;
        private static uint POTENTIAL_MATCH_TIMEOUT = 500; //ms

        private uint turningTime = 0;
        private static uint TURNING_TIMOUT = 5000;

        private uint unburyingTime = 0;
        private static uint UNBURYING_TIMOUT = 1750;

        private enum OrientState {
            INIT,
            LOCATING_VISION_TARGETS,
            UNBURYING,
            ORIENTING_TOWARDS_MINING_ZONE
        }

        OrientState orientState;

        public Orient() {
            orientState = OrientState.INIT;
            stopwatch = new CTRE.Phoenix.Stopwatch();
            stopwatch.Start();
        }

        public bool Update(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {

            switch (orientState) {
                case OrientState.INIT:
                    orientState = OrientState.LOCATING_VISION_TARGETS;
                    turningTime = stopwatch.DurationMs;
                    break;

                case OrientState.LOCATING_VISION_TARGETS:

                    if (LocateVisionTargets(ref logitechController, ref NUC_SerialConnection)) {
                        orientState = OrientState.ORIENTING_TOWARDS_MINING_ZONE;
                    } else if (stopwatch.DurationMs - turningTime > TURNING_TIMOUT) {
                        orientState = OrientState.UNBURYING;
                        unburyingTime = stopwatch.DurationMs;
                    }

                    break;

                case OrientState.UNBURYING:

                    if (Unburying(ref logitechController, ref NUC_SerialConnection)) {
                        turningTime = stopwatch.DurationMs;
                        orientState = OrientState.LOCATING_VISION_TARGETS;
                    }

                    break;


                case OrientState.ORIENTING_TOWARDS_MINING_ZONE:
                    logitechController.ResetValues();
                    Debug.Print("DONE");
                    break;

                default:
                    break;
            }

            return false;
        }

        private bool LocateVisionTargets(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            float speed = 1.0f;

            if (NUC_SerialConnection.HasVisionConnection()) {

                if (!potentialMatch) {
                    potentialMatch = true;
                    potentialMatchTime = stopwatch.DurationMs;
                } else if ((stopwatch.DurationMs - potentialMatchTime) > POTENTIAL_MATCH_TIMEOUT) return true;

                if (NUC_SerialConnection.GetAbsolutePosition().yaw > 0) speed = 0; //Change to less than (yaw < 0) if LEFT_Y set to negative speed

            } else {
                potentialMatch = false;
            }

            logitechController.AXES.LEFT_Y = speed;
            logitechController.AXES.RIGHT_Y = -speed;

            return false;

        }

        private bool Unburying(ref Controller logitechController, ref SerialCommsHandler NUC_SerialConnection) {
            logitechController.AXES.LEFT_Y = -1;
            logitechController.AXES.RIGHT_Y = -1;

            return stopwatch.DurationMs - unburyingTime > UNBURYING_TIMOUT;
        }

    }
}
