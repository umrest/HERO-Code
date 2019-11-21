using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    class VisionDecoder {


        //POD data structures for vision data
        public struct Orientation {
            public short yaw;
            public short pitch;
            public short roll;

            public void reset() {
                yaw = 0;
                pitch = 0;
                roll = 0;
            }
        }

        public struct Location {
            public short x;
            public short y;
            public short z;

            public void reset() {
                x = 0;
                y = 0;
                z = 0;
            }
        }


        private Orientation orientation;
        private Location location;

        //Initializes all orientation and location data to 0.
        public VisionDecoder() {
            orientation.reset();
            location.reset();
        }


        public void DecodeData(byte[] data) {

            //Read in PAIRS of bytes, and convert to short
            orientation.yaw = TypeConverter.BytesToShort(data[1], data[2]);
            orientation.pitch = TypeConverter.BytesToShort(data[3], data[4]);
            orientation.roll = TypeConverter.BytesToShort(data[5], data[6]);

            location.x = TypeConverter.BytesToShort(data[7], data[8]);
            location.y = TypeConverter.BytesToShort(data[9], data[10]);
            location.z = TypeConverter.BytesToShort(data[11], data[12]);

            
        }

        //Getters for orientation and location data
        public Orientation GetOrientation() {
            return orientation;
        }

        public Location GetLocation() {
            return location;
        }

    }
}
