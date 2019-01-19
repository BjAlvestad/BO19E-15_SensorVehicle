using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    public class LidarDistance
    {
        public ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>> Distances
        {
            get
            {
                //eventWaitHandle.WaitOne();  // BUG: Can't use eventWaitHandle here due to it causing significant slowdown (maybe copy out list before enumerating it instead)
                return _distances;
            }
            private set { _distances = value; }
        }

        public CalculationType DefaultCalculationType { get; set; }

        private bool _fwdHasBeenCalculated;
        private bool _leftHasBeenCalculated;
        private bool _rightHasBeenCalculated;
        private bool _aftHasBeenCalculated;
        private float _fwd;
        private float _left;
        private float _right;
        private float _aft;

        EventWaitHandle eventWaitHandle = new AutoResetEvent(false);
        private ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>> _distances;

        public LidarDistance(CalculationType defaultCalculationType = CalculationType.Max)
        {
            Distances = new ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>>(new Dictionary<VerticalAngle, List<HorizontalPoint>>());
            DefaultCalculationType = defaultCalculationType;
            LidarDistanceCollector.Run = true;
            LidarDistanceCollector.NewDistances += OnNewDistances;
        }

        public float Fwd
        {
            get
            {
                if (_fwdHasBeenCalculated)
                {
                    return _fwd;
                }

                _fwd = GetDistance(345, 15);
                _fwdHasBeenCalculated = true;
                return _fwd;
            }
        }

        public float Left
        {
            get
            {
                if (_leftHasBeenCalculated)
                {
                    return _left;
                }

                _left = GetDistance(255, 285);
                _leftHasBeenCalculated = true;
                return _left;
            }
        }

        public float Right
        {
            get
            {
                if (_rightHasBeenCalculated)
                {
                    return _right;
                }

                _right = GetDistance(75, 105);
                _rightHasBeenCalculated = true;
                return _right;
            }
        }

        public float Aft
        {
            get
            {
                if (_aftHasBeenCalculated)
                {
                    return _aft;
                }

                _aft = GetDistance(165, 195);
                _aftHasBeenCalculated = true;
                return _aft;
            }
        }

        public float GetDistance(float fromAngle, float toAngle, VerticalAngle verticalAngle = VerticalAngle.Up3)
        {
            return GetDistance(fromAngle, toAngle, DefaultCalculationType, verticalAngle);
        }    
        
        public float GetDistance(float fromAngle, float toAngle, CalculationType calculationType, VerticalAngle verticalAngle = VerticalAngle.Up3)
        {
            if (Distances == null || (!Distances.ContainsKey(verticalAngle))) return float.NaN;

            List<float> distancesInRange = GetDistancesInRange(fromAngle, toAngle, verticalAngle);

            return PerformCalculation(distancesInRange, calculationType);
        }

        public List<float> GetDistancesInRange(float fromAngle, float toAngle, VerticalAngle verticalAngle)
        {
            if (Distances == null || (!Distances.ContainsKey(verticalAngle))) return new List<float>(){float.NaN};

            List<float> distancesInRange = new List<float>();
            bool angleSpansZero = fromAngle > toAngle;

            foreach (HorizontalPoint point in Distances[verticalAngle])
            {
                if (angleSpansZero && (point.Angle > fromAngle || point.Angle < toAngle))
                {
                    distancesInRange.Add(point.Distance);
                }
                else if (point.Angle > fromAngle && point.Angle < toAngle)
                {
                    distancesInRange.Add(point.Distance);
                }
            }

            return distancesInRange;
        }

        private float PerformCalculation(List<float> values, CalculationType calculationType)
        {
            if (values.Count == 0) return float.NaN;

            switch (calculationType)
            {
                case CalculationType.Min:
                    return values.Min();
                case CalculationType.Max:
                    return values.Max();
                case CalculationType.Mean:
                    return values.Average(); 
                case CalculationType.Median:
                    values.Sort();
                    return values[values.Count / 2];
                default:
                    throw new ArgumentOutOfRangeException(nameof(calculationType), calculationType, null);
            }
        }

        private void OnNewDistances(object sender, LidarDistanceEventArgs e)
        {
            //eventWaitHandle.Reset();
            Distances = e.LidarCycles;
            //eventWaitHandle.Set();

            _fwdHasBeenCalculated = false;
            _leftHasBeenCalculated = false;
            _rightHasBeenCalculated = false;
            _aftHasBeenCalculated = false;
        }
    }
}
