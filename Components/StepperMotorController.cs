using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace HERO_Code_2019 {
    class StepperMotorController {

        private OutputPort directionPort;
        private PWM movePort;

        public enum Direction {
            FORWARDS = 0,
            BACKWARDS = 1,
            STOPPED = 2
        }

        private Direction lastDirection;
        private uint maxSpeed;

        public StepperMotorController(Cpu.Pin directionPin, Microsoft.SPOT.Hardware.Cpu.PWMChannel movePin, uint maxSpeed) {
            lastDirection = Direction.STOPPED;

            this.maxSpeed = maxSpeed;

            directionPort = new OutputPort(directionPin, false);
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

            if (direction != lastDirection) {
                //movePort.Stop();
                Thread.Sleep(250);
            }

            lastDirection = direction;

            if (direction == Direction.STOPPED) {
                movePort.Stop();
                return;
            }

            directionPort.Write(direction == Direction.FORWARDS);

            movePort.Start();
        }

        //public void MoveOneTick() {
        //    if (direction == Direction.STOPPED) return;

        //    

        //    lastMovement = !lastMovement;
        //    movePort.Write(lastMovement);
        //}





    }
}
