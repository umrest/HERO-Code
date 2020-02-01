using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using System.Collections;


namespace HERO_Code_2019 {

    class Excavation {

        private const int ACTUATOR_STOWED_POSITION = 270;
        private const int ACTUATOR_RAISED_POSITION = 755;


        private TalonSRX leftActuator;
        private TalonSRX rightActuator;

        private const int STEPPER_MAX_SPEED = 250;
        private StepperMotorController augerExtender;

        private TalonSRX augerRotator;


        private LightSensor lightSensor;



        //private TalonSRX AugerRotator = new TalonSRX(13);
        //private TalonSRX LeftAugerExtender = new TalonSRX(14);
        //private TalonSRX RightAugerExtender = new TalonSRX(15);



        //Definitions for Excavation State Machine
        ExcavationState excavationState;

        private enum ExcavationState {
            INIT,
            STOWED,
            STOWING,
            RAISED,
            RAISING,
            EXCAVATING,
            DUMPING
        }


        //Setup
        public Excavation() {
            excavationState = ExcavationState.INIT;

            //Actuators
            leftActuator = TalonFactory.CreateLinearActuator(CAN_IDs.EXCAVATION.LEFT_ACTUATOR);
            rightActuator = TalonFactory.CreateLinearActuator(CAN_IDs.EXCAVATION.RIGHT_ACTUATOR);

            //Stepper Motors (controlled from one motor controller object)
            augerExtender = new StepperMotorController(CTRE.HERO.IO.Port3.Pin3, CTRE.HERO.IO.Port3.PWM_Pin9, STEPPER_MAX_SPEED);
            augerExtender.Stop();

            augerRotator = TalonFactory.CreateAugerRotator(CAN_IDs.EXCAVATION.AUGER_ROTATOR);

            //Light sensor for detecting full excavation tube
            lightSensor = new LightSensor(CTRE.HERO.IO.Port8.Analog_Pin5);
        }

        //Update calls, run every robot loop
        public void Update(ref Controller controller, bool enabled) {

            if (!enabled) {
                StopAll();
                return;
            }


            UpdateStateMachine(ref controller);

            // Debug.Print(leftActuator.GetSelectedSensorPosition().ToString() + ",   " + rightActuator.GetSelectedSensorPosition() + ",    " + excavationState);
        }

        private void UpdateStateMachine(ref Controller controller) {
            switch (excavationState) {

                //Initial State
                case ExcavationState.INIT:
                    if (controller.BUTTONS.Y) excavationState = ExcavationState.RAISING;
                    if (controller.BUTTONS.A) excavationState = ExcavationState.STOWING;

                    StopAll();
                    break;


                //Static States
                case ExcavationState.STOWED:
                    StopAll();

                    if (controller.BUTTONS.Y) excavationState = ExcavationState.RAISING;
                    break;

                case ExcavationState.RAISED:
                    leftActuator.Set(ControlMode.PercentOutput, 0);
                    rightActuator.Set(ControlMode.PercentOutput, 0);


                    if (controller.BUTTONS.A) excavationState = ExcavationState.STOWING;


                    //Control auger extension stepper motors
                    if (controller.BUTTONS.POV_UP) augerExtender.SetSpeed(-0.1f);
                    else if (controller.BUTTONS.POV_DOWN) augerExtender.SetSpeed(0.1f);
                    else augerExtender.Stop();

                    break;

                //Dynamic States
                case ExcavationState.RAISING:
                    if (controller.BUTTONS.A) excavationState = ExcavationState.STOWING;
                    if (GoToActuatorPosition(ACTUATOR_RAISED_POSITION)) excavationState = ExcavationState.RAISED;

                    augerExtender.Stop();

                    break;

                case ExcavationState.STOWING:
                    if (controller.BUTTONS.Y) excavationState = ExcavationState.RAISING;
                    if (GoToActuatorPosition(ACTUATOR_STOWED_POSITION)) excavationState = ExcavationState.STOWED;

                    augerExtender.Stop();
                    break;



                case ExcavationState.EXCAVATING:
                    break;
                case ExcavationState.DUMPING:
                    break;

                default:
                    Debug.Print("ERROR: Invalid Excavation State Accessed!");
                    break;
            }
        }







        //Motor Control

        private void StopAll() {
            leftActuator.Set(ControlMode.PercentOutput, 0);
            rightActuator.Set(ControlMode.PercentOutput, 0);
            augerExtender.Stop();

            //AugerRotator.Set(ControlMode.PercentOutput, 0);
            //LeftAugerExtender.Set(ControlMode.PercentOutput, 0);
            //RightAugerExtender.Set(ControlMode.PercentOutput, 0);

        }



        bool stopped = true;
        private bool GoToActuatorPosition(int position) {

            int distanceToGo = position - leftActuator.GetSelectedSensorPosition();
            int direction = distanceToGo > 0 ? 1 : -1;
            const float magnitude = 1.0f;

            if (!stopped && (distanceToGo * direction) < 2) stopped = true;
            else if (stopped && (distanceToGo * direction) > 5) stopped = false;

            if (!stopped) {

                int posDifference = leftActuator.GetSelectedSensorPosition() - rightActuator.GetSelectedSensorPosition();

                int sign = posDifference > 0 ? 1 : -1;
                const float P_Value = .05f;

                float lSpeed = (direction * magnitude) - (P_Value * posDifference);
                float rSpeed = (direction * magnitude) + (P_Value * posDifference);

                leftActuator.Set(ControlMode.PercentOutput, lSpeed);
                rightActuator.Set(ControlMode.PercentOutput, rSpeed);

                //   Debug.Print(posDifference.ToString());
                return false;
            }

            StopAll();
            return true;


        }



        public ArrayList GetTalonInfo() {
            ArrayList talonInfoList = new ArrayList();

            TalonSRX t;
            TalonInfo info = new TalonInfo();

            t = leftActuator;
            info.CAN_ID = (short)t.GetDeviceID();
            info.percentOutput = TalonInfo.ConvertPercentOutputToByte((int)(100 * t.GetMotorOutputPercent()));
            info.currentDraw = TalonInfo.ConvertCurrentToShort(t.GetOutputCurrent());
            info.encoderPosition = (short)t.GetSelectedSensorPosition();
            info.encoderVelocity = (short)t.GetSelectedSensorVelocity();

            talonInfoList.Add(new TalonInfo(info));

            t = rightActuator;
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
