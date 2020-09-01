using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using System.Collections;


namespace HERO_Code {

    class Excavation {

        private const int ACTUATOR_STOWED_POSITION = 270;//270
        private const int ACTUATOR_RAISED_POSITION = 755;


        private TalonSRX leftActuator;
        private TalonSRX rightActuator;

        private const int STEPPER_MAX_SPEED = 250;
        private StepperMotorController augerExtender;

        private LimitSwitch forwardLimitSwitch;
        private LimitSwitch reverseLimitSwitch;

        private TalonSRX augerRotator;
        private LightSensor lightSensor;

        private CTRE.Phoenix.CANifier CANifier;


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


            //Init Canifier for controlling stepper motors
            CANifier = new CTRE.Phoenix.CANifier((ushort)CAN_IDs.ACCESSORIES.CANIFIER);

            //Initialize the forward and reverse limit switches for extending/retracting the auger
            forwardLimitSwitch = new LimitSwitch(CANifier, CTRE.Phoenix.CANifier.GeneralPin.SPI_CLK_PWM0P);
            reverseLimitSwitch = new LimitSwitch(CANifier, CTRE.Phoenix.CANifier.GeneralPin.SPI_MOSI_PWM1P);

            //Stepper Motors (controlled from one motor controller object)
            augerExtender = new StepperMotorController(CANifier, CTRE.Phoenix.CANifier.GeneralPin.SPI_CS, CTRE.HERO.IO.Port3.PWM_Pin9, STEPPER_MAX_SPEED,
                CTRE.Phoenix.CANifier.GeneralPin.SPI_CLK_PWM0P, CTRE.Phoenix.CANifier.GeneralPin.SPI_MOSI_PWM1P);
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
                    if (controller.BUTTONS.A && reverseLimitSwitch.IsPressed()) excavationState = ExcavationState.STOWING;

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


                    if (controller.BUTTONS.A && reverseLimitSwitch.IsPressed()) excavationState = ExcavationState.STOWING;
                    Debug.Print(reverseLimitSwitch.IsPressed().ToString());

                    //Control auger extension stepper motors
                    if (controller.BUTTONS.POV_UP) augerExtender.SetSpeed(-.5f);
                    else if (controller.BUTTONS.POV_DOWN) augerExtender.SetSpeed(.2f);
                    else augerExtender.Stop();

                    if (controller.BUTTONS.X) augerRotator.Set(ControlMode.PercentOutput, 0.3);
                    else if (controller.BUTTONS.B) augerRotator.Set(ControlMode.PercentOutput, -0.3);
                    else augerRotator.Set(ControlMode.PercentOutput, 0);

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
            augerRotator.Set(ControlMode.PercentOutput, 0);
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


            t = augerRotator;
            info.CAN_ID = (short)t.GetDeviceID();
            info.percentOutput = TalonInfo.ConvertPercentOutputToByte((int)(100 * t.GetMotorOutputPercent()));
            info.currentDraw = TalonInfo.ConvertCurrentToShort(t.GetOutputCurrent());
            info.encoderPosition = (short)t.GetSelectedSensorPosition();
            info.encoderVelocity = (short)t.GetSelectedSensorVelocity();
            talonInfoList.Add(new TalonInfo(info));

            info.CAN_ID = (short)CAN_IDs.EXCAVATION.AUGER_EXTENDER;
            info.percentOutput = TalonInfo.ConvertPercentOutputToByte((int)(100 * augerExtender.GetDirectionAsVBUS()));
            info.currentDraw = -1;
            int encoderPos = 0;
            if (reverseLimitSwitch.IsPressed()) encoderPos = -1;
            else if (forwardLimitSwitch.IsPressed()) encoderPos = 1;
            info.encoderPosition = encoderPos;
            info.encoderVelocity = info.percentOutput;
            talonInfoList.Add(new TalonInfo(info));
                

            return talonInfoList;
        }

    }
}
