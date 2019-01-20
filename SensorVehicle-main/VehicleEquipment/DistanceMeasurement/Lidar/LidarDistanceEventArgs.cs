using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    internal class LidarDistanceEventArgs
    {
        public ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>> LidarCycles { get; internal set;  }
        public TimeSpan TimeSpentCollectingPackets { get; internal set; }
        public TimeSpan TimeSpentPerformingCalculations { get; internal set; }
    }
}
