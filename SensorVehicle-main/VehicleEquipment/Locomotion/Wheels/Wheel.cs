using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleEquipment.Locomotion.Wheels
{
    public class Wheel
    {
        private readonly IVehicleCommunication vehicleCommunication;

        public static int speedLeft = 0;
        public static int speedRight = 0;

        public Wheel(IVehicleCommunication comWithWheel)
        {
            vehicleCommunication = comWithWheel;
        }

        /// <summary>
        /// Desired speed - Input range [-100, 100]
        /// </summary>
        /// <param name="LeftValue"></param>
        /// <param name="RightValue"></param>
        public void SetSpeed(int LeftValue, int RightValue)
        {
            byte[] bytes = new byte[3];
            bytes[0] = 0x50;
            bytes[1] = (byte)Convert(LeftValue);
            bytes[2] = (byte)Convert(RightValue);

            vehicleCommunication.Write(bytes);  // 
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
