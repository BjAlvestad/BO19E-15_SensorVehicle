using System.Collections.Generic;
using System.ComponentModel;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    public interface ILidarDistance : INotifyPropertyChanged
    {
        bool RaiseNotificationForSelective { get; set; }
        void StartCollector();
        void StopCollector();

        float GetFwd();
        float GetLeft();
        float GetRight();
        float GetAft();

        float GetDistance(float fromAngle, float toAngle, VerticalAngle verticalAngle, CalculationType calculationType);
        List<float> GetDistancesInRange(float fromAngle, float toAngle, VerticalAngle verticalAngle);
    }
}
