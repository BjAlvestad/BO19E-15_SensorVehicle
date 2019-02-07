using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    class Ultrasonic
    {
        private readonly IVehicleCommunication vehicleCommunication;

        public Ultrasonic(IVehicleCommunication comWithUltrasonic)
        {
            vehicleCommunication = comWithUltrasonic;

        }
        public string DistanceLeft()
        {
            return Distance()[0];
        }

        public string DistanceForward()
        {
            return Distance()[1];
        }

        public string DistanceRight()
        {
            return Distance()[2];
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
