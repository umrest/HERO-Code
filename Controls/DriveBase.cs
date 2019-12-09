using System;
using Microsoft.SPOT;
using System.Collections;


using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;



namespace HERO_Code_2019 {
    class DriveBase {
        private TalonSRX FrontLeft = new TalonSRX(1);
        private TalonSRX FrontRight = new TalonSRX(2);
        private TalonSRX BackLeft = new TalonSRX(8);
        private TalonSRX BackRight = new TalonSRX(7);

        private const short ARCADE = 1;
        private const short TANK = 2;
        private const short ALL_WHEEL = 3;

        private short MODE;

        //Initializes the 4 talons, 1 for each wheel
        //Assigns a current limit of 10 A for a continous time of 150 ms
        public DriveBase(short mode = TANK) {
            MODE = mode;

            const int MAX_CURRENT = 5;

            FrontLeft.ConfigContinuousCurrentLimit(MAX_CURRENT);
            FrontRight.ConfigContinuousCurrentLimit(MAX_CURRENT);
            BackLeft.ConfigContinuousCurrentLimit(MAX_CURRENT);
            BackRight.ConfigContinuousCurrentLimit(MAX_CURRENT);

            FrontLeft.ConfigPeakCurrentLimit(2 * MAX_CURRENT);
            FrontLeft.ConfigPeakCurrentDuration(0);

            FrontRight.ConfigPeakCurrentLimit(2 * MAX_CURRENT);
            FrontRight.ConfigPeakCurrentDuration(0);

            BackLeft.ConfigPeakCurrentLimit(2 * MAX_CURRENT);
            BackLeft.ConfigPeakCurrentDuration(0);

            BackRight.ConfigPeakCurrentLimit(2 * MAX_CURRENT);
            BackRight.ConfigPeakCurrentDuration(0);



            FrontRight.SetInverted(true);
            BackRight.SetInverted(true);

        }

        //Main drive function, called from the robot class
        public void Drive(ref Controller controller, bool enabled) {
            //Debug.Print("Pos: " + FrontLeft.GetSelectedSensorPosition().ToString()
            //    + "Velocity: " + FrontLeft.GetSelectedSensorVelocity()
            //    + "Current: " + FrontLeft.GetOutputCurrent().ToString());


            if (enabled && MODE == TANK) {
                TankDrive(ref controller);
            } else Stop();
        }



        //Different modes of controlling the robot from the same joystick input
        private void TankDrive(ref Controller controller) {
            double LEFT_SPEED = controller.AXES.LEFT_Y;
            double RIGHT_SPEED = controller.AXES.RIGHT_Y;

            SetSpeeds(LEFT_SPEED, RIGHT_SPEED);
        }


        //Halts all motors, as opposed to continuing with the previous speed
        private void Stop() {
            SetSpeeds(0, 0);
        }

        //Helper function for setting the speed of each side
        //TODO - Modify to be wheel specific to make traction-control possible
        private void SetSpeeds(double L_Speed, double R_Speed) {
            FrontLeft.Set(ControlMode.PercentOutput, L_Speed);
            FrontRight.Set(ControlMode.PercentOutput, R_Speed);
            BackLeft.Set(ControlMode.PercentOutput, L_Speed);
            BackRight.Set(ControlMode.PercentOutput, R_Speed);
        }

        public ArrayList GetTalonInfo() {
            ArrayList talonInfoList = new ArrayList();

            TalonSRX t; 
            TalonInfo info = new TalonInfo();

            t = FrontLeft;
            info.CAN_ID = (short) t.GetDeviceID();
            info.currentDraw = TalonInfo.ConvertCurrentToShort(t.GetOutputCurrent());
            info.encoderPosition = (short) t.GetSelectedSensorPosition();
            info.encoderVelocity = (short) t.GetSelectedSensorVelocity();
            talonInfoList.Add(new TalonInfo(info));

            t = FrontRight;
            info.CAN_ID = (short)t.GetDeviceID();
            info.currentDraw = TalonInfo.ConvertCurrentToShort(t.GetOutputCurrent());
            info.encoderPosition = (short)t.GetSelectedSensorPosition();
            info.encoderVelocity = (short)t.GetSelectedSensorVelocity();
            talonInfoList.Add(new TalonInfo(info));

            t = BackLeft;
            info.CAN_ID = (short)t.GetDeviceID();
            info.currentDraw = TalonInfo.ConvertCurrentToShort(t.GetOutputCurrent());
            info.encoderPosition = (short)t.GetSelectedSensorPosition();
            info.encoderVelocity = (short)t.GetSelectedSensorVelocity();
            talonInfoList.Add(new TalonInfo(info));

            t = BackRight;
            info.CAN_ID = (short)t.GetDeviceID();
            info.currentDraw = TalonInfo.ConvertCurrentToShort(t.GetOutputCurrent());
            info.encoderPosition = (short)t.GetSelectedSensorPosition();
            info.encoderVelocity = (short)t.GetSelectedSensorVelocity();
            talonInfoList.Add(new TalonInfo(info));

            return talonInfoList;
        }

    }
}
