using System.Collections.Generic;
using VehicleEquipment.DistanceMeasurement.Lidar;

namespace VehicleEquipment.UnitTests.DistanceMeasurement.Lidar
{
    public class FakeLidarData
    {
         public static readonly Dictionary<VerticalAngle, float> ExpectedValues = new Dictionary<VerticalAngle, float>()
        {
            {VerticalAngle.Down15, 8.512f},
            {VerticalAngle.Up1, 8.514f},
            {VerticalAngle.Down13, 8.516f},
            {VerticalAngle.Up3, 8.518f},
            {VerticalAngle.Down11, 8.520f},
            {VerticalAngle.Up5, 8.522f},
            {VerticalAngle.Down9, 8.524f},
            {VerticalAngle.Up7, 8.526f},
            {VerticalAngle.Down7, 8.528f},
            {VerticalAngle.Up9, 8.530f},
            {VerticalAngle.Down5, 8.532f},
            {VerticalAngle.Up11, 8.534f},
            {VerticalAngle.Down3, 8.536f},
            {VerticalAngle.Up13, 8.538f},
            {VerticalAngle.Down1, 8.540f},
            {VerticalAngle.Up15, 8.542f},
        };

        public static readonly byte[] FakeDataBlock1 = new byte[100]
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

        public byte[] GenerateFakeDataPacket()
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
