using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleEquipment.DistanceMeasurement.Lidar;

namespace Communication.MockCommunication
{
    public class MockLidarPacketReceiver : ILidarPacketReceiver
    {
        public async Task<Queue<byte[]>> GetQueueOfDataPacketsAsync(byte numberOfCycles)
        {
            Queue<byte[]> dataPackets = new Queue<byte[]>();

            for (int i = 0; i < 50; i++)
            {
                dataPackets.Enqueue(GenerateFakeDataPacket());
            }
            
            return dataPackets;
        }

        private static readonly byte[] FakeDataBlock1 = new byte[100]
        {
            0xff, 0xee, // Flagg
            0x39, 0x30, // Azimuth 123.45 degrees
            0xA0, 0x10, 0x00, // Distance Ch00 8,512 meters
            0xA1, 0x10, 0x00, // Distance Ch01 8,514 meters
            0xA2, 0x10, 0x00, // Distance Ch02 8,516 meters
            0xA3, 0x10, 0x00, // Distance Ch03 8,518 meters
            0xA4, 0x10, 0x00, // Distance Ch04 8,520 meters
            0xA5, 0x10, 0x00, // Distance Ch05 8,522 meters
            0xA6, 0x10, 0x00, // Distance Ch06 8,524 meters
            0xA7, 0x10, 0x00, // Distance Ch07 8,526 meters
            0xA8, 0x10, 0x00, // Distance Ch08 8,528 meters
            0xA9, 0x10, 0x00, // Distance Ch09 8,530 meters
            0xAA, 0x10, 0x00, // Distance Ch10 8,532 meters
            0xAB, 0x10, 0x00, // Distance Ch11 8,534 meters
            0xAC, 0x10, 0x00, // Distance Ch12 8,536 meters
            0xAD, 0x10, 0x00, // Distance Ch13 8,538 meters
            0xAE, 0x10, 0x00, // Distance Ch14 8,540 meters
            0xAF, 0x10, 0x00, // Distance Ch15 8,542 meters            
            0xB0, 0x10, 0x00, // Distance Ch00 8,544 meters
            0xB1, 0x10, 0x00, // Distance Ch01 8,546 meters
            0xB2, 0x10, 0x00, // Distance Ch02 8,548 meters
            0xB3, 0x10, 0x00, // Distance Ch03 8,550 meters
            0xB4, 0x10, 0x00, // Distance Ch04 8,552 meters
            0xB5, 0x10, 0x00, // Distance Ch05 8,554 meters
            0xB6, 0x10, 0x00, // Distance Ch06 8,556 meters
            0xB7, 0x10, 0x00, // Distance Ch07 8,558 meters
            0xB8, 0x10, 0x00, // Distance Ch08 8,560 meters
            0xB9, 0x10, 0x00, // Distance Ch09 8,562 meters
            0xBA, 0x10, 0x00, // Distance Ch10 8,564 meters
            0xBB, 0x10, 0x00, // Distance Ch11 8,566 meters
            0xBC, 0x10, 0x00, // Distance Ch12 8,568 meters
            0xBD, 0x10, 0x00, // Distance Ch13 8,570 meters
            0xBE, 0x10, 0x00, // Distance Ch14 8,572 meters
            0xBF, 0x10, 0x00, // Distance Ch15 8,564 meters
        };

        private byte[] GenerateFakeDataPacket()
        {
            int availableIndex = 0;
            byte[] fakeDataPacket = new byte[1206];

            // Adds the 12 data blocks
            for (int i = 0; i < 12; i++)
            {
                foreach (byte b in FakeDataBlock1)
                {
                    fakeDataPacket[availableIndex] = b;
                    ++availableIndex;
                }
            }

            // Adds the time stamp (1,522.100065 seconds past the hour)
            fakeDataPacket[availableIndex] = 0x61;
            ++availableIndex;
            fakeDataPacket[availableIndex] = 0x67;
            ++availableIndex;
            fakeDataPacket[availableIndex] = 0xb9;
            ++availableIndex;
            fakeDataPacket[availableIndex] = 0x5a;
            ++availableIndex;


            // Adds the Factory bytes (Dual retrun mode, and source is VLP-16)
            fakeDataPacket[availableIndex] = 0x39;
            ++availableIndex;
            fakeDataPacket[availableIndex] = 0x22;

            return fakeDataPacket;
        }
    }
}
