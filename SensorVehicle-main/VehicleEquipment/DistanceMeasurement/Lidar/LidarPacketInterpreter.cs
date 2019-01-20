using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    internal static class LidarPacketInterpreter
    {
        private const float MinimumRange = 0.3f;

        public static ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>> InterpretData(Queue<byte[]> lidarDataPackets, List<VerticalAngle> verticalAnglesToCalculate)
        {
            IDictionary<VerticalAngle, List<HorizontalPoint>> interpretedData = InitializeCollection(verticalAnglesToCalculate);

            while (lidarDataPackets.Count > 0)
            {
                byte[] dataPack = lidarDataPackets.Dequeue();

                // Note that with this code we only use the first dataBlock of each dataPack, and ignore the other 11 datablocks (which contain 0.02 degree increments).
                float azimuthAngle = GetAzimuthAngle(dataPack);
                foreach (VerticalAngle verticalAngle in verticalAnglesToCalculate)
                {
                    float distance = GetDistance(dataPack, verticalAngle);
                    if (distance > MinimumRange)
                    {
                        interpretedData[verticalAngle].Add(new HorizontalPoint(azimuthAngle, distance));
                    }
                }
            }
            
            //SortLidarDistances(interpretedData);  //TODO: Check if list should be sortet before sending out, or iterated for each check in LidarDistance class

            return new ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>>(interpretedData);
        }

        private static Dictionary<VerticalAngle, List<HorizontalPoint>> InitializeCollection(List<VerticalAngle> verticalAnglesToInclude)
        {
            Dictionary<VerticalAngle, List<HorizontalPoint>> collection = new Dictionary<VerticalAngle, List<HorizontalPoint>>();
            foreach (VerticalAngle verticalAngle in verticalAnglesToInclude)
            {
                collection.Add(verticalAngle, new List<HorizontalPoint>());
            }

            return collection;
        }

        internal static float GetAzimuthAngle(byte[] dataBlock)
        {
            byte azimuthByte1 = dataBlock[2];
            byte azimuthByte2 = dataBlock[3];

            return ReverseAndCombineBytes(azimuthByte1, azimuthByte2) / 100.0f;
        }

        internal static float GetDistance(byte[] dataBlock, VerticalAngle vertical)
        {
            int channelNumber = (int) vertical;

            byte distanceByte1 = dataBlock[4 + 3 * channelNumber];
            byte distanceByte2 = dataBlock[5 + 3 * channelNumber];

            float distanceInMillimeters = ReverseAndCombineBytes(distanceByte1, distanceByte2) * 2.0f;
            return distanceInMillimeters / 1000;
        }

        private static ushort ReverseAndCombineBytes(byte byte1, byte byte2)
        {
            return (ushort)((byte2<<8) + byte1);
        }

        private static void SortLidarDistances(IDictionary<VerticalAngle, List<HorizontalPoint>> collectionToSort)
        {
            foreach (VerticalAngle verticalAngle in collectionToSort.Keys)
            {
                collectionToSort[verticalAngle].Sort();
            }
        }
    }
}
