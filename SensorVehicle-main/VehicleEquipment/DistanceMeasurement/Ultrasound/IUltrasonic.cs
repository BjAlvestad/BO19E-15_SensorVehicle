namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    public interface IUltrasonic
    {
        float DistanceLeft();

        float DistanceForward();

        float DistanceRight();
    }
}
