namespace VehicleEquipment.DistanceMeasurement
{
    public interface IDistance
    {
        float Fwd { get; }
        float Left { get; }
        float Right { get; }        
        float MinRange { get; }        
        float MaxRange { get; }        
        float Resolution { get; }        
    }
}
