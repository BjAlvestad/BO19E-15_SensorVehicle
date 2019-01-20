using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using VehicleEquipment.DistanceMeasurement.Lidar;

namespace VehicleEquipment.TestAppConsole.DistanceMeasurement.Lidar
{
    static class LidarDistanceCollectorTest
    {
        public static void GetDistance()
        {

        }


        public static void TestOneCycleOfLidarDataCollector()
        {
            LidarDistanceCollector.Run = true;

            //LidarDistanceCollector.AddVerticalAngle(VerticalAngle.Up3);
            //LidarDistanceCollector.NumberOfCyclesPerCollection = numberOfPacketsPerCycle;

            Stopwatch stopwatch = Stopwatch.StartNew();
            LidarDistanceCollector.CollectDistances();
            stopwatch.Stop();
            //Console.WriteLine($"Reading packets from {numberOfPacketsPerCycle} cycles took {LidarCommunication.TimeElapsedWhileFillingDataPacketQueue}ms for one cycle");

            PrintHorizontalAnglesToConsole(true);
        }

        private static void PrintHorizontalAnglesToConsole(bool printAllAngles = false)
        {
            int numberOfAnglesToPrint = printAllAngles ? LidarDistanceCollector._distances[VerticalAngle.Up3].Count : 300;

            // Display horizontal angles
            Console.WriteLine("\nPRINTING 360 horizontal angles:");
            int lastNewline = 0;
            for (int i = 0; i < numberOfAnglesToPrint; i++)
            {
                float horizontalAngle = LidarDistanceCollector._distances[VerticalAngle.Up3][i].Angle;

                if (i > lastNewline + 2 && Math.Floor(horizontalAngle) % 10 <= 3)
                {
                    Console.WriteLine();
                    lastNewline = i;
                }

                Console.Write($"{horizontalAngle: 000.00}, ");
            }

            Console.WriteLine($"\nFinnished prining {numberOfAnglesToPrint}");
        }
    }
}
