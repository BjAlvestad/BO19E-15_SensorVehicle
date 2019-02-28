using System.ComponentModel;

namespace VehicleEquipment.Locomotion.Wheels
{
    public interface IWheel : INotifyPropertyChanged
    {
        bool RaiseNotificationForSelective { get; set; }

        int CurrentSpeedLeft { get; }
        int CurrentSpeedRight { get; }

        void SetSpeed(int leftValue, int rightValue, bool onlySendIfValuesChanged = true);

        void Fwd(int speed);
        void TurnLeft(int speed);
        void TurnRight(int speed);
        void Reverse(int speed);
        void Stop();
    }
}
