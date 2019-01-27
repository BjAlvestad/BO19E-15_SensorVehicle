using System;
using System.Threading;
using VehicleEquipment.TestAppConsole.DistanceMeasurement.Lidar;

namespace VehicleEquipment.TestAppConsole
{
    class TestProgramForVehicleEquipment
    {
        static void Main(string[] args)
        {
            //LidarDistanceCollectorTest.TestOneCycleOfLidarDataCollector();

            LidarDistanceTest.lidarDistance.StartCollector();
            LidarDistanceTest.PrintDirectionsContinously();

            //Console.WriteLine("Sleeping for 1 second");
            //Thread.Sleep(1000);
            //Console.WriteLine("Printing Distances");
            //LidarDistanceTest.PrintDistances();

            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();
        }
    }
}
