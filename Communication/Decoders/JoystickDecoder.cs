using System;
using System.IO;
using Microsoft.SPOT;

namespace HERO_Code {

    class JoystickDecoder {

        private bool button_a;
        private bool button_b;
        private bool button_x;
        private bool button_y;

        private bool button_rb;
        private bool button_lb;
        private bool button_select;
        private bool button_start;
        private bool button_lj;
        private bool button_rj;

        private bool button_povUp;
        private bool button_povRight;
        private bool button_povDown;
        private bool button_povLeft;

        private float lj_x;
        private float lj_y;
        private float rj_x;
        private float rj_y;

        //TODO
        //private  float lt;
        //private  float rt;

        public void DecodeJoystickData(byte[] data) {

            //Extract the individual bits from 2 bytes
            //and assign to button values
            BitArray8 button_array1 = new BitArray8(data[0]);
            BitArray8 button_array2 = new BitArray8(data[1]);

            button_a = button_array1.GetBit(0);
            button_b = button_array1.GetBit(1);
            button_x = button_array1.GetBit(2);
            button_y = button_array1.GetBit(3);
            button_lb = button_array1.GetBit(4);
            button_rb = button_array1.GetBit(5);
            button_select = button_array1.GetBit(6);
            button_start = button_array1.GetBit(7);

            button_lj = button_array2.GetBit(0);
            button_rj = button_array2.GetBit(1);

            button_povUp = button_array2.GetBit(2);
            button_povRight = button_array2.GetBit(3);
            button_povDown = button_array2.GetBit(4);
            button_povLeft = button_array2.GetBit(5);

            //Read joystick axes
            //Invert the y axes so that positive values are up on the joystick


            //            lj_x = TypeConverter.Remap( (short) data[3]) - middle, 1.0f);



            lj_x = ByteToFloat(data[2]);
            lj_y = -ByteToFloat(data[3]);
            rj_x = ByteToFloat(data[4]);
            rj_y = -ByteToFloat(data[5]);


            //Debug Only
            // PrintAllValues();
        }


        //Print all joystick buttons and axes
        private void PrintAllValues() {

            Debug.Print("A Button: " + button_a.ToString());
            Debug.Print("B Button: " + button_b.ToString());
            Debug.Print("X Button: " + button_x.ToString());
            Debug.Print("Y Button: " + button_y.ToString());

            Debug.Print("LB Button: " + button_lb.ToString());
            Debug.Print("RB Button: " + button_rb.ToString());
            Debug.Print("Select Button: " + button_select.ToString());
            Debug.Print("Start Button: " + button_start.ToString());
            Debug.Print("LJ Button: " + button_lj.ToString());
            Debug.Print("RJ Button: " + button_rj.ToString());

            Debug.Print("");
            Debug.Print("ljx: " + lj_x);
            Debug.Print("ljy: " + lj_y);
            Debug.Print("rjx: " + rj_x);
            Debug.Print("rjy: " + rj_y);
        }


        //Maps an input axis byte [0:255] to a float [-1:1]
        private float ByteToFloat(byte value_in) {
            float value = (float)value_in;

            float mid = 255 / 2;

            value -= mid;
            value /= mid;

            return value;
        }

        public void UpdateJoystickValues(ref Controller controller) {

            //AXES
            controller.AXES.LEFT_X = lj_x;
            controller.AXES.LEFT_Y = lj_y;
            controller.AXES.RIGHT_X = rj_x;
            controller.AXES.RIGHT_Y = rj_y;



            //BUTTONS 
            controller.BUTTONS.A = button_a;
            controller.BUTTONS.B = button_b;
            controller.BUTTONS.X = button_x;
            controller.BUTTONS.Y = button_y;

            controller.BUTTONS.LB = button_lb;
            controller.BUTTONS.RB = button_rb;

            controller.BUTTONS.START = button_start;
            controller.BUTTONS.SELECT = button_select;

            controller.BUTTONS.LJ = button_lj;
            controller.BUTTONS.RJ = button_rj;

            controller.BUTTONS.POV_UP = button_povUp;
            controller.BUTTONS.POV_RIGHT = button_povRight;
            controller.BUTTONS.POV_DOWN = button_povDown;
            controller.BUTTONS.POV_LEFT = button_povLeft;

            //controller.AXES.LT = controller.GetAxis(4);
            //controller.AXES.RT = controller.GetAxis(5);
            //controller.BUTTONS.LT = controller.AXES.LT > 0;
            //controller.BUTTONS.RT = controller.AXES.RT > 0;
        }
    }
}



