using System;
using Helpers;

namespace VehicleEquipment.Locomotion.Encoder
{
    public class Encoder
    {
        private readonly IVehicleCommunication _vehicleCommunication;
        private readonly object _velocityCalcLock = new object();

        /// <summary>
        /// The point in time at which data was last collected from encoders. <para />
        /// I.e. last successfull call to <see cref="CollectAndResetDistanceFromEncoder"/>
        /// </summary>
        public DateTime LastRequestTimeStamp { get; private set; }

        /// <summary>
        /// The time span the last collected data was collected over (in milliseconds). <para />
        /// I.e. time accumulated by the micro-controller since last call to <see cref="CollectAndResetDistanceFromEncoder"/>
        /// </summary>
        public int TimeAccumulatedForLastRequest { get; private set; }

        /// <summary>
        /// Total time accumulated since last call to <see cref="ResetTotalDistanceTraveled"/>
        /// </summary>
        public TimeSpan TotalTime { get; private set; }

        /// <summary>
        /// The distance traveled received from the micro-controller at last call to <see cref="CollectAndResetDistanceFromEncoder"/>
        /// </summary>
        public double DistanceAtLastRequest { get; private set; }

        /// <summary>
        /// Total distance accumulated since last call to <see cref="ResetTotalDistanceTraveled"/>
        /// </summary>
        public double TotalDistanceTravelled { get; private set; }

        /// <summary>
        /// Average velocity based on last data collected from micro-controller. <para />
        /// Note that this is an average based on the total time accumulated on the micro-controller. 
        /// This value will not be a good representation of the vehicles speed unless two consecutive calls to <see cref="CollectAndResetDistanceFromEncoder"/> are performed, while the vehicle is not accelerating.
        /// </summary>
        /// <remarks>
        /// Average velocity is calculated from <see cref="TimeAccumulatedForLastRequest"/> and <see cref="DistanceAtLastRequest"/>
        /// </remarks>
        public double AvgVel { get; private set; } //Cm per sekund

        /// <summary>
        /// Message code received from encoder.
        /// </summary>
        public string Message { get; private set; }

        internal Error Error { get; }

        public Encoder(IVehicleCommunication comWithEncoder)
        {
            _vehicleCommunication = comWithEncoder;
            Error = new Error();
        }

        internal double CollectAndResetDistanceFromEncoder()
        {
            if (Error.Unacknowledged)
            {
                DistanceAtLastRequest = double.NaN;
                return DistanceAtLastRequest;
            }

            try
            {
                VehicleDataPacket data = _vehicleCommunication.Read();

                Message = $"Message from micro controller: {data.Code}";

                lock (_velocityCalcLock)
                {
                    DistanceAtLastRequest = data.Integers[0];
                    TimeAccumulatedForLastRequest = data.Integers[1];
                    AvgVel = DistanceAtLastRequest / (TimeAccumulatedForLastRequest / 1000f);
                }

                TotalTime += TimeSpan.FromMilliseconds(TimeAccumulatedForLastRequest);
                TotalDistanceTravelled += DistanceAtLastRequest;
            }
            catch (Exception e)
            {
                DistanceAtLastRequest = double.NaN;
                TimeAccumulatedForLastRequest = 0;
                Error.Message = e.Message;
                Error.DetailedMessage = e.ToString();
                Error.Unacknowledged = true;
            }

            LastRequestTimeStamp = DateTime.Now;
            return DistanceAtLastRequest;
        }

        internal void ResetTotalDistanceTraveled()
        {
            TotalDistanceTravelled = 0;
            TotalTime = TimeSpan.Zero;
        }

        //TODO: Consider changeing names of properties
        // Suggested changes to the three fields above
        //TimeSpan TimeSinceLastReading { get; }
        //double DistanceTravelledInCm { get; }
        //double AvgSpeed { get; }
    }
}
