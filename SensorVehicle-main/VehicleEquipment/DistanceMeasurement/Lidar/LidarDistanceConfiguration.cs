using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Helpers;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    public class LidarDistanceConfiguration : ThreadSafeNotifyPropertyChanged
    {
        public ExclusiveSynchronizedObservableCollection<VerticalAngle> ActiveVerticalAngles { get; }

        public LidarDistanceConfiguration(VerticalAngle[] verticalAngles)
        {
            DefaultHalfBeamOpening = 15;
            DefaultCalculationType = CalculationType.Max; //TEMP

            ActiveVerticalAngles = new ExclusiveSynchronizedObservableCollection<VerticalAngle>();
            DefaultVerticalAngle = verticalAngles[0];
            ActiveVerticalAngles.AddFromArray(verticalAngles);

            NumberOfCycles = 3;

            MinRange = 1.0;  // According to page 10 of VLP-16 user manual: 'points with distances less than one meter should be ignored'. But other sources claim smaller distances can be used.
            MaxRange = 100.0;  // According to page 3 of VLP-16 user manual: 'range from 1m to 100m'.
        }

        private double _minRange;
        public double MinRange
        {
            get { return _minRange; }
            set { SetProperty(ref _minRange, value); }
        }

        //TODO: MaxRange is currently not in use. To be put in use or removed.
        private double _maxRange;
        public double MaxRange
        {
            get { return _maxRange; }
            set { SetProperty(ref _maxRange, value); }
        }

        private CalculationType _defaultCalculationType;
        public CalculationType DefaultCalculationType
        {
            get { return _defaultCalculationType; }
            set { SetProperty(ref _defaultCalculationType, value); }
        }

        private VerticalAngle _defaultVerticalAngle;
        public VerticalAngle DefaultVerticalAngle
        {
            get { return _defaultVerticalAngle; }
            set { SetProperty(ref _defaultVerticalAngle, value); }
        }

        private int _defaultHalfBeamOpening;
        public int DefaultHalfBeamOpening
        {
            get { return _defaultHalfBeamOpening; }
            set { SetProperty(ref _defaultHalfBeamOpening, value); }
        }

        private int _numberOfCycles;
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
