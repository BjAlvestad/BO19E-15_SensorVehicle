using System;
using System.Collections.Generic;
using System.Text;

namespace VehicleEquipment.Locomotion.Encoder
{
    public class Encoder : IEncoder
    {
        private readonly IVehicleCommunication _vehicleCommunication ;

        public DateTime LastRequestTimeStamp { get; set; }
        public int TimeAccumulatedSinceLastRequest { get; set; }
        public double DistanceSinceLastRequest { get; set; }
        public double TotalDistanceTravelled { get; set; }

        public double AvgVel => DistanceSinceLastRequest / TimeAccumulatedSinceLastRequest;

        public Encoder(IVehicleCommunication comWithEncoder)
        {
            _vehicleCommunication = comWithEncoder;
        }

        public void GetEncoderData()
        {
            try
            {
                byte[] response = _vehicleCommunication.Read();
                
                // BUG: first element is always set to 0, and are for that reason not used here (bug in microcontroller code?)
                bool positiveNum = response[4] == 0; //TODO: Transfere signed integer instead of separate sign

                byte firstDistanceByte = response[1];
                byte secondDistanceByte = response[2];
                TimeAccumulatedSinceLastRequest = response[3];
                if (positiveNum)
                {
                    DistanceSinceLastRequest = Convert.ToInt16((firstDistanceByte << 8) | secondDistanceByte);
                }
                else
                {
                    DistanceSinceLastRequest = -Convert.ToInt16((firstDistanceByte << 8) | secondDistanceByte);
                }
            }
            catch (Exception p)
            {
                DistanceSinceLastRequest = double.NaN;
            }

            TotalDistanceTravelled += DistanceSinceLastRequest;
        }
    }
}
