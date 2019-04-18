using System;
using System.Diagnostics;
using Helpers;

namespace VehicleEquipment.Locomotion.Wheels
{
    public class Wheel : ThreadSafeNotifyPropertyChanged, IWheel
    {
        private const int MaximumValidSpeed = 100;

        private readonly IVehicleCommunication vehicleCommunication;

        public Wheel(IVehicleCommunication comWithWheel)
        {
            vehicleCommunication = comWithWheel;
            Error = new Error();
        }

        public Error Error { get; }

        private int _currentSpeedLeft;
        public int CurrentSpeedLeft
        {
            get { return _currentSpeedLeft; }
            private set { SetPropertyRaiseSelectively(ref _currentSpeedLeft, value); }
        }

        private int _currentSpeedRight;
        public int CurrentSpeedRight
        {
            get { return _currentSpeedRight; }
            private set { SetPropertyRaiseSelectively(ref _currentSpeedRight, value); }
        }

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
        /// <param name="leftValue">Left wheel speed (valid value is between -100 and +100)</param>
        /// <param name="rightValue">Right wheel speed (valid value is between -100 and +100)</param>
        /// <param name="onlySendIfValuesChanged">If set to false, new command will be sent to wheels, even if command is the same as old.</param>
        public void SetSpeed(int leftValue, int rightValue, bool onlySendIfValuesChanged = true)
        {
            if ((onlySendIfValuesChanged && leftValue == CurrentSpeedLeft && rightValue == CurrentSpeedRight) || Error.Unacknowledged) return;

            try
            {
                int validatedSpeedLeft = ValidatedSpeed(leftValue);
                int validatedSpeedRight = ValidatedSpeed(rightValue);

                vehicleCommunication.Write(MessageCode.NoMessage, validatedSpeedLeft, validatedSpeedRight);

                CurrentSpeedLeft = validatedSpeedLeft;
                CurrentSpeedRight = validatedSpeedRight;
            }
            catch (Exception e)
            {
                CurrentSpeedLeft = 999;
                CurrentSpeedRight = 999;
                Error.Message = $"Error when setting wheel speed...\n{e.Message}";
                Error.DetailedMessage = e.ToString();
            }
        }

        private static int ValidatedSpeed(int value)
        {
            if (value > MaximumValidSpeed) return MaximumValidSpeed;
            if (value < -MaximumValidSpeed) return -MaximumValidSpeed;

            return value;
        }
    }
}
