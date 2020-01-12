using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace HERO_Code_2019 {
    class LightSensor {

        public static double DARK_THRESHOLD = 0.3;

        private AnalogInput analogInput;

        public LightSensor(Cpu.AnalogChannel analogPin) {
            analogInput = new AnalogInput(analogPin);
        }

        public double GetRawValue() {
            return analogInput.Read();
        }


        public bool IsLightDetected() {
            return analogInput.Read() < DARK_THRESHOLD;
        }

        public void printString() {
            Debug.Print(IsLightDetected() ? "LIGHT" : "DARK");

        }



    }
}
