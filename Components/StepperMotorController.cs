using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace HERO_Code_2019 {
    class StepperMotorController {

        private Microsoft.SPOT.Hardware.PWM movePort;

        private CTRE.Phoenix.CANifier CANifier;
        private CTRE.Phoenix.CANifier.GeneralPin directionPort;
        private CTRE.Phoenix.CANifier.GeneralPin forwardLimitSwitch;
        private CTRE.Phoenix.CANifier.GeneralPin reverseLimitSwitch;

        public enum Direction {
            FORWARDS = 0,
            BACKWARDS = 1,
            STOPPED = 2
        }

        private Direction lastDirection;
        private uint maxSpeed;


        public StepperMotorController(CTRE.Phoenix.CANifier CANifier, CTRE.Phoenix.CANifier.GeneralPin directionPin, Microsoft.SPOT.Hardware.Cpu.PWMChannel movePin, uint maxSpeed,
            CTRE.Phoenix.CANifier.GeneralPin forwardLimitSwitchPin, CTRE.Phoenix.CANifier.GeneralPin reverseLimitSwitchPin) {

            lastDirection = Direction.STOPPED;

            this.maxSpeed = maxSpeed;

            this.directionPort = directionPin;
            this.forwardLimitSwitch = forwardLimitSwitchPin;
            this.reverseLimitSwitch = reverseLimitSwitchPin;
            this.CANifier = CANifier;

            movePort = new PWM(movePin, maxSpeed, maxSpeed / 2, PWM.ScaleFactor.Microseconds, false);
        }

        public void SetSpeed(float percentOutput) {
            if (percentOutput > 1 || percentOutput < -1 || percentOutput == 0) {
                Stop();
                return;
            }

            Direction direction = (percentOutput > 0) ? Direction.FORWARDS : Direction.BACKWARDS;

            percentOutput = percentOutput > 0 ? percentOutput : -percentOutput;
            uint speed = (UInt16)(maxSpeed / percentOutput);

            movePort.Period = speed;
            movePort.Duration = (uint)(movePort.Period / 2);

            Move(direction);
        }

        public void Stop() {
           Move(Direction.STOPPED);
        }

        private void Move(Direction direction) {
            Debug.Print((!CANifier.GetGeneralInput(forwardLimitSwitch)).ToString() + (!CANifier.GetGeneralInput(reverseLimitSwitch)).ToString());


            if (direction != lastDirection) Thread.Sleep(250);

            if (direction == Direction.FORWARDS && IsForwardLimitSwitchPressed()) {
                Stop();
              //  Debug.Print("Forward Limit Switch Tripped");
                return;
            } else if (direction == Direction.BACKWARDS && IsReverseLimitSwitchPressed()) {
                Stop();
              //  Debug.Print("Reverse Limit Switch Tripped");
                return;
            }

            lastDirection = direction;

            if (direction == Direction.STOPPED) {
                movePort.Stop();
                return;
            }

            CANifier.SetGeneralOutput(directionPort, direction == Direction.FORWARDS, true);

            movePort.Start();
        }


        public bool IsForwardLimitSwitchPressed() { return ! CANifier.GetGeneralInput(forwardLimitSwitch); }
        public bool IsReverseLimitSwitchPressed() { return CANifier.GetGeneralInput(reverseLimitSwitch); }






    }
}
