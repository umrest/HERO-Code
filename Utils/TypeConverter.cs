using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    static class TypeConverter {

        
        
        public static short BytesToShort(byte lowByte, byte highByte) {
            return (short)(((highByte & 0xFF) << 8) | (lowByte & 0xFF));
        }

        public static byte[] ToByteArray(short val) {
            byte[] byteArray = new byte[2];

            byteArray[0] = (byte)val;
            byteArray[1] = (byte)(val >> 8);

            return byteArray;
        }

        public static byte[] ToByteArray(int val) {
            const int size = sizeof(int);

            byte[] byteArray = new byte[size];

            for (int i = 0; i < size; i++) {
                byteArray[i] = (byte)val;
                val = val >> 8;
            }

            return byteArray;
        }

        public static byte[] ToByteArray(long val) {
            const int size = sizeof(long);

            byte[] byteArray = new byte[size];

            for (int i = 0; i < size; i++) {
                byteArray[i] = (byte)val;
                val = val >> 8;
            }

            return byteArray;
        }

        public static float Remap(short val, float high) {
     
            Debug.Assert(high != 0);

            float max = short.MaxValue;

            return (val / max) * high;
        }

        public static float Constrain(float val, float low, float high) {
            if (val < low) return low;
            if (val > high) return high;
            return val;
        }

        public static int Constrain(int val, int low, int high) {
            if (val < low) return low;
            if (val > high) return high;
            return val;
        }
        
        

    }
}
