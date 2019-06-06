using Helpers;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    public class LidarDistanceConfiguration : ThreadSafeNotifyPropertyChanged
    {
        /// <summary>
        /// The vertical angles that is in use from the lidar. (Only angles added to this list will be calculated).
        /// </summary>
        public ExclusiveSynchronizedObservableCollection<VerticalAngle> ActiveVerticalAngles { get; }

        public LidarDistanceConfiguration(VerticalAngle[] verticalAngles)
        {
            DefaultHalfBeamOpening = 15;
            DefaultCalculationType = CalculationType.Max;

            ActiveVerticalAngles = new ExclusiveSynchronizedObservableCollection<VerticalAngle>();
            DefaultVerticalAngle = verticalAngles[0];
            ActiveVerticalAngles.AddFromArray(verticalAngles);

            NumberOfCycles = 3;

            MinRange = 1.0;  // According to page 10 of VLP-16 user manual: 'points with distances less than one meter should be ignored'. But other sources claim smaller distances can be used.
            MaxRange = 100.0;  // According to page 3 of VLP-16 user manual: 'range from 1m to 100m'.
        }

        private double _minRange;
        /// <summary>
        /// Minimum range for lidar. Distances below this range will be discarded. <para />
        /// The manual states 1 meter as minimum range. When testing the Lidar measures accurately down to 0.5 meters. Distances below 0.5 seems to be discarded by the lidar itself.
        /// </summary>
        public double MinRange
        {
            get { return _minRange; }
            set { SetProperty(ref _minRange, value); }
        }

        private double _maxRange;
        /// <summary>
        /// Maximum range for lidar. Currently not used in code, but may later be implemented for ignoring distances above selected value. <para />
        /// The manual states that 100 meters is maximum range.
        /// </summary>
        public double MaxRange
        {
            get { return _maxRange; }
            set { SetProperty(ref _maxRange, value); }
        }

        private CalculationType _defaultCalculationType;
        /// <summary>
        /// Calculation that is used by default when returning a single distance over a range (e.g. Fwd or Left)
        /// </summary>
        public CalculationType DefaultCalculationType
        {
            get { return _defaultCalculationType; }
            set { SetProperty(ref _defaultCalculationType, value); }
        }

        private VerticalAngle _defaultVerticalAngle;
        /// <summary>
        /// The vertical angle to use distances from, when no vertical angle is explicitly specified
        /// </summary>
        public VerticalAngle DefaultVerticalAngle
        {
            get { return _defaultVerticalAngle; }
            set { SetProperty(ref _defaultVerticalAngle, value); }
        }

        private int _defaultHalfBeamOpening;
        /// <summary>
        /// Specifies the width over which the Fwd, Left, Right and Aft distances are measured (e.g. 15 will cause Fwd to measure from 345 to 15 degrees)
        /// </summary>
        public int DefaultHalfBeamOpening
        {
            get { return _defaultHalfBeamOpening; }
            set { SetProperty(ref _defaultHalfBeamOpening, value); }
        }

        private int _numberOfCycles;
        /// <summary>
        /// The number of lidar revolutions to collect distances over before they become accessible for use <para />
        /// Higher number of cycles gives better resolution, but also longer delay between each update.
        /// </summary>
        public int NumberOfCycles
        {
            get { return _numberOfCycles; }
            set
            {
                int valueToSet = value > 0 ? value : 1;
                SetProperty(ref _numberOfCycles, valueToSet);
            }
        }

    }
}
