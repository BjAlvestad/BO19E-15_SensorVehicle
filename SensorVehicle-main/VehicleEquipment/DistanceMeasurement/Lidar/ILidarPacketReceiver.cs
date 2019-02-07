using System.Collections.Generic;
using System.Threading.Tasks;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    public interface ILidarPacketReceiver
    {
        Task<Queue<byte[]>> GetQueueOfDataPacketsAsync(byte numberOfCycles);
    }
}
