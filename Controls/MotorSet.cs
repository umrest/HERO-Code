using System;
using Microsoft.SPOT;
using System.Collections;

namespace HERO_Code {
    class MotorSet {

        public class TalonInfo {
            public TalonInfo(short CAN_ID, float currentDraw, long encoderValue) {
                this.CAN_ID = CAN_ID;
                this.currentDraw = currentDraw;
                this.encoderValue = encoderValue;
            }


            public short CAN_ID;
            public float currentDraw;
            public long encoderValue;
        }


        private ArrayList list;

        public MotorSet() {

            list = new ArrayList();


            list.Add(new TalonInfo(1, 2.0f, 3));

        }

        
    }
}
