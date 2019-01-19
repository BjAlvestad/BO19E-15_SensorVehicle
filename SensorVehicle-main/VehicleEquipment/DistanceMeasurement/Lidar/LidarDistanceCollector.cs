using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    internal static class LidarDistanceCollector
    {
        private static readonly Thread LidarCollectorThread;

        private static readonly Stopwatch PacketCollectionStopwatch = new Stopwatch();
        private static readonly Stopwatch CalculationStopwatch = new Stopwatch();

        private static List<VerticalAngle> _activeAngles = new List<VerticalAngle>();
        internal static ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>> _distances;

        static LidarDistanceCollector()
        {
            LidarCollectorThread = new Thread(CollectDistances);
            LidarCollectorThread.IsBackground = true;  // Run as background thread, so that it doesn't prevent closing the program.
            TemporaryMethodToSetAllVerticalAnglesToActive(); // TEMP: adds all vertical angles to activeAngles list until we can find a good way to check which angles should be read.
        }

        public static event EventHandler<LidarDistanceEventArgs> NewDistances;
        
        private static bool _run;
        public static bool Run
        {
            get => _run;
            set
            {
                if (value == true && _run == false)
                {
                    LidarCollectorThread.Start();
                }
                _run = value;
            }
        }

        public static void CollectDistances()
        {
            while (Run)
            {
                PacketCollectionStopwatch.Start();
                Queue<byte[]> lidarPackets = LidarPacketReceiver.GetQueueOfDataPackets();
                PacketCollectionStopwatch.Stop();

                CalculationStopwatch.Start();
                _distances = LidarPacketInterpreter.InterpretData(lidarPackets, _activeAngles);
                CalculationStopwatch.Stop();

                OnNewDistances();
            }
        }

        private static void OnNewDistances()
        {
            LidarDistanceEventArgs lidarDistanceEventArgs = new LidarDistanceEventArgs()
            {
                TimeSpentCollectingPackets = PacketCollectionStopwatch.Elapsed,
                TimeSpentPerformingCalculations = CalculationStopwatch.Elapsed,
                LidarCycles = _distances
            };

            NewDistances?.Invoke(null, lidarDistanceEventArgs);

            PacketCollectionStopwatch.Reset();
            CalculationStopwatch.Reset();
        }

        private static void TemporaryMethodToSetAllVerticalAnglesToActive()
        {
            foreach (VerticalAngle verticalAngle in Enum.GetValues(typeof(VerticalAngle)))
            {
                _activeAngles.Add(verticalAngle);
            }
        }
    }
}
