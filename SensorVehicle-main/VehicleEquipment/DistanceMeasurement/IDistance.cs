namespace VehicleEquipment.DistanceMeasurement
{
    public interface IDistance
    {
        float MinRange { get; }        
        float MaxRange { get; }        
        float Resolution { get; }        

        float Fwd { get; }
        float Left { get; }
        float Right { get; }
    }
}
