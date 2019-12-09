using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    class VisionDecoder {


        //POD data structures for vision data
        public struct Orientation {
            public float yaw;
            public float pitch;
            public float roll;

            public void reset() {
                yaw = 0;
                pitch = 0;
                roll = 0;
            }
        }

        public struct Location {
            public float x;
            public float y;
            public float z;

            public void reset() {
                x = 0;
                y = 0;
                z = 0;
            }
        }


        private Orientation orientation;
        private Location location;

        private Orientation orientation2;
        private Location location2;

        private static float MAX_ANGLE = 90.0f;
        private static float DISTANCE_RESOLUTION = 10.0f;

        //Initializes all orientation and location data to 0.
        public VisionDecoder() {
            orientation.reset();
            location.reset();
        }


        public void DecodeData(byte[] data) {

            //Read in PAIRS of bytes, and convert to short
            orientation.yaw = TypeConverter.Remap(TypeConverter.BytesToShort(data[1], data[2]), MAX_ANGLE);
            orientation.pitch = TypeConverter.Remap(TypeConverter.BytesToShort(data[3], data[4]), MAX_ANGLE);
            orientation.roll = TypeConverter.Remap(TypeConverter.BytesToShort(data[5], data[6]), MAX_ANGLE);

            location.x = TypeConverter.BytesToShort(data[7], data[8]) / DISTANCE_RESOLUTION;
            location.y = TypeConverter.BytesToShort(data[9], data[10]) / DISTANCE_RESOLUTION;
            location.z = TypeConverter.BytesToShort(data[11], data[12]) / DISTANCE_RESOLUTION;


            //
            orientation2.yaw = TypeConverter.Remap(TypeConverter.BytesToShort(data[13], data[14]), MAX_ANGLE);
            orientation2.pitch = TypeConverter.Remap(TypeConverter.BytesToShort(data[15], data[16]), MAX_ANGLE);
            orientation2.roll = TypeConverter.Remap(TypeConverter.BytesToShort(data[17], data[18]), MAX_ANGLE);

            location2.x = TypeConverter.BytesToShort(data[19], data[20]) / DISTANCE_RESOLUTION;
            location2.y = TypeConverter.BytesToShort(data[21], data[22]) / DISTANCE_RESOLUTION;
            location2.z = TypeConverter.BytesToShort(data[23], data[24]) / DISTANCE_RESOLUTION;



            //PrintAll();

            
        }

        public void PrintAll() {
            Debug.Print("Yaw: " + orientation.yaw.ToString() + " Pitch: " + orientation.pitch + " Roll: " + orientation.roll);
            Debug.Print("X: " + location.x.ToString() + " Y: " + location.y.ToString() + " Z: " + location.z.ToString());
            Debug.Print("");

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
