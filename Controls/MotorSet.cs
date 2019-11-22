using System;
using Microsoft.SPOT;
using System.Collections;

namespace HERO_Code_2019 {
    abstract class MotorSet {

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
            //list.Add(new TalonInfo(1,2.0,3));
        }

        public abstract 

    }
}
