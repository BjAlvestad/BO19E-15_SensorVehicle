using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using VehicleEquipment.DistanceMeasurement.Lidar;

namespace VehicleEquipment.TestAppConsole.DistanceMeasurement.Lidar
{
    static class LidarDistanceTest
    {
        public static LidarDistance lidarDistance = new LidarDistance(new LidarPacketReceiver(), GetListOfAllVerticalAngles());

        public static void PrintDistanceFromIndex()
        {
            Console.WriteLine($"index 0: {lidarDistance.Distances[VerticalAngle.Up3][0].Angle : 000.00} degrees,  {lidarDistance.Distances[VerticalAngle.Up3][0].Distance}m");
            Console.WriteLine($"index 15: {lidarDistance.Distances[VerticalAngle.Up3][15].Angle : 000.00} degrees,  {lidarDistance.Distances[VerticalAngle.Up3][15].Distance}m");
            Console.WriteLine($"index 200: {lidarDistance.Distances[VerticalAngle.Up3][200].Angle : 000.00} degrees,  {lidarDistance.Distances[VerticalAngle.Up3][200].Distance}m");
        }       
        
        public static void PrintAverageDirections()
        {
            Console.WriteLine($"\t Fwd: {lidarDistance.Fwd : 000.00}\t\t");
            Console.WriteLine($"Left: {lidarDistance.Left : 000.00} \t Right: {lidarDistance.Right : 000.00}\t\t");
            Console.WriteLine($"\t Aft: {lidarDistance.Aft : 000.00}\t\t");
        }

        public static void PrintDirectionsContinously()
        {
            int cursorRow = Console.CursorTop;
            while (true)
            {
                PrintAverageDirections();
                //PrintDistanceFromIndex();
                Console.SetCursorPosition(0, cursorRow);
            }          
        }

        public static void PrintDistances()
        {
            List<HorizontalPoint> distances = lidarDistance.Distances[VerticalAngle.Up3];


            int numberOfAnglesToPrint = distances.Count;

            // Display horizontal angles
            Console.WriteLine("\nPRINTING 360 horizontal angles:");
            int lastNewline = 0;
            for (int i = 0; i < numberOfAnglesToPrint; i++)
            {
                float horizontalAngle = distances[i].Angle;

                if (i > lastNewline + 2 && Math.Floor(horizontalAngle) % 10 <= 3)
                {
                    Console.WriteLine();
                    lastNewline = i;
                }

                Console.Write($"{horizontalAngle: 000.00} ({distances[i].Distance}m), ");
            }

            Console.WriteLine($"\nFinnished prining {numberOfAnglesToPrint}");
        }

        private static VerticalAngle[] GetListOfAllVerticalAngles()
        {
            List<VerticalAngle> activeAngles = new List<VerticalAngle>();

            foreach (VerticalAngle verticalAngle in Enum.GetValues(typeof(VerticalAngle)))
            {
                activeAngles.Add(verticalAngle);
            }

            return activeAngles.ToArray();
        }
    }
}
