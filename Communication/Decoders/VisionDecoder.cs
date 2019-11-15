using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    class VisionDecoder {

        private short yaw;
        private short pitch;
        private short roll;

        private short x;
        private short y;
        private short z;



        public void DecodeData(byte[] data) {

            yaw = TypeConverter.BytesToShort(data[1], data[2]);
            pitch = TypeConverter.BytesToShort(data[3], data[4]);
            roll = TypeConverter.BytesToShort(data[5], data[6]);
            x = TypeConverter.BytesToShort(data[7], data[8]);
            y = TypeConverter.BytesToShort(data[9], data[10]);
            z = TypeConverter.BytesToShort(data[11], data[12]);

            //.Print(yaw.ToString());
            Debug.Print(yaw.ToString());
        }

        public int GetYaw() {
            return yaw;
        }

    }
}
