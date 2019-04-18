using System;
using System.ComponentModel;
using Helpers;

namespace VehicleEquipment.Locomotion.Encoder
{
    public interface IEncoders : INotifyPropertyChanged
    {
        bool RaiseNotificationForSelective { get; set; }

        Error Error { get; }

        Encoder Left { get; }
        Encoder Right { get; }

        int CollectionInterval { get; set; }
        bool CollectContinously { get; set; }

        void ResetTotalDistanceTraveled();
        void CollectAndResetDistanceFromEncoders();

        void ClearAllEncoderErrors();
    }
}
