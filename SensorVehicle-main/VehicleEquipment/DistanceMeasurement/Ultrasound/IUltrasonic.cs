using System;

namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    public interface IUltrasonic
    {
        float Left { get; }

        float Fwd { get; }

        float Right { get; }

        DateTime TimeStamp { get; }

        TimeSpan PermissableDistanceAge { get; set; }
    }
}
