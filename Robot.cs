using System;
using System.Collections;
using Microsoft.SPOT;
using CTRE.Phoenix;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;

namespace HERO_Code_2019 {
    class Robot {

        SerialCommsHandler NUC_SerialConnection;

        public Robot() {

            //Initialize a serial connection with the Intel NUC
            NUC_SerialConnection = new SerialCommsHandler(SerialCommsHandler.Constants.Port.Port1);
        }

        public void Run() {

            //Read incoming serial packets
            NUC_SerialConnection.ReadFromNUC();


            if (NUC_SerialConnection.isRobotEnabled()) {
                
                //Heartbeat pulse to the motors to enable them
                CTRE.Phoenix.Watchdog.Feed();

                NUC_SerialConnection.UpdateJoystickValues(ref logitechController);

                //MoveMotors
                bool enabled = logitechController.BUTTONS.LB;

                driveBase.Drive(ref logitechController, enabled);
            }
        }


        Controller logitechController = new Controller();

        DriveBase driveBase = new DriveBase();
    }

    class Controller {

        public double ApplyDeadzones(double axis_in) {
            if (axis_in <= DEADZONE || axis_in >= DEADZONE) return 0;
            else return axis_in;
        }

        const double DEADZONE = .09;

        public class Axes {
            public double LEFT_Y = 0;
            public double LEFT_X = 0;
            public double RIGHT_Y = 0;
            public double RIGHT_X = 0;

            public double LT = 0;
            public double RT = 0;
        }


        public class Buttons {
            public bool A = false;
            public bool B = false;
            public bool X = false;
            public bool Y = false;

            public bool LB = false;
            public bool RB = false;
            public bool LT = false;
            public bool RT = false;
        }

        //public int POV = -1;
        //public int POV_UP = 1;
        //public int POV_DOWN = 2;

        public Buttons BUTTONS = new Buttons();
        public Axes AXES = new Axes();
    }
}
