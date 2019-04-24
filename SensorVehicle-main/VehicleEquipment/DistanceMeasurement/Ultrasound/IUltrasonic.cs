using System;
using System.ComponentModel;
using Helpers;

namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    public interface IUltrasonic : INotifyPropertyChanged
    {
        bool DeisolateI2cCommunciation { get; set; }

        float Left { get; }

        float Fwd { get; }
        float FwdLeft { get; }
        float FwdRight { get; }

        float Right { get; }

        DateTime TimeStamp { get; }

        int PermissableDistanceAge { get; set; }

        bool RaiseNotificationForSelective { get; set; }

        Error Error { get; }

        bool RefreshUltrasonicContinously { get; set; }
    }
}
