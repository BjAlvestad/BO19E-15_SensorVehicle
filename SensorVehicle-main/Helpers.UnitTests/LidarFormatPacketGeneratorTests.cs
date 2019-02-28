using System.Collections.Generic;
using Xunit;

namespace Helpers.UnitTests
{
    public class LidarFormatPacketGeneratorTests
    {
        [Fact]
        public void GenerateDataPacket_ValidAzimutDelta_CorrectDifference()
        {
            double packetAzimuthAngleDelta = 2;
            double blockDelta = packetAzimuthAngleDelta / 12;

            LidarFormatPacketGenerator packetGenerator = new LidarFormatPacketGenerator(0, 0, packetAzimuthAngleDelta);

            byte[] packet1 = packetGenerator.GenerateDataPacket(azimuth: 10, packetStartDistance: 0);
            byte[] packet2 = packetGenerator.GenerateDataPacket(azimuth: 10+packetAzimuthAngleDelta, packetStartDistance: 0);

            List<double> horizontalAngles = GetHorizontalAnglesFromPackets(packet1);
            horizontalAngles.AddRange(GetHorizontalAnglesFromPackets(packet2));

            for (int i = 1; i < horizontalAngles.Count; i++)
            {
                Assert.Equal(blockDelta, horizontalAngles[i] - horizontalAngles[i-1], 1);
            }
        }

        private List<double> GetHorizontalAnglesFromPackets(byte[] packet)
        {
            List<double> horizontalAngles = new List<double>();

            for (int i = 0; i < 12; i++)
            {
                byte azimuthByte1 = packet[2 + 100*i];
                byte azimuthByte2 = packet[3 + 100*i];

                double angleInDegrees = ReverseAndCombineBytes(azimuthByte1, azimuthByte2) / 100.0;
                horizontalAngles.Add(angleInDegrees);
            }

            return horizontalAngles;
        }

        private static ushort ReverseAndCombineBytes(byte byte1, byte byte2)
        {
            return (ushort)((byte2<<8) + byte1);
        }
    }
}
