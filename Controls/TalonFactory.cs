using System;
using Microsoft.SPOT;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;

namespace HERO_Code_2019 {
    public static class TalonFactory {

        //DriveBase
        public static TalonSRX CreateDriveBaseTalon(int CAN_ID, bool inverted = false) {
            TalonSRX talon = new TalonSRX(CAN_ID);

            const int MAX_CURRENT = 10;

            talon.ConfigContinuousCurrentLimit(MAX_CURRENT);
            talon.ConfigPeakCurrentLimit(2 * MAX_CURRENT);
            talon.ConfigPeakCurrentDuration(0);
            talon.SetInverted(inverted);
            return talon;
        }


        //Excavation
        public static TalonSRX CreateLinearActuator(int CAN_ID) {
            TalonSRX talon = new TalonSRX(CAN_ID);

            talon.ConfigSelectedFeedbackSensor(FeedbackDevice.Analog);
            talon.ConfigFeedbackNotContinuous(true, 100);
            talon.SetInverted(true);
            talon.SetSensorPhase(true);

            talon.ConfigPeakCurrentLimit(6);
            talon.ConfigPeakCurrentDuration(150);
            talon.ConfigContinuousCurrentLimit(3);

            // talon.SetStatusFramePeriod(StatusFrame.Status_1_General_, 25); // Sets encoder value feedback rate -TODO

            return talon;
        }

        public static TalonSRX CreateAugerRotator(int CAN_ID) {
            TalonSRX talon = new TalonSRX(CAN_ID);




            return talon;
        }

    }

}
