using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    static class TypeConverter {
        
        public static short BytesToShort(byte lowByte, byte highByte) {
            return (short)(((highByte & 0xFF) << 8) | (lowByte & 0xFF));
        }


    }
}
