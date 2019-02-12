namespace VehicleEquipment.Locomotion.Encoder
{
    public interface IEncoder
    {
        int TimeAccumulatedSinceLastRequest { get; }
        double DistanceSinceLastRequest { get; }
        double AvgVel { get; }

        //TODO: Change fields in this interface after the Encoder class has been fixed (fields in interface are just copied from Encoder now)
        // Suggested changes to the three fields above
        //TimeSpan TimeSinceLastReading { get; }
        //double DistanceTravelledInCm { get; }
        //double AvgSpeed { get; }
    }
}
