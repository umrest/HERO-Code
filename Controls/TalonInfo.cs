using System;
using Microsoft.SPOT;

namespace HERO_Code_2019 {
    public class TalonInfo {

        public TalonInfo(TalonInfo info) {
            this.CAN_ID = info.CAN_ID;
            this.currentDraw = info.currentDraw;
            this.encoderPosition = info.encoderPosition;
            this.encoderVelocity = info.encoderVelocity;
        }

        public TalonInfo(short CAN_ID, short currentDraw, long encoderPosition, int encoderVelocity) {
            this.CAN_ID = CAN_ID;
            this.currentDraw = currentDraw;
            this.encoderPosition = encoderPosition;
            this.encoderVelocity = encoderVelocity;

        }

        public TalonInfo(short CAN_ID, float currentDraw, long encoderPosition, int encoderVelocity) {
            this.CAN_ID = CAN_ID;
            this.currentDraw = ConvertCurrentToShort(currentDraw);
            this.encoderPosition = encoderPosition;
            this.encoderVelocity = encoderVelocity;

        }

        public TalonInfo() {
            this.CAN_ID = -1;
        }


        public short CAN_ID;
        public short currentDraw;
        public long encoderPosition;
        public int encoderVelocity;

        public const int NUM_BYTES = 15;


        public static short ConvertCurrentToShort(float currentDraw) {
            return (short)(currentDraw * 100);
        }


        public byte[] GetDataAsByteArray() {

            byte[] byteArray = new byte[NUM_BYTES];
            byte[] tempArray = new byte[8];

            byteArray[0] = (byte)CAN_ID;


            tempArray = TypeConverter.ToByteArray(currentDraw);
            byteArray[1] = tempArray[0];
            byteArray[2] = tempArray[1];

            tempArray = TypeConverter.ToByteArray(encoderPosition);
            byteArray[3] = tempArray[0];
            byteArray[4] = tempArray[1];
            byteArray[5] = tempArray[2];
            byteArray[6] = tempArray[3];

            byteArray[7] = tempArray[4];
            byteArray[8] = tempArray[5];
            byteArray[9] = tempArray[6];
            byteArray[10] = tempArray[7];

            tempArray = TypeConverter.ToByteArray(encoderVelocity);
            byteArray[11] = tempArray[0];
            byteArray[12] = tempArray[1];
            byteArray[13] = tempArray[2];
            byteArray[14] = tempArray[3];


            return byteArray;
        }

        
    }
}
