using System;
using CTRE.Native;
using CTRE.Phoenix;
using Microsoft.SPOT;

namespace HERO_Code {
    class LimitSwitch {

        private CTRE.Phoenix.CANifier CANifier;
        private CTRE.Phoenix.CANifier.GeneralPin CANifierPin;
        public LimitSwitch(CTRE.Phoenix.CANifier CANifier, CTRE.Phoenix.CANifier.GeneralPin CANifierPin) {
            this.CANifier = CANifier;
            this.CANifierPin = CANifierPin;
        }

        public bool IsPressed() {
            return !CANifier.GetGeneralInput(CANifierPin);
        }
    }
}
