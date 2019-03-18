using System.ComponentModel;

namespace VehicleEquipment.Locomotion.Encoder
{
    public interface IEncoders : INotifyPropertyChanged
    {
        bool RaiseNotificationForSelective { get; set; }

        Encoder Left { get; set; }
        Encoder Right { get; set; }

        void ResetTotalDistanceTraveled();
        void CollectAndResetDistanceFromEncoders();
    }
}
