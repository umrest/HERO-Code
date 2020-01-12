using System;
using Microsoft.SPOT;
using System.Collections;


namespace HERO_Code_2019 {
    class SerialCommsHandler {

        //Serial connection
        System.IO.Ports.SerialPort NUC_serialPort;

        //Decoders
        DashboardStateDecoder dashboardDecoder;
        JoystickDecoder joystickDecoder;
        VisionDecoder visionDecoder;


        //Look for three-byte key at start of packet in order to prevent weird offsets
        const byte KEY0 = 44;
        const byte KEY1 = 254;
        const byte KEY2 = 153;


        //Serial comms constants
        public static class Constants {

            public const int PACKET_SIZE_READ = 128;
            public const int PACKET_SIZE_WRITE = 230;

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


        // ------------------- READING ------------------- //

        //Reads an incoming packet from the NUC into a byte array,
        //And sends it to the appropriate decoder based on the pack ID (the first byte)
        public void ReadFromNUC() {

            //Assert that the packet size is less than the size of the serial buffer
            // (The buffer physically in the serial chip itself)
            Debug.Assert(Constants.PACKET_SIZE_READ <= 256);


            //Incoming packet is read into this array
            byte[] in_bytes = new byte[Constants.PACKET_SIZE_READ];


            if (NUC_serialPort.BytesToRead > 250) {
                Debug.Print("Buffer overflowing !!!!");
                NUC_serialPort.DiscardInBuffer();
                Debug.Assert(NUC_serialPort.BytesToRead < 250);
            }

            //Wait for at least 12
            if (NUC_serialPort.BytesToRead >= Constants.PACKET_SIZE_READ + 3) {




                if ((NUC_serialPort.ReadByte() != KEY0) || (NUC_serialPort.ReadByte() != KEY1) || (NUC_serialPort.ReadByte() != KEY2)) {
                    Debug.Print("Invalid three byte key!!!");
                    NUC_serialPort.DiscardInBuffer();
                    return;
                }


                //Read the next packet into an empty byte array
                NUC_serialPort.Read(in_bytes, 0, Constants.PACKET_SIZE_READ);

                //Access the packet type from the first byte, and send the byte aray to the appropriate decoder
                byte type = in_bytes[0];


                switch (type) {

                    //Enable or disable state and control mode of robot
                    case Constants.PacketType.DASHBOARD_IN:
                        dashboardDecoder.DecodeDashboardStateData(in_bytes);
                        break;

                    //Joystick data for teleop mode
                    case Constants.PacketType.JOYSTICK_TYPE:
                        joystickDecoder.DecodeJoystickData(in_bytes);
                        break;

                    //Vision data - orientation and location
                    case Constants.PacketType.VISION:
                        visionDecoder.DecodeData(in_bytes);
                        break;

                    //Error message for unknown packet type
                    default:
                        Debug.Print("ERROR - Unknown or invalid packet type: " + type);
                        break;
                }
            }
        }


        //Updates the values of an input controller object with the data from a joystick packet
        public void UpdateJoystickValues(ref Controller controller) {
            joystickDecoder.updateJoystickValues(ref controller);
        }


        //Returns the robot enable status, decoded from the dashboard packet
        public bool IsRobotEnabled() {
            return dashboardDecoder.IsEnabled();
        }

        //Return the control mode of the robot
        public short GetControlMode() {
            return dashboardDecoder.GetControlMode();
        }

        //Returns Orientation and Location data structures from vision calculations
        public VisionDecoder.Orientation GetVisionOrientation() {
            return visionDecoder.GetOrientation();
        }

        public VisionDecoder.Location GetVisionLocation() {
            return visionDecoder.GetLocation();
        }


        // ------------------- WRITING ------------------- //

        public void WriteToNUC(ref ArrayList talonInfoList) {

            NUC_serialPort.WriteByte(KEY0);
            NUC_serialPort.WriteByte(KEY1);
            NUC_serialPort.WriteByte(KEY2);

            byte[] packet = new byte[Constants.PACKET_SIZE_WRITE];


            NUC_serialPort.WriteByte(Constants.PacketType.DASHBOARD_OUT);


            int idx = 0;
            foreach (Object o in talonInfoList) {


                TalonInfo talonInfo = (TalonInfo)o;

                talonInfo.GetDataAsByteArray().CopyTo(packet, TalonInfo.NUM_BYTES * idx);

                idx++;
            }

            NUC_serialPort.Write(packet, 0, Constants.PACKET_SIZE_WRITE);


        }
    }
}
