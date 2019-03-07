using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Helpers;
using VehicleEquipment.DistanceMeasurement.Lidar;

namespace Communication.Simulator
{
    public class SimulatedLidarPacketReceiver : ILidarPacketReceiver
    {
        private SimulatorAppServiceClient _simulatorCommunication;
        private LidarFormatPacketGenerator _packetGenerator;

        public SimulatedLidarPacketReceiver(SimulatorAppServiceClient simulatorCommunication)
        {
            _simulatorCommunication = simulatorCommunication;
            _packetGenerator = new LidarFormatPacketGenerator(packetDeltaDistance:0, channelDeltaDistance:0, packetAzimuthAngleDelta:0);
        }

        public async Task<Queue<byte[]>> GetQueueOfDataPacketsAsync(byte numberOfCycles)
        {
            Queue<byte[]> dataPackets = ConvertDistancesToByteArray(GetDistancesFromSimulator());     

            await Task.Delay(600);

            return dataPackets;
        }

        private float[] GetDistancesFromSimulator()
        {
            ValueSet valuesToSend = new ValueSet {{"LIDAR", ""}};

            ValueSet dataReceived = Task.Run(() => _simulatorCommunication.RequestDataAsync(valuesToSend)).GetAwaiter().GetResult(); //TODO: This is a temporary hack that blocks the async method. WARNING MAY CAUSE ISSUES!. See if Read() can be rewritten to be async.  See Figure 7 "The Thread Pool Hack" on https://msdn.microsoft.com/en-us/magazine/mt238404.aspx

            return dataReceived["DATA"] as float[];
        }

        private Queue<byte[]> ConvertDistancesToByteArray(float[] distancesAsFloats)
        {
            Queue<byte[]> dataPackets = new Queue<byte[]>();

            for (int i = 0; i < distancesAsFloats.Length ; i++)
            {
                dataPackets.Enqueue(_packetGenerator.GenerateDataPacket(azimuth: i, packetStartDistance: distancesAsFloats[i]));
            }

            return dataPackets;
        }
    }
}
