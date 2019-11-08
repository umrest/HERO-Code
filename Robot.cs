using System;
using System.Collections;
using Microsoft.SPOT;
using CTRE.Phoenix;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;

namespace REST_2018_Robot
{
    class Robot
    {
        public Robot()
        {
            GameController _gamepad = new GameController(UsbHostDevice.GetInstance());
        }

        public void Run()
        {
            if (xboxController.IsConnected())
            {
                CTRE.Phoenix.Watchdog.Feed();

                GetSensorData();
                GetJoystickInput();

                //MoveMotors
                bool enabled = xboxController.BUTTONS.LB;

                driveBase.Drive(ref xboxController, enabled);
                lift.RunLift(ref xboxController, enabled);

            }
        }

        private void GetSensorData()
        {
            //TODO
        }

        private void GetJoystickInput()
        {
            xboxController.Update();
        }

        XboxController xboxController = new XboxController();

        DriveBase driveBase = new DriveBase();
        Lift lift = new Lift();
    }

    class XboxController
    {
        public XboxController()
        {
            this.controller = new GameController(UsbHostDevice.GetInstance());
        }

        GameControllerValues values = new GameControllerValues();

        public void Update()
        {
            controller.GetAllValues(ref values);

            //AXES
            AXES.LEFT_X = ApplyDeadzones(controller.GetAxis(0));
            AXES.LEFT_Y = ApplyDeadzones(controller.GetAxis(1));
            AXES.RIGHT_X = ApplyDeadzones(controller.GetAxis(2));
            AXES.RIGHT_Y = ApplyDeadzones(controller.GetAxis(3));
            AXES.LT = controller.GetAxis(4);
            AXES.RT = controller.GetAxis(5);

            //BUTTONS 
            BUTTONS.A = controller.GetButton(1);
            BUTTONS.B = controller.GetButton(2);
            BUTTONS.X = controller.GetButton(3);
            BUTTONS.Y = controller.GetButton(4);

            BUTTONS.LB = controller.GetButton(5);
            BUTTONS.RB = controller.GetButton(6);
            BUTTONS.LT = AXES.LT > 0;
            BUTTONS.RT = AXES.RT > 0;

            POV = values.pov;

        }

        public bool IsConnected()
        {
            return controller.GetConnectionStatus() == UsbDeviceConnection.Connected;
        }

        public double ApplyDeadzones(double axis_in)
        {
            if (axis_in <= DEADZONE || axis_in >= DEADZONE) return 0;
            else return axis_in;
        }

        const double DEADZONE = .09;

        public class Axes
        {
            public double LEFT_Y = 0;
            public double LEFT_X = 0;
            public double RIGHT_Y = 0;
            public double RIGHT_X = 0;

            public double LT = 0;
            public double RT = 0;
        }


        public class Buttons
        {
            public bool A = false;
            public bool B = false;
            public bool X = false;
            public bool Y = false;

            public bool LB = false;
            public bool RB = false;
            public bool LT = false;
            public bool RT = false;
        }

        public int POV = -1;
        public int POV_UP = 1;
        public int POV_DOWN = 2;

        private GameController controller;

        public Buttons BUTTONS = new Buttons();
        public Axes AXES = new Axes();
    }
}
