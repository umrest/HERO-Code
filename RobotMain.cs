using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using CTRE.Phoenix.Controller;
using CTRE.Phoenix.MotorControl;
using CTRE.Phoenix.MotorControl.CAN;

namespace REST_2018_Robot
{
    public class RobotMain
    {

        //Robot startup
        public static void Main()
        {
            Robot robot = new Robot();

            //Main run thread
            while (true)
            {
                robot.Run();

                Utils.Delay(5);
            }
        }
    }
}
