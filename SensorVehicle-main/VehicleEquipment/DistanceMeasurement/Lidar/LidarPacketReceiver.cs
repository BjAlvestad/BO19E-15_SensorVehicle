using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    public class LidarPacketReceiver : ILidarPacketReceiver
    {
        public const string DefaultLidarIp = "192.168.1.201";
        public const int DefaultDataPort = 2368;

        internal const int NumberOfDatapacksPerCycle = 300; // Note: This is only true at 300 RPM (it is 150 for 600RPM).
        internal const int DataPayloadSize = 1206;

        private readonly UdpClient Udp = new UdpClient(DefaultDataPort);
        private IPEndPoint _lidarIpEp = new IPEndPoint(IPAddress.Parse(DefaultLidarIp), DefaultDataPort);

        public LidarPacketReceiver()
        {
            // We set buffer size to zero (default is 8192 bytes) to prevent reading old packets from the buffer (we only want the latest distance readings).
            Udp.Client.ReceiveBufferSize = 0;  
        }

        /// <summary>
        /// <para>Collects 300 datapacks from LIDAR per cycle (assuming Lidar is set up to run at 300RPM).</para>
        /// <para>This is a blocking function, and will block the caller until all data packs has been received.</para>
        /// </summary>
        /// <param name="numberOfCycles">
        /// <para>Each cycle contains 300 datapacks, with approximately 1.19 degrees between each data pack.</para>
        /// <para>The readings from each cycle are shifted by approximately 0.80 degrees. Thus you can get greater accuracy by collecting several cycles.</para>
        /// <para>The default of 3 cycles will give a resolution of approximately ??0.4 degrees??.</para>
        /// </param>
        /// <returns>Queue of datapackets collected from LIDAR</returns>
        public Queue<byte[]> GetQueueOfDataPackets(byte numberOfCycles = 3)
        {     
            Queue<byte[]> receivedPackets = new Queue<byte[]>();

            for (int i = 0; i < numberOfCycles * NumberOfDatapacksPerCycle; i++)
            {
                byte[] dataPacket = GetDataPacket();
                if (dataPacket != null)
                {
                    receivedPackets.Enqueue(dataPacket);
                }
            }

            return receivedPackets;
        }

        public byte[] GetDataPacket()
        {
            byte[] dataPacket = Udp.Receive(ref _lidarIpEp);  //TODO: Change to async

            return (dataPacket.Length == DataPayloadSize) ? dataPacket : null;
        }
    }
}
