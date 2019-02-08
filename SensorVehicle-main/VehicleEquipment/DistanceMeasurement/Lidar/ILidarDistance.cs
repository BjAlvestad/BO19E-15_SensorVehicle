using System.Collections.Generic;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    public interface ILidarDistance
    {
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
