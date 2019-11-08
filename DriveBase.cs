using System;
using Microsoft.SPOT;

using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;



namespace REST_2018_Robot
{
    class DriveBase
    {
        public const int NUM_MOTORS = 4;


        TalonSRX FrontLeft = new TalonSRX(1);
        TalonSRX FrontRight = new TalonSRX(2);
        TalonSRX BackLeft = new TalonSRX(3);
        TalonSRX BackRight = new TalonSRX(4);

        const short ARCADE = 1;
        const short TANK = 2;
        const short ALL_WHEEL = 3;

        short MODE = TANK;

        public DriveBase()
        {
            const int MAX_CURRENT = 10;
            const int TIMEOUT_MS = 150;

            FrontLeft.ConfigContinuousCurrentLimit(MAX_CURRENT, TIMEOUT_MS);
            FrontRight.ConfigContinuousCurrentLimit(MAX_CURRENT, TIMEOUT_MS);
            BackLeft.ConfigContinuousCurrentLimit(MAX_CURRENT, TIMEOUT_MS);
            BackRight.ConfigContinuousCurrentLimit(MAX_CURRENT, TIMEOUT_MS);

            FrontRight.SetInverted(true);
            BackRight.SetInverted(true);
        }

        public void Drive(ref XboxController controller, bool enabled)
        {
            if (enabled && MODE == TANK)
            {
                TankDrive(ref controller);
            }
            else Stop();
        }

        private void SetSpeeds(double L_Speed, double R_Speed)
        {
            FrontLeft.Set(ControlMode.PercentOutput, L_Speed);
            FrontRight.Set(ControlMode.PercentOutput, R_Speed);
            BackLeft.Set(ControlMode.PercentOutput, L_Speed);
            BackRight.Set(ControlMode.PercentOutput, R_Speed);
        }

        private void TankDrive(ref XboxController controller)
        {
           

            double LEFT_SPEED = controller.AXES.LEFT_Y;
            double RIGHT_SPEED = controller.AXES.RIGHT_Y;
            
            SetSpeeds(LEFT_SPEED, RIGHT_SPEED);
        }

        private void Stop()
        {
            SetSpeeds(0, 0);
        }

    }
}
