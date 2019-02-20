using System;
using System.Diagnostics;

namespace VehicleEquipment.Locomotion.Wheels
{
    public class Wheel : IWheel
    {
        private readonly IVehicleCommunication vehicleCommunication;

        public Wheel(IVehicleCommunication comWithWheel)
        {
            vehicleCommunication = comWithWheel;
        }

        public int CurrentSpeedLeft { get; private set; }
        public int CurrentSpeedRight { get; private set; }

        public void Fwd(int speed = 50)
        {
            SetSpeed(speed, speed);
        }

        public void TurnLeft(int speed = 50)
        {
            SetSpeed(-speed, speed);
        }

        public void TurnRight(int speed = 50)
        {
            SetSpeed(speed, -speed);
        }

        public void Reverse(int speed = 50)
        {
            SetSpeed(-speed, -speed);
        }

        public void Stop()
        {
            SetSpeed(0, 0);
        }

        /// <summary>
        /// Sends desired speed to wheel encoder - Input range [-100, 100]
        /// </summary>
        /// <param name="LeftValue">Left wheel speed (valid value is between -100 and +100)</param>
        /// <param name="RightValue">Right wheel speed (valid value is between -100 and +100)</param>
        public void SetSpeed(int LeftValue, int RightValue)
        {
            if (LeftValue == CurrentSpeedLeft && RightValue == CurrentSpeedRight) return;

            try
            {
                vehicleCommunication.Write(MessageCode.NoMessage, ValidatedSpeed(LeftValue), ValidatedSpeed(RightValue));

                CurrentSpeedLeft = LeftValue;
                CurrentSpeedRight = RightValue;
            }
            catch (Exception e)
            {
                CurrentSpeedLeft = 999;
                CurrentSpeedRight = 999;
                Debug.WriteLine($"ERROR when writing to wheel: {e.Message}");
            }
        }

        private static int ValidatedSpeed(int value)
        {
            if (value > 100) return 100;
            if (value < -100) return -100;

            return value;
        }
    }
}
