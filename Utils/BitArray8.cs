using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    class BitArray8 {

        //Stores eight bits
        private byte data = 0;

        public BitArray8(byte data) {
            this.data = data;
        }

        public void SetBit(int pos, bool value) {
            if (value) {
                //left-shift 1, then bitwise OR
                data = (byte)(data | (1 << pos));
            } else {
                //left-shift 1, then take complement, then bitwise AND
                data = (byte)(data & ~(1 << pos));
            }
        }

        public bool GetBit(int pos) {
            //left-shift 1, then bitwise AND, then check for non-zero
            return ((data & (1 << pos)) != 0);
        }
    }
}
