﻿using System;
using System.Diagnostics;
using Helpers;

namespace VehicleEquipment.Locomotion.Wheels
{
    public class Wheel : ThreadSafeNotifyPropertyChanged, IWheel
    {
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
            if (onlySendIfValuesChanged && leftValue == CurrentSpeedLeft && rightValue == CurrentSpeedRight) return;

            try
            {
                vehicleCommunication.Write(MessageCode.NoMessage, ValidatedSpeed(leftValue), ValidatedSpeed(rightValue));

                CurrentSpeedLeft = leftValue;
                CurrentSpeedRight = rightValue;
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
