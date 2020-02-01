using System;
using Microsoft.SPOT;
using System.Collections;


using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;



namespace HERO_Code_2019 {
    class DriveBase {
        private TalonSRX FrontLeft;
        private TalonSRX FrontRight;
        private TalonSRX BackLeft;
        private TalonSRX BackRight;

        private const short ARCADE = 1;
        private const short TANK = 2;
        private const short ALL_WHEEL = 3;

        private short MODE;

        //Initializes the 4 talons, 1 for each wheel
        //Assigns a current limit of 10 A for a continous time of 150 ms
        public DriveBase(short mode = TANK) {
            MODE = mode;

            FrontLeft = TalonFactory.CreateDriveBaseTalon(CAN_IDs.DRIVE_BASE.FRONT_LEFT_WHEEL);
            FrontRight = TalonFactory.CreateDriveBaseTalon(CAN_IDs.DRIVE_BASE.FRONT_RIGHT_WHEEL);
            BackLeft = TalonFactory.CreateDriveBaseTalon(CAN_IDs.DRIVE_BASE.BACK_LEFT_WHEEL, true);
            BackRight = TalonFactory.CreateDriveBaseTalon(CAN_IDs.DRIVE_BASE.BACK_RIGHT_WHEEL, true);
       
        }

        //Main drive function, called from the robot class
        public void Update(ref Controller controller, bool enabled) {
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
            info.percentOutput = TalonInfo.ConvertPercentOutputToByte((int)(100 * t.GetMotorOutputPercent()));
            info.currentDraw = TalonInfo.ConvertCurrentToShort(t.GetOutputCurrent());
            info.encoderPosition = (short) t.GetSelectedSensorPosition();
            info.encoderVelocity = (short) t.GetSelectedSensorVelocity();
            
            talonInfoList.Add(new TalonInfo(info));

            t = FrontRight;
            info.CAN_ID = (short)t.GetDeviceID();
            info.percentOutput = TalonInfo.ConvertPercentOutputToByte((int)(100 * t.GetMotorOutputPercent()));
            info.currentDraw = TalonInfo.ConvertCurrentToShort(t.GetOutputCurrent());
            info.encoderPosition = (short)t.GetSelectedSensorPosition();
            info.encoderVelocity = (short)t.GetSelectedSensorVelocity();
            talonInfoList.Add(new TalonInfo(info));

            t = BackLeft;
            info.CAN_ID = (short)t.GetDeviceID();
            info.percentOutput = TalonInfo.ConvertPercentOutputToByte((int)(100 * t.GetMotorOutputPercent()));
            info.currentDraw = TalonInfo.ConvertCurrentToShort(t.GetOutputCurrent());
            info.encoderPosition = (short)t.GetSelectedSensorPosition();
            info.encoderVelocity = (short)t.GetSelectedSensorVelocity();
            talonInfoList.Add(new TalonInfo(info));

            t = BackRight;
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
