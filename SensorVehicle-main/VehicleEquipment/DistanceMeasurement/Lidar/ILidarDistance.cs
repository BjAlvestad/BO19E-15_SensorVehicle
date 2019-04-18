using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Helpers;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    public interface ILidarDistance : INotifyPropertyChanged
    {
        bool RaiseNotificationForSelective { get; set; }

        Error Error { get; }

        bool RunCollector { get; set; }

        double MinRange { get; set; }
        double MaxRange { get; set; }

        void StartCollector();
        void StopCollector();

        HorizontalPoint LargestDistance { get; }
        float Fwd { get; }
        float Left { get; }
        float Right { get; }
        float Aft { get; }

        int DefaultHalfBeamOpening { get; set; }
        CalculationType DefaultCalculationType { get; set; }
        VerticalAngle DefaultVerticalAngle { get; set; }
        ExclusiveSynchronizedObservableCollection<VerticalAngle> ActiveVerticalAngles { get; }
        int NumberOfCycles { get; set; }

        ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>> Distances { get; }
        DateTime LastUpdate { get; }
        long LastCollectionDuration { get; }
        long LastDataInterpretationDuration { get; }

        HorizontalPoint LargestDistanceInRange(float fromAngle = 260, float toAngle = 100);

        float GetDistance(float fromAngle, float toAngle, VerticalAngle verticalAngle, CalculationType calculationType);
        List<float> GetDistancesInRange(float fromAngle, float toAngle, VerticalAngle verticalAngle);
        List<HorizontalPoint> GetHorizontalPointsInRange(float fromAngle, float toAngle, VerticalAngle verticalAngle);
    }
}
