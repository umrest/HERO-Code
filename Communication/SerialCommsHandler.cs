using System;
using Microsoft.SPOT;
using System.Collections;



namespace HERO_Code {
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

            public const int KEY_SIZE = 3;

            public static byte[] PacketSize = {
                //Read
                0,      // Padding  0
                8,      // Joystick 1
                31,     // Vision   2
                0,0,0,0,0,0, // [3,8]
                9,      // Dashboard_READ 9

                //Write
                TalonInfo.NUM_BYTES * 10,    // Motor Info OUT
                1,      //Dashboard State OUT
                0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
          
                //Write
            };


            public const int BAUD_RATE = 115200;

            //Packet type encoding/key in the first byte of an incoming packet
            public static class PacketType {
                public const int JOYSTICK_TYPE = 1;
                public const int VISION = 2;
                public const int REALESENSE = 3;
                public const int DASHBOARD_IN = 9;

                //OUT
                public const int MOTOR_INFO_OUT = 10;
                public const int DASHBOARD_OUT = 11;
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


            if (NUC_serialPort.BytesToRead > 250) {
                Debug.Print("Buffer overflowing !!!!");
                NUC_serialPort.DiscardInBuffer();
                Debug.Assert(NUC_serialPort.BytesToRead < 250);
            }

            //Wait for at least 4 Bytes (3 key bytes and type byte)
            if (NUC_serialPort.BytesToRead > Constants.KEY_SIZE) {

                //Verify the packet key. If Invalid, discard the buffer
                if ((NUC_serialPort.ReadByte() != KEY0) || (NUC_serialPort.ReadByte() != KEY1) || (NUC_serialPort.ReadByte() != KEY2)) {
                    Debug.Print("Invalid three byte key!!!");
                    NUC_serialPort.DiscardInBuffer();
                    return;
                }

                //Read the packet type, create array for the incoming bytes sized based on the packet type
                //Incoming packet is read into this array
                byte type = (byte)NUC_serialPort.ReadByte();
                byte packetSize = Constants.PacketSize[type];
                byte[] in_bytes = new byte[Constants.PacketSize[type]];


                //Block until whole packet arrives in serial buffer
                while (NUC_serialPort.BytesToRead < packetSize) continue;


                //Read the next packet into an empty byte array
                NUC_serialPort.Read(in_bytes, 0, packetSize);

                //Access the packet type from the first byte, and send the byte aray to the appropriate decoder



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
            joystickDecoder.UpdateJoystickValues(ref controller);
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
        public bool HasVisionConnection() {
            return visionDecoder.GetHasVisionConnection();
        }

        public VisionDecoder.Orientation GetVisionOrientation_HopperLineup() {
            return visionDecoder.GetOrientation_HopperLineup();
        }

        public VisionDecoder.Location GetVisionLocation_HopperLineup() {
            return visionDecoder.GetLocation_HopperLineup();
        }
        public VisionDecoder.Orientation GetVisionOrientation_FieldNavigation() {
            return visionDecoder.GetOrientation_FieldNavigation();
        }

        public VisionDecoder.Location GetVisionLocation_FieldNavigation() {
            return visionDecoder.GetLocation_FieldNavigation();
        }

        public VisionDecoder.AbsolutePosition GetAbsolutePosition() {
            return visionDecoder.GetAbsolutePosition();
        }


        // ------------------- WRITING ------------------- //

        public void WriteDashboardState() {
            //Write the dashboard
            NUC_serialPort.WriteByte(KEY0);
            NUC_serialPort.WriteByte(KEY1);
            NUC_serialPort.WriteByte(KEY2);

            NUC_serialPort.WriteByte(Constants.PacketType.DASHBOARD_OUT);
            NUC_serialPort.WriteByte((byte)GetControlMode());

        }


        public void WriteMotorInfo(ref ArrayList talonInfoList) {

            //Write the motor
            NUC_serialPort.WriteByte(KEY0);
            NUC_serialPort.WriteByte(KEY1);
            NUC_serialPort.WriteByte(KEY2);

            byte[] motorInfoPacket = new byte[Constants.PacketSize[Constants.PacketType.MOTOR_INFO_OUT]];


            NUC_serialPort.WriteByte(Constants.PacketType.MOTOR_INFO_OUT);


            int idx = 0;
            foreach (Object o in talonInfoList) {

                TalonInfo talonInfo = (TalonInfo)o;

                talonInfo.GetDataAsByteArray().CopyTo(motorInfoPacket, TalonInfo.NUM_BYTES * idx);

                idx++;
            }

            NUC_serialPort.Write(motorInfoPacket, 0, motorInfoPacket.Length);


        }
    }
}
