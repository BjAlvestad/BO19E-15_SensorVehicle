using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace VehicleEquipment.Locomotion.Encoder
{
    public class Encoder : IEncoder
    {
        private readonly IVehicleCommunication _vehicleCommunication ;

        public DateTime LastRequestTimeStamp { get; set; }
        public int TimeAccumulatedForLastRequest { get; set; }
        public double DistanceAtLastRequest { get; set; }
        public double TotalDistanceTravelled { get; set; }

        public double AvgVel => (DistanceAtLastRequest) / (TimeAccumulatedForLastRequest / 1000f); //Cm per sekund

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
            GetEncoderData();
            LastRequestTimeStamp = DateTime.Now;

            return DistanceAtLastRequest;
        }

        private void GetEncoderData()
        {
            try
            {
                byte[] response = _vehicleCommunication.Read();

                int fromAddress = response[0];

                int[] reassembledValues = AssembleIntsFromByteArray(response, numberOfIntsToAssemble: 2);

                DistanceAtLastRequest = reassembledValues[0];
                TimeAccumulatedForLastRequest = reassembledValues[1];
                TotalDistanceTravelled += DistanceAtLastRequest;
            }
            catch (Exception p)
            {
                DistanceAtLastRequest = double.NaN;
                TimeAccumulatedForLastRequest = 0;
                Debug.WriteLine($"EXCEPTION IN ENCODER.cs: {p.Message}");
            }

            TotalDistanceTravelled += DistanceAtLastRequest;
        }

        private int[] AssembleIntsFromByteArray(byte[] response, int numberOfIntsToAssemble)
        {
            int[] assembledData = new int[numberOfIntsToAssemble];

            for (int i = 0; i < numberOfIntsToAssemble; i++)
            {
                assembledData[i] = (response[1 + 4 * i] << 24) | (response[2 + 4 * i] << 16) | (response[3 + 4 * i] << 8) | response[4 + 4 * i];
            }
            Debug.WriteLine($"Assemble {String.Join(", ", response)} into {String.Join(", ", assembledData)}");
            Debug.WriteLine($"Properties are now: DistanceAtLastRequest {DistanceAtLastRequest},  TimeAccumulatedForLastRequest {TimeAccumulatedForLastRequest},  TotalDistanceTraveled {TotalDistanceTravelled}");

            return assembledData;
        }
    }
}
