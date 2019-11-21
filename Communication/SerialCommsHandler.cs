using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    class SerialCommsHandler {

        //Serial connection
        System.IO.Ports.SerialPort NUC_serialPort;

        //Decoders
        DashboardStateDecoder dashboardDecoder;
        JoystickDecoder joystickDecoder;
        VisionDecoder visionDecoder;


        //Serial comms constants
        public static class Constants {

            public const int PACKET_SIZE = 128;
            public const int BAUD_RATE = 115200;

            //Packet type encoding/key in the first byte of an incoming packet
            public static class PacketType {
                public const int JOYSTICK_TYPE = 1;
                public const int VISION = 2;
                public const int REALESENSE = 3;
                public const int DASHBOARD_IN = 9;
                public const int DASHBOARD_OUT = 10;
            }


            //References to the 3 UART ports on the HERO board
            public static class Port {
                public static string Port1 = CTRE.HERO.IO.Port1.UART;
                public static string Port4 = CTRE.HERO.IO.Port4.UART;
                public static string Port7 = CTRE.HERO.IO.Port6.UART;
            }

        }


        //Creates a serial object with specified port and baudrate
        public SerialCommsHandler(string PortName) {
            NUC_serialPort = new System.IO.Ports.SerialPort(PortName, Constants.BAUD_RATE);
            NUC_serialPort.Open();
            NUC_serialPort.DiscardInBuffer();

            dashboardDecoder = new DashboardStateDecoder();
            joystickDecoder = new JoystickDecoder();
            visionDecoder = new VisionDecoder();
        }


        //Reads an incoming packet from the NUC into a byte array,
        //And sends it to the appropriate decoder based on the pack ID (the first byte)
        public void ReadFromNUC() {

            //Assert that the packet size is less than the size of the serial buffer
            // (The buffer physically in the serial chip itself)
            Debug.Assert(Constants.PACKET_SIZE <= 256);


            //Incoming packet is read into this array
            byte[] in_bytes = new byte[Constants.PACKET_SIZE];


            if (NUC_serialPort.BytesToRead > 250) {
                Debug.Print("Buffer overflowing !!!!");
                NUC_serialPort.DiscardInBuffer();
                Debug.Assert(NUC_serialPort.BytesToRead < 250);
            }

            //Wait for at least 12
            if (NUC_serialPort.BytesToRead >= Constants.PACKET_SIZE + 3) {

                if ( (NUC_serialPort.ReadByte() != 44) || (NUC_serialPort.ReadByte() != 254) || (NUC_serialPort.ReadByte() != 153)) {
                    NUC_serialPort.DiscardInBuffer();
                    return;
                }


                //Read the next packet into an empty byte array
                NUC_serialPort.Read(in_bytes, 0, Constants.PACKET_SIZE);

                //Access the packet type from the first byte, and send the byte aray to the appropriate decoder
                byte type = in_bytes[0];

                
                //Enable or disable state of robot
                if (type == Constants.PacketType.DASHBOARD_IN) {

                    dashboardDecoder.DecodeDashboardStateData(in_bytes);

                //Joystick data for teleop mode
                } else if (type == Constants.PacketType.JOYSTICK_TYPE) {

                    joystickDecoder.DecodeJoystickData(in_bytes);

                //Vision data - orientation and location
                } else if (type == Constants.PacketType.VISION) {

                    visionDecoder.DecodeData(in_bytes);

                //Unknown packet ID
                } else Debug.Print("INVALID PACKET TYPE: " + type);


            }
        }


        //Updates the values of an input controller object with the data from a joystick packet
        public void UpdateJoystickValues(ref Controller controller) {
            joystickDecoder.updateJoystickValues(ref controller);
        }


        //Returns the robot enable status, decoded from the dashboard packet
        public bool isRobotEnabled() {
            return dashboardDecoder.IsEnabled;
        }

        //Returns Orientation and Location data structures from vision calculations
        public VisionDecoder.Orientation GetVisionOrientation() {
            return visionDecoder.GetOrientation();
        }

        public VisionDecoder.Location GetVisionLocation() {
            return visionDecoder.GetLocation();
        }
    }
}
