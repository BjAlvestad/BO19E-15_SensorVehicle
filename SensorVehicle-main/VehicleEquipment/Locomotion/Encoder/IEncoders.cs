using System;
using System.ComponentModel;

namespace VehicleEquipment.Locomotion.Encoder
{
    public interface IEncoders : INotifyPropertyChanged
    {
        bool RaiseNotificationForSelective { get; set; }

        Encoder Left { get; set; }
        Encoder Right { get; set; }

        TimeSpan CollectionInterval { get; set; }
        bool CollectContinously { get; set; }

        bool HasUnacknowledgedError { get; }
        string Message { get; }
        void ClearMessage();

        void ResetTotalDistanceTraveled();
        void CollectAndResetDistanceFromEncoders();
    }
}
