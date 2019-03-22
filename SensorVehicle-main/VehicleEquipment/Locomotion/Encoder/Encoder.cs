﻿using System;
using Helpers;

namespace VehicleEquipment.Locomotion.Encoder
{
    public class Encoder
    {
        private readonly IVehicleCommunication _vehicleCommunication;
        private readonly object _velocityCalcLock = new object();

        public DateTime LastRequestTimeStamp { get; private set; }
        public int TimeAccumulatedForLastRequest { get; private set; }
        public TimeSpan TotalTime { get; private set; }
        public double DistanceAtLastRequest { get; private set; }
        public double TotalDistanceTravelled { get; private set; }

        public double AvgVel { get; private set; } //Cm per sekund

        public string Message { get; private set; }

        public Encoder(IVehicleCommunication comWithEncoder)
        {
            _vehicleCommunication = comWithEncoder;
        }

        public double CollectAndResetDistanceFromEncoder()
        {
            try
            {
                VehicleDataPacket data = _vehicleCommunication.Read();

                Message = $"Message from micro controller: {data.Code}";

                lock (_velocityCalcLock)
                {
                    DistanceAtLastRequest = data.Integers[0];
                    TimeAccumulatedForLastRequest = data.Integers[1];
                    AvgVel = DistanceAtLastRequest / (TimeAccumulatedForLastRequest / 1000f);
                }

                TotalTime += TimeSpan.FromMilliseconds(TimeAccumulatedForLastRequest);
                TotalDistanceTravelled += DistanceAtLastRequest;
            }
            catch (Exception p)
            {
                DistanceAtLastRequest = double.NaN;
                TimeAccumulatedForLastRequest = 0;
                Message = $"ERROR: An exception occured when collecting encoder data: {p.Message}";
                throw;
            }

            LastRequestTimeStamp = DateTime.Now;
            return DistanceAtLastRequest;
        }

        public void ResetTotalDistanceTraveled()
        {
            TotalDistanceTravelled = 0;
            TotalTime = TimeSpan.Zero;
        }

        //TODO: Consider changeing names of properties
        // Suggested changes to the three fields above
        //TimeSpan TimeSinceLastReading { get; }
        //double DistanceTravelledInCm { get; }
        //double AvgSpeed { get; }
    }
}
