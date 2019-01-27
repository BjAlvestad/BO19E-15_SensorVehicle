namespace VehicleEquipment.DistanceMeasurement
{
    public interface IDistance
    {
        float MinRange { get; }        
        float MaxRange { get; }        
        float Resolution { get; }        

        float GetFwd();
        float GetLeft();
        float GetRight();
    }
}
