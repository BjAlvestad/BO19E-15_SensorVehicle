using System;
using System.Collections.Generic;
using System.Text;

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

        /// <summary>
        /// Desired speed - Input range [-100, 100]
        /// </summary>
        /// <param name="LeftValue"></param>
        /// <param name="RightValue"></param>
        public void SetSpeed(int LeftValue, int RightValue)
        {
            if (LeftValue == CurrentSpeedLeft && RightValue == CurrentSpeedRight) return;

            byte[] bytes = new byte[3];
            bytes[0] = 0x50;
            bytes[1] = (byte)Convert(LeftValue);
            bytes[2] = (byte)Convert(RightValue);

            vehicleCommunication.Write(bytes);  // 

            CurrentSpeedLeft = bytes[1] - 128;
            CurrentSpeedRight = bytes[2] - 128;
        }

        /// <summary>
        /// Converts input value from range [-100, 100] to [28, 228]
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private int Convert(int value)
        {
            if (value > 100)
            {
                return 228;
            }
            else if (value < -100)
            {
                return 28;
            }
            else
            {
                return value + 128;
            }
        }
    }
}
