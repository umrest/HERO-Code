using System;
using System.Collections;
using Microsoft.SPOT;
using CTRE.Phoenix;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;
using CTRE.Phoenix.Sensors;

namespace HERO_Code {
    class Robot {

        public const bool CAN_NETWORK_IS_ENABLED = true;

        public const int BATTERY_VOLTAGE = 24;

        //Create a serial connection with the NUC
        SerialCommsHandler NUC_SerialConnection;

        //Initialize the handler for switching between different control modes (ie Teleop, Autonomous, Test, etc)
        ControlModeHandler controlModeHandler;

        //Initialize a logitech joystick object
        Controller logitechController;

        //Initialize the drive base motor controllers
        DriveBase driveBase = new DriveBase();

        //Initialize the Excavator motor controllers
        Excavation excavation = new Excavation();


        public Robot() {

            //Initialize a serial connection with the Intel NUC
            NUC_SerialConnection = new SerialCommsHandler(SerialCommsHandler.Constants.Port.Port1);

            //Initialize the handler for switching between different control modes (ie Teleop, Autonomous, Test, etc)
            controlModeHandler = new ControlModeHandler();

            //Initialize a logitech joystick object
            logitechController = new Controller();

            //Initialize the drive base motor controllers
            driveBase = new DriveBase();

        }


        // ---------- ROBOT MAIN LOOP ---------- //
        public void Run() {

            //Read incoming serial packets
            NUC_SerialConnection.ReadFromNUC();

            //Check for dashboard enable signal in order for the robot to be enabled
            bool ROBOT_ENABLED = NUC_SerialConnection.IsRobotEnabled();

            //Reset the robot control mode to disabled
            controlModeHandler.SetMode(ControlModeHandler.ControlMode.DISABLED);


            if (ROBOT_ENABLED) {

                //Heartbeat pulse to the motors to enable them
                //Motor controllers will disable automatically if this is not regularly received
                CTRE.Phoenix.Watchdog.Feed();



                //Update the robot control mode and send over the logitech controller object to be updated
                if (ROBOT_ENABLED) {
                    controlModeHandler.SetMode(NUC_SerialConnection.GetControlMode());
                    controlModeHandler.updateControllerValues(ref logitechController, ref NUC_SerialConnection);
                }

                //MoveMotors
                if (CAN_NETWORK_IS_ENABLED) {
                    // Debug.Print("Fix BackRight shaft key DB");
                    driveBase.Update(ref logitechController, controlModeHandler.IsRobotActive());
                    excavation.Update(ref logitechController, controlModeHandler.IsRobotActive());
                }
            }


           

            //Serial write to Dashboard with talon motor data
            if (CAN_NETWORK_IS_ENABLED) PackageTalonInfo();

        }

        byte cntr = 0;
        private void PackageTalonInfo() {
            if ((cntr++) % 100 != 0) return;

            NUC_SerialConnection.WriteDashboardState();

            ArrayList talonInfoList = new ArrayList();

            foreach (TalonInfo info in driveBase.GetTalonInfo()) talonInfoList.Add(info);
            foreach (TalonInfo info in excavation.GetTalonInfo()) talonInfoList.Add(info);

            NUC_SerialConnection.WriteMotorInfo(ref talonInfoList);
        }


    }

}
