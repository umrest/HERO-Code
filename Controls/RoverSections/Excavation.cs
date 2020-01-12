using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using System.Collections;


namespace HERO_Code_2019 {



    class Excavation {

        private static int ACTUATOR_STOWED_POSITION = 350;
        private static int ACTUATOR_RAISED_POSITION = 700;

        private TalonSRX LeftActuator = new TalonSRX(11);
        private TalonSRX RightActuator = new TalonSRX(12);

        private LightSensor lightSensor = new LightSensor(CTRE.HERO.IO.Port8.Analog_Pin5);

        //private TalonSRX AugerRotator = new TalonSRX(13);
        //private TalonSRX LeftAugerExtender = new TalonSRX(14);
        //private TalonSRX RightAugerExtender = new TalonSRX(15);


        ExcavationState excavationState;

        private enum ExcavationState {
            STOWED,
            STOWING,
            RAISED,
            RAISING,
            EXCAVATING,
            DUMPING
        }

        public Excavation() {
            excavationState = ExcavationState.STOWED;

            //Left Actuator

            LeftActuator.ConfigSelectedFeedbackSensor(FeedbackDevice.Analog);
            LeftActuator.SetInverted(true);
            LeftActuator.SetSensorPhase(true);

            LeftActuator.ConfigPeakCurrentLimit(6);
            LeftActuator.ConfigPeakCurrentDuration(150);
            LeftActuator.ConfigContinuousCurrentLimit(3);

            //Right Actuator

            RightActuator.ConfigSelectedFeedbackSensor(FeedbackDevice.Analog);
            RightActuator.SetInverted(true);
            RightActuator.SetSensorPhase(true);

            RightActuator.ConfigPeakCurrentLimit(6);
            RightActuator.ConfigPeakCurrentDuration(150);
            RightActuator.ConfigContinuousCurrentLimit(3);



        }

        public void Update(ref Controller controller, bool enabled) {

            if (!enabled) {
                Stop();
                return;
            }

            UpdateStateMachine(ref controller);

        }


        private void UpdateStateMachine(ref Controller controller) {
            switch (excavationState) {

                //Static States
                case ExcavationState.STOWED:
                    Stop();

                    if (controller.BUTTONS.Y) excavationState = ExcavationState.RAISING;
                    break;

                case ExcavationState.RAISED:
                    Stop();

                    if (controller.BUTTONS.A) excavationState = ExcavationState.STOWING;
                    break;

                //Dynamic States
                case ExcavationState.RAISING:
                    GoToActuatorPosition(ACTUATOR_RAISED_POSITION);
                    break;

                case ExcavationState.STOWING:
                    GoToActuatorPosition(ACTUATOR_STOWED_POSITION);
                    break;

                default:
                    Debug.Print("ERROR: Invalid Excavation State Accessed!");
                    break;
            }
        }







        //Motor Control

        private void Stop() {
            LeftActuator.Set(ControlMode.PercentOutput, 0);
            RightActuator.Set(ControlMode.PercentOutput, 0);

            //AugerRotator.Set(ControlMode.PercentOutput, 0);
            //LeftAugerExtender.Set(ControlMode.PercentOutput, 0);
            //RightAugerExtender.Set(ControlMode.PercentOutput, 0);

        }



        bool stopped = true;
        private void GoToActuatorPosition(int position) {

            int distanceToGo = position - LeftActuator.GetSelectedSensorPosition();
            int direction = distanceToGo > 0 ? 1 : -1;
            const float magnitude = 1.0f;

            if (!stopped && (distanceToGo * direction) < 5) stopped = true;
            else if (stopped && (distanceToGo * direction) > 15) stopped = false;

            if (!stopped) {

                int posDifference = LeftActuator.GetSelectedSensorPosition() - RightActuator.GetSelectedSensorPosition();

                int sign = posDifference > 0 ? 1 : -1;
                const float P_Value = .05f;

                float lSpeed = (direction * magnitude) - (P_Value * posDifference);
                float rSpeed = (direction * magnitude) + (P_Value * posDifference);

                LeftActuator.Set(ControlMode.PercentOutput, lSpeed);
                RightActuator.Set(ControlMode.PercentOutput, rSpeed);

                Debug.Print(posDifference.ToString());

            } else Stop();
        }



        public ArrayList GetTalonInfo() {
            ArrayList talonInfoList = new ArrayList();

            TalonSRX t;
            TalonInfo info = new TalonInfo();

            t = LeftActuator;
            info.CAN_ID = (short)t.GetDeviceID();
            info.percentOutput = TalonInfo.ConvertPercentOutputToByte((int)(100 * t.GetMotorOutputPercent()));
            info.currentDraw = TalonInfo.ConvertCurrentToShort(t.GetOutputCurrent());
            info.encoderPosition = (short)t.GetSelectedSensorPosition();
            info.encoderVelocity = (short)t.GetSelectedSensorVelocity();

            talonInfoList.Add(new TalonInfo(info));

            t = RightActuator;
            info.CAN_ID = (short)t.GetDeviceID();
            info.percentOutput = TalonInfo.ConvertPercentOutputToByte((int)(100 * t.GetMotorOutputPercent()));
            info.currentDraw = TalonInfo.ConvertCurrentToShort(t.GetOutputCurrent());
            info.encoderPosition = (short)t.GetSelectedSensorPosition();
            info.encoderVelocity = (short)t.GetSelectedSensorVelocity();
            talonInfoList.Add(new TalonInfo(info));

            return talonInfoList;
        }

    }
}
