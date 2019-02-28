using System.ComponentModel;

namespace VehicleEquipment.Locomotion.Encoder
{
    public interface IEncoder : INotifyPropertyChanged
    {
        bool RaiseNotificationForSelective { get; set; }

        int TimeAccumulatedForLastRequest { get; }
        double DistanceAtLastRequest { get; }
        double AvgVel { get; }
        double TotalDistanceTravelled { get; }

        void ResetTotalDistanceTraveled();
        double CollectAndResetDistanceFromEncoder();

        //TODO: Change fields in this interface after the Encoder class has been fixed (fields in interface are just copied from Encoder now)
        // Suggested changes to the three fields above
        //TimeSpan TimeSinceLastReading { get; }
        //double DistanceTravelledInCm { get; }
        //double AvgSpeed { get; }
    }
}
