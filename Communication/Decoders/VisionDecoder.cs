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


        private Orientation orientation_FieldNavigation;
        private Location location_FieldNavigation;

        private Orientation orientation_HopperLineup;
        private Location location_HopperLineup;

        private static float MAX_ANGLE = 90.0f;
        private static float DISTANCE_RESOLUTION = 10.0f;

        //Initializes all orientation and location data to 0.
        public VisionDecoder() {
            orientation_FieldNavigation.reset();
            location_FieldNavigation.reset();
        }


        public void DecodeData(byte[] data) {

            //Read in PAIRS of bytes, and convert to short

            //LARGE field location tag
            orientation_FieldNavigation.yaw = TypeConverter.Remap(TypeConverter.BytesToShort(data[0], data[1]), MAX_ANGLE);
            orientation_FieldNavigation.pitch = TypeConverter.Remap(TypeConverter.BytesToShort(data[2], data[3]), MAX_ANGLE);
            orientation_FieldNavigation.roll = TypeConverter.Remap(TypeConverter.BytesToShort(data[4], data[5]), MAX_ANGLE);

            location_FieldNavigation.x = TypeConverter.BytesToShort(data[6], data[7]) / DISTANCE_RESOLUTION;
            location_FieldNavigation.y = TypeConverter.BytesToShort(data[8], data[9]) / DISTANCE_RESOLUTION;
            location_FieldNavigation.z = TypeConverter.BytesToShort(data[10], data[11]) / DISTANCE_RESOLUTION;


            //SMALL - Hopper lineup tag
            orientation_HopperLineup.yaw = TypeConverter.Remap(TypeConverter.BytesToShort(data[12], data[13]), MAX_ANGLE);
            orientation_HopperLineup.pitch = TypeConverter.Remap(TypeConverter.BytesToShort(data[14], data[15]), MAX_ANGLE);
            orientation_HopperLineup.roll = TypeConverter.Remap(TypeConverter.BytesToShort(data[16], data[17]), MAX_ANGLE);

            location_HopperLineup.x = TypeConverter.BytesToShort(data[18], data[19]) / DISTANCE_RESOLUTION;
            location_HopperLineup.y = TypeConverter.BytesToShort(data[20], data[21]) / DISTANCE_RESOLUTION;
            location_HopperLineup.z = TypeConverter.BytesToShort(data[22], data[23]) / DISTANCE_RESOLUTION;



           //PrintAll();

            
        }

        public void PrintAll() {
            Debug.Print("Yaw: " + orientation_FieldNavigation.yaw.ToString() + " Pitch: " + orientation_FieldNavigation.pitch + " Roll: " + orientation_FieldNavigation.roll);
            Debug.Print("X: " + location_FieldNavigation.x.ToString() + " Y: " + location_FieldNavigation.y.ToString() + " Z: " + location_FieldNavigation.z.ToString());
            Debug.Print("");

        }

        //Getters for orientation and location data
        public Orientation GetOrientation_HopperLineup() {
            return orientation_HopperLineup;
        }

        public Location GetLocation_HopperLineup() {
            return location_HopperLineup;
        }


        //
        public Orientation GetOrientation_FieldNavigation() {
            return orientation_FieldNavigation;
        }

        public Location GetLocation_FieldNavigation() {
            return location_FieldNavigation;
        }

    }
}
