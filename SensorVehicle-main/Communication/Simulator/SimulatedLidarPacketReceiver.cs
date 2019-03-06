using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VehicleEquipment.DistanceMeasurement.Lidar;

namespace Communication.Simulator
{
    public class SimulatedLidarPacketReceiver : ILidarPacketReceiver
    {
        public SimulatedLidarPacketReceiver(SimulatorAppServiceClient simulatorCommunication)
        {
            
        }

        public Task<Queue<byte[]>> GetQueueOfDataPacketsAsync(byte numberOfCycles)
        {
            throw new NotImplementedException();
        }
    }
}
