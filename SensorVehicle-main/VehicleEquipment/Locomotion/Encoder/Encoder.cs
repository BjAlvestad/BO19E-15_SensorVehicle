using System;
using Helpers;

namespace VehicleEquipment.Locomotion.Encoder
{
    public class Encoder : ThreadSafeNotifyPropertyChanged, IEncoder
    {
        private readonly IVehicleCommunication _vehicleCommunication ;

        public DateTime LastRequestTimeStamp { get; set; }
        public int TimeAccumulatedForLastRequest { get; set; }
        public double DistanceAtLastRequest { get; set; }
        public double TotalDistanceTravelled { get; set; }

        public double AvgVel => (DistanceAtLastRequest) / (TimeAccumulatedForLastRequest / 1000f); //Cm per sekund

        public string Message { get; set; }

        public Encoder(IVehicleCommunication comWithEncoder)
        {
            _vehicleCommunication = comWithEncoder;
        }

        public void ResetTotalDistanceTraveled()
        {
            TotalDistanceTravelled = 0;
        }

        public double CollectAndResetDistanceFromEncoder()
        {
            try
            {
                VehicleDataPacket data = _vehicleCommunication.Read();

                Message = $"Message from micro controller: {data.Code}";

                DistanceAtLastRequest = data.Integers[0];
                TimeAccumulatedForLastRequest = data.Integers[1];
                TotalDistanceTravelled += DistanceAtLastRequest;
            }
            catch (Exception p)
            {
                DistanceAtLastRequest = double.NaN;
                TimeAccumulatedForLastRequest = 0;
                Message = $"ERROR: An exception occured when collecting encoder data: {p.Message}";
            }

            LastRequestTimeStamp = DateTime.Now;
            return DistanceAtLastRequest;
        }
    }
}
