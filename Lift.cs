using System;
using Microsoft.SPOT;

using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;

namespace REST_2018_Robot
{
    class Lift
    {
        public const int NUM_MOTORS = 2;

        TalonSRX LeftActuator = new TalonSRX(5);
        TalonSRX RightActuator = new TalonSRX(6);

        TalonSRX ExcavationBelt = new TalonSRX(7);
        TalonSRX CollectionBelt = new TalonSRX(8);

        TalonSRX Digger = new TalonSRX(9);

        public Lift()
        {
            const int MAX_CURRENT = 15;
            const int TIMEOUT_MS = 100;

            LeftActuator.ConfigContinuousCurrentLimit(MAX_CURRENT, TIMEOUT_MS);
            RightActuator.ConfigContinuousCurrentLimit(MAX_CURRENT, TIMEOUT_MS);

            LeftActuator.SetNeutralMode(NeutralMode.Brake);
            RightActuator.SetNeutralMode(NeutralMode.Brake);

            LeftActuator.ConfigSelectedFeedbackSensor(FeedbackDevice.Analog);
            RightActuator.ConfigSelectedFeedbackSensor(FeedbackDevice.Analog);

            const float P = 33.57f;
            const float I = 0;


            LeftActuator.Config_kP(P);
            RightActuator.Config_kP(P);
            LeftActuator.Config_kI(I);
            RightActuator.Config_kI(I);

            LeftActuator.ConfigMotionAcceleration(150);
            RightActuator.ConfigMotionAcceleration(150);
            RightActuator.ConfigMotionCruiseVelocity(4);
            LeftActuator.ConfigMotionCruiseVelocity(4);


            ExcavationBelt.ConfigContinuousCurrentLimit(MAX_CURRENT, TIMEOUT_MS); //REMOVE ME EVENTUALLY
            ExcavationBelt.SetInverted(true);
            CollectionBelt.ConfigContinuousCurrentLimit(MAX_CURRENT, TIMEOUT_MS);

            Digger.ConfigContinuousCurrentLimit(MAX_CURRENT, TIMEOUT_MS);
            Digger.SetInverted(true);
        }

        public void RunLift(ref XboxController controller, bool enabled)
        {
            if (enabled)
            {
                RunActuators(ref controller);
                RunBelts(ref controller);
                RunDigger(ref controller);
            }
            else Stop();
        }

        double OUTPUT = 500;

        private void RunActuators(ref XboxController controller)
        {
            //Update Encoders
            Utils.Print("R, L CURRENT");
            Utils.Print(OUTPUT);
            Utils.Print(RightActuator.GetSelectedSensorPosition());

            Utils.Print("");

            //Run Motors

            if (controller.POV == controller.POV_UP) OUTPUT = 300;
            else if (controller.POV == controller.POV_DOWN) OUTPUT = 800;

            //if ((OUTPUT < RightActuator.GetSelectedSensorPosition() && RightActuator.GetSelectedSensorVelocity() > 1)
            //    || (OUTPUT > RightActuator.GetSelectedSensorPosition() && RightActuator.GetSelectedSensorVelocity() < 1))
            //{
            //    LeftActuator.Set(ControlMode.PercentOutput, 0);
            //    RightActuator.Set(ControlMode.PercentOutput, 0);
            //}
            //else
            //{
                LeftActuator.Set(ControlMode.MotionMagic, OUTPUT);
                RightActuator.Set(ControlMode.MotionMagic, OUTPUT);
            //}

        }

        private void RunBelts(ref XboxController controller)
        {
            double SPEED = 1;

            if (controller.BUTTONS.RB) SPEED = -.7;

            if (controller.BUTTONS.A) ExcavationBelt.Set(ControlMode.PercentOutput, SPEED);
            else ExcavationBelt.Set(ControlMode.PercentOutput, 0);

            if (controller.BUTTONS.Y) CollectionBelt.Set(ControlMode.PercentOutput, SPEED);
            else CollectionBelt.Set(ControlMode.PercentOutput, 0);
        }

        private void RunDigger(ref XboxController controller)
        {
            if (controller.BUTTONS.LT)
            {
                Digger.Set(ControlMode.PercentOutput, 1);
            }

            else if (controller.BUTTONS.RT)
            {
                Digger.Set(ControlMode.PercentOutput, -1);
            }

            else Digger.Set(ControlMode.PercentOutput, 0);
        }

        private void Stop()
        {
            LeftActuator.Set(ControlMode.PercentOutput, 0);
            RightActuator.Set(ControlMode.PercentOutput, 0);
            CollectionBelt.Set(ControlMode.PercentOutput, 0);
            ExcavationBelt.Set(ControlMode.PercentOutput, 0);
            Digger.Set(ControlMode.PercentOutput, 0);
        }
    }
}
