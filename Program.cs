using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;


namespace NUC_SerialTest {
    public class Program {

        const int JOYSTICK_TYPE = 1;
        const int DASHBOARD_TYPE = 9;

        public static void Main() {

            System.IO.Ports.SerialPort NUC_serialPort = new System.IO.Ports.SerialPort(CTRE.HERO.IO.Port1.UART, 115200);
            NUC_serialPort.Open();

            int s = 0;


            NUC_serialPort.DiscardInBuffer();

            /* loop forever */
            while (true) {
                byte[] _rx = new byte[128];
                CTRE.Phoenix.Watchdog.Feed();

                if (NUC_serialPort.BytesToRead >= 128) {


                    
                    int size = NUC_serialPort.Read(_rx, 0, 128);


                    byte type = _rx[0];


                    if (type == DASHBOARD_TYPE) {
                        DecodeDashboardState.DecodeDashboardStateData(_rx);
                    } else if (type == JOYSTICK_TYPE) {
                        JoystickData.DecodeJoystickData(_rx);
                    } else Debug.Print("INVALID TYPE: " + type);

                   
                } 

                /* wait a bit */
                System.Threading.Thread.Sleep(1);
            }
        }
    }
}
