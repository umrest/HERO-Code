using System;
using Microsoft.SPOT;

namespace HERO_Code {
    static class CAN_IDs {

        public static class DRIVE_BASE {
            public static int FRONT_LEFT_WHEEL = 01;
            public static int FRONT_RIGHT_WHEEL = 02;
            public static int BACK_LEFT_WHEEL = 03;
            public static int BACK_RIGHT_WHEEL = 04;
        }

        public static class EXCAVATION {
            public static int LEFT_ACTUATOR = 11;
            public static int RIGHT_ACTUATOR = 12;

            public static int AUGER_ROTATOR = 13;
            public static int AUGER_EXTENDER = 14;
        }

        public static class COLLECTION {
            public static int LEFT_DUMPER = 21;
            public static int RIGHT_DUMPER = 22;
        }

        public static class ACCESSORIES {
            public static int CANIFIER = 41;
            public static int PIGEON_IMU = 42;
        }



    }
}
