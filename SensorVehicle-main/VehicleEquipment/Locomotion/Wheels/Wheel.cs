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
        }

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

        private bool _hasUnacknowledgedError;
        public bool HasUnacknowledgedError
        {
            get { return _hasUnacknowledgedError; }
            set { SetProperty(ref _hasUnacknowledgedError, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            private set { SetProperty(ref _message, value); }
        }

        public void ClearMessage()
        {
            Message = "";
            HasUnacknowledgedError = false;
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
            if ((onlySendIfValuesChanged && leftValue == CurrentSpeedLeft && rightValue == CurrentSpeedRight) || HasUnacknowledgedError) return;

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
                Message += $"Error when setting speed: \n{e.Message}\n\nStacktrace: \n{e.StackTrace}\n**************";
                Debug.WriteLine($"ERROR when writing to wheel: {e}");
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
