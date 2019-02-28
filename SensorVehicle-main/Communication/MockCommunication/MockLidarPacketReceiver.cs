using System.Collections.Generic;
using System.Threading.Tasks;
using Helpers;
using VehicleEquipment.DistanceMeasurement.Lidar;

namespace Communication.MockCommunication
{
    public class MockLidarPacketReceiver : ILidarPacketReceiver
    {
        private readonly LidarFormatPacketGenerator _packetGenerator;
        private readonly double _packetAzimuthAngleDelta = 5.9;
        private readonly double _packetDeltaDistance = 1;

        public MockLidarPacketReceiver()
        {
            const double channelDeltaDistance = 2;
            _packetGenerator = new LidarFormatPacketGenerator(_packetDeltaDistance, channelDeltaDistance, _packetAzimuthAngleDelta);
        }

        public async Task<Queue<byte[]>> GetQueueOfDataPacketsAsync(byte numberOfCycles)
        {
            int packetsToGenerate = 60;

            Queue<byte[]> dataPackets = new Queue<byte[]>();

            for (int i = 0; i < packetsToGenerate; i++)
            {
                dataPackets.Enqueue(_packetGenerator.GenerateDataPacket(_packetAzimuthAngleDelta * i, _packetDeltaDistance*i));
            }

            await Task.Delay(600);

            return dataPackets;
        }
    }
}
