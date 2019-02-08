using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    public class Ultrasonic : IUltrasonic
    {
        private readonly IVehicleCommunication vehicleCommunication;

        public Ultrasonic(IVehicleCommunication comWithUltrasonic)
        {
            vehicleCommunication = comWithUltrasonic;

        }
        public float DistanceLeft()
        {
            return Convert.ToSingle(Distance()[0]);
        }

        public float DistanceForward()
        {
            return Convert.ToSingle(Distance()[1]);
        }

        public float DistanceRight()
        {
            return Convert.ToSingle(Distance()[2]);
        }

        private string[] Distance()
        {
            byte[] bytes = new byte[20];
            string text = "";
            bytes = vehicleCommunication.Read();
            foreach (byte b in bytes)
            {
                text = text + (char)b;
            }

            return text.Split('-');
        }
    }
}
