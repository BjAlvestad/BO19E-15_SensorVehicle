using System;
using System.Collections.Generic;

namespace Helpers
{
    public class LidarFormatPacketGenerator
    {
        private readonly List<byte> _dataPacket;

        /// <summary>
        /// LidarFormatPacketGenerator is used to generate packets 
        /// </summary>
        /// <param name="packetDeltaDistance">Distance increase between each packet (i.e. between two horizontal angles on the same channel)</param>
        /// <param name="channelDeltaDistance">Distance difference between each channel (i.e. between two vertical angles, for the same horizontal angle.)</param>
        /// <param name="packetAzimuthAngleDelta">Increase in horizontal angle between each block generated</param>
        public LidarFormatPacketGenerator(double packetDeltaDistance, double channelDeltaDistance, double packetAzimuthAngleDelta)
        {
            _dataPacket = new List<byte>();
            PacketDeltaDistance = packetDeltaDistance;
            ChannelDeltaDistance = channelDeltaDistance;
            BlockAzimuthAngleDelta = packetAzimuthAngleDelta / 12;
        }

        public double PacketDeltaDistance { get; set; }
        public double ChannelDeltaDistance { get; set; }
        public double BlockAzimuthAngleDelta { get; set; }

        /// <summary>
        /// A DataPacket is the packets the Lidar sends out. It consists of 12 DataBlocks.
        /// </summary>
        /// <param name="azimuth"></param>
        /// <param name="packetStartDistance"></param>
        /// <returns></returns>
        public byte[] GenerateDataPacket(double azimuth, double packetStartDistance)
        {
            _dataPacket.Clear();

            for (int i = 0; i < 12; i++)
            {
                double azimuthForBlock = azimuth + BlockAzimuthAngleDelta * i;
                double blockStartDistance = packetStartDistance + PacketDeltaDistance * i;

                AddDataBlockToPacket(azimuthForBlock, blockStartDistance);
            }

            if (_dataPacket.Count != 100 * 12) throw new Exception($"Generated invalid packet size, expected 12*100 bytes, generated {_dataPacket.Count} bytes");

            return _dataPacket.ToArray();
        }

        /// <summary>
        /// A datablock consists of 100 bytes (2 flags, 3 for azimuth, and 3x32 for distance)
        /// </summary>
        /// <param name="azimuth"></param>
        /// <param name="blockStartDistance"></param>
        private void AddDataBlockToPacket(double azimuth, double blockStartDistance)
        {
            AddFlaggs();
            AddAzimuth(azimuth);

            AddDistances(blockStartDistance);
        }

        private void AddFlaggs()
        {
            _dataPacket.Add(0xff);
            _dataPacket.Add(0xee);
        }

        private void AddAzimuth(double azimuth)
        {
            ushort value =  Convert.ToUInt16(Math.Min(azimuth * 100, 35999));

            _dataPacket.Add((byte)(value & 0b0000_0000_1111_1111));
            _dataPacket.Add((byte)(value >> 8));
        }

        private void AddDistances(double blockStartDistance)
        {
            for (int i = 0; i < 2; i++)
            {
                double firstChannelInBlockDistance = blockStartDistance + i * PacketDeltaDistance / 2;

                for (int channel = 0; channel < 16; channel++)
                {
                    double currentChannelDistance = firstChannelInBlockDistance + ChannelDeltaDistance * channel;

                    ushort value = Convert.ToUInt16(Math.Min((currentChannelDistance / 2)*1000, 50000));

                    _dataPacket.Add((byte) (value & 0b0000_0000_1111_1111));
                    _dataPacket.Add((byte)(value >> 8));
                    _dataPacket.Add(0);
                }
            }
        }
    }
}
