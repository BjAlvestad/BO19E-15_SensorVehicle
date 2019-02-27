﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Helpers;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    public interface ILidarDistance : INotifyPropertyChanged
    {
        bool RaiseNotificationForSelective { get; set; }
        bool HasUnacknowledgedError { get; }
        string Message { get; }

        bool RunCollector { get; set; }

        void ClearMessage();

        void StartCollector();
        void StopCollector();

        float Fwd { get; }
        float Left { get; }
        float Right { get; }
        float Aft { get; }

        int DefaultHalfBeamOpening { get; set; }
        CalculationType DefaultCalculationType { get; set; }
        VerticalAngle DefaultVerticalAngle { get; set; }
        ExclusiveSynchronizedObservableCollection<VerticalAngle> ActiveVerticalAngles { get; }

        ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>> Distances { get; }
        DateTime LastUpdate { get; }

        float GetDistance(float fromAngle, float toAngle, VerticalAngle verticalAngle, CalculationType calculationType);
        List<float> GetDistancesInRange(float fromAngle, float toAngle, VerticalAngle verticalAngle);
    }
}
