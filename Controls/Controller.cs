using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {

    class Controller {

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


        //Helper methods

        //Treat any joystick values within the DEADZONE threshold of 0 as 0 itself
        public double ApplyDeadzones(double axis_in) {
            if (axis_in <= DEADZONE || axis_in >= DEADZONE) return 0;
            else return axis_in;
        }
    }
}
