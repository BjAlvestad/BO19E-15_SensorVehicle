using System;
using System.ComponentModel;

namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    public interface IUltrasonic : INotifyPropertyChanged
    {
        float Left { get; }

        float Fwd { get; }
        float FwdLeft { get; }
        float FwdRight { get; }

        float Right { get; }

        DateTime TimeStamp { get; }

        int PermissableDistanceAge { get; set; }

        bool RaiseNotificationForSelective { get; set; }

        bool HasUnacknowledgedError { get; }

        string Message { get; }

        void ClearMessage();

        bool RefreshUltrasonicContinously { get; set; }
    }
}
