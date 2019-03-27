using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    public class LidarDistance : ThreadSafeNotifyPropertyChanged, ILidarDistance
    {
        private bool _largestDistanceHasBeenCalculated;
        private bool _fwdHasBeenCalculated;
        private bool _leftHasBeenCalculated;
        private bool _rightHasBeenCalculated;
        private bool _aftHasBeenCalculated;
        private readonly ILidarPacketReceiver _packetReceiver;
        private CancellationTokenSource _collectorCancelToken;

        public ExclusiveSynchronizedObservableCollection<VerticalAngle> ActiveVerticalAngles { get; }

        public LidarDistance(ILidarPacketReceiver packetReceiver, params VerticalAngle[] verticalAngles)
        {
            _packetReceiver = packetReceiver;

            DefaultHalfBeamOpening = 15;
            DefaultCalculationType = CalculationType.Max; //TEMP
            DefaultVerticalAngle = verticalAngles[0];

            MinRange = 1.0;  // According to page 10 of VLP-16 user manual: 'points with distances less than one meter should be ignored'. But other sources claim smaller distances can be used.
            MaxRange = 100.0;  // According to page 3 of VLP-16 user manual: 'range from 1m to 100m'.

            NumberOfCycles = 3;

            ActiveVerticalAngles = new ExclusiveSynchronizedObservableCollection<VerticalAngle>();
            ActiveVerticalAngles.AddFromArray(verticalAngles);
        }

        public ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>> Distances { get; private set; }

        private double _minRange;
        public double MinRange
        {
            get { return _minRange; }
            set { SetProperty(ref _minRange, value);  }
        }

        //TODO: MaxRange is currently not in use. To be put in use or removed.
        private double _maxRange;
        public double MaxRange
        {
            get { return _maxRange; }
            set { SetProperty(ref _maxRange, value); }
        }

        private DateTime _lastUpdate;
        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            private set { SetProperty(ref _lastUpdate, value); }
        }

        public bool IsCollectorRunning { get; private set; }
        
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

        private string _message;
        public string Message
        {
            get { return _message; }
            private set { SetProperty(ref _message, value); }
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

        private bool _hasUnacknowledgedError;
        public bool HasUnacknowledgedError
        {
            get { return _hasUnacknowledgedError; }
            set { SetProperty(ref _hasUnacknowledgedError, value); }
        }

        private bool _runCollector;
        public bool RunCollector
        {
            get { return _runCollector; }
            set
            {
                SetProperty(ref _runCollector, value);
                //TODO: Change logic to start thread directly, and then remove the no longer needed 'IsCollectorRunning', 'StartCollector()', 'StopCollector()'
                if(value) StartCollector();
                else StopCollector();
            }
        }

        private HorizontalPoint _largestDistance;
        public HorizontalPoint LargestDistance
        {
            get
            {
                if (_largestDistanceHasBeenCalculated) return _largestDistance;
                if(Distances == null || !Distances.ContainsKey(DefaultVerticalAngle)) return new HorizontalPoint(float.NaN, float.NaN);

                _largestDistanceHasBeenCalculated = true;
                SetPropertyRaiseSelectively(ref _largestDistance, Distances[DefaultVerticalAngle].OrderByDescending(i => i.Distance).First());
                return _largestDistance;
            }
        }

        private float _fwd;
        public float Fwd
        {
            get
            {
                if (_fwdHasBeenCalculated)
                {
                    return _fwd;
                }

                _fwdHasBeenCalculated = true;
                SetPropertyRaiseSelectively(ref _fwd, GetDistance((360 - DefaultHalfBeamOpening), (0 + DefaultHalfBeamOpening)));
                return _fwd;
            }
        }

        private float _left;
        public float Left
        {
            get
            {
                if (_leftHasBeenCalculated)
                {
                    return _left;
                }

                _leftHasBeenCalculated = true;
                SetPropertyRaiseSelectively(ref _left, GetDistance(270 - DefaultHalfBeamOpening, 270 + DefaultHalfBeamOpening));
                return _left;
            }
        }

        private float _right;
        public float Right
        {
            get
            {
                if (_rightHasBeenCalculated)
                {
                    return _right;
                }

                _rightHasBeenCalculated = true;
                SetPropertyRaiseSelectively(ref _right, GetDistance(90 - DefaultHalfBeamOpening, 90 + DefaultHalfBeamOpening));
                return _right;
            }
        }

        private float _aft;
        public float Aft
        {
            get
            {
                if (_aftHasBeenCalculated)
                {
                    return _aft;
                }

                _aft = GetDistance(180 - DefaultHalfBeamOpening, 180 + DefaultHalfBeamOpening);
                _aftHasBeenCalculated = true;
                return _aft;
            }
        }

        public float GetDistance(float fromAngle, float toAngle)
        {
            return GetDistance(fromAngle, toAngle, DefaultVerticalAngle, DefaultCalculationType);
        }    
        
        public float GetDistance(float fromAngle, float toAngle, VerticalAngle verticalAngle, CalculationType calculationType)
        {
            if (Distances == null || (!Distances.ContainsKey(verticalAngle))) return float.NaN;

            List<float> distancesInRange = GetDistancesInRange(fromAngle, toAngle, verticalAngle);

            return PerformCalculation(distancesInRange, calculationType);
        }

        public List<float> GetDistancesInRange(float fromAngle, float toAngle, VerticalAngle verticalAngle)
        {
            if (Distances == null || (!Distances.ContainsKey(verticalAngle))) return new List<float>(){float.NaN};

            List<float> distancesInRange = new List<float>();
            List<HorizontalPoint> horizontalPointsInRange = GetHorizontalPointsInRange(fromAngle, toAngle, verticalAngle);

            foreach (HorizontalPoint point in horizontalPointsInRange)
            {
                distancesInRange.Add(point.Distance);
            }

            return distancesInRange;
        }

        public List<HorizontalPoint> GetHorizontalPointsInRange(float fromAngle, float toAngle, VerticalAngle verticalAngle)
        {
            if (Distances == null || (!Distances.ContainsKey(verticalAngle))) return new List<HorizontalPoint>(){new HorizontalPoint(0, float.NaN)};

            List<HorizontalPoint> pointsInRange = new List<HorizontalPoint>();
            bool angleSpansZero = fromAngle > toAngle;

            int startIndex = Distances[verticalAngle].FindIndex(point => point.Angle > fromAngle);

            //TEMP: New logic (and list is first sorted in LidarPacketInterpreter). Check which is fastest. Old or this.
            if (angleSpansZero)
            {
                if (startIndex != -1)
                {
                    for (int i = startIndex; i < Distances[verticalAngle].Count; i++)
                    {
                        pointsInRange.Add(Distances[verticalAngle][i]);
                    }
                }

                int endIndex = Distances[verticalAngle].FindIndex(point2 => point2.Angle > toAngle);
                for (int i = 0; i < endIndex; i++)
                {
                    pointsInRange.Add(Distances[verticalAngle][i]);
                }
            }
            else
            {
                if (startIndex == -1) return new List<HorizontalPoint>(){new HorizontalPoint(0, float.NaN)};

                int i = startIndex;
                HorizontalPoint point = Distances[verticalAngle][i];
                while (point.Angle < toAngle && i < Distances[verticalAngle].Count)
                {
                    point = Distances[verticalAngle][i];
                    pointsInRange.Add(point);
                    ++i;
                }
            }

            return pointsInRange;
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

        public void ClearMessage()
        {
            Message = "";
            HasUnacknowledgedError = false;
        }

        public void StartCollector()
        {
            if (IsCollectorRunning)
            {
                return;
            }
            IsCollectorRunning = true;
            _collectorCancelToken = new CancellationTokenSource();
            PeriodicUpdateDistancesAsync(TimeSpan.FromMilliseconds(10), _collectorCancelToken.Token);
        }

        public void StopCollector()
        {
            if (!IsCollectorRunning || _collectorCancelToken.IsCancellationRequested)
            {
                IsCollectorRunning = false;
                return;
            }
            _collectorCancelToken.Cancel();
            IsCollectorRunning = false;
        }

        public async Task PeriodicUpdateDistancesAsync(TimeSpan timeToWaitAfterUpdate, CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    Queue<byte[]> lidarPackets = await _packetReceiver.GetQueueOfDataPacketsAsync((byte)NumberOfCycles);
                    Distances = LidarPacketInterpreter.InterpretData(lidarPackets, ActiveVerticalAngles, (float)MinRange);
                    _largestDistanceHasBeenCalculated = false;
                    _fwdHasBeenCalculated = false;
                    _leftHasBeenCalculated = false;
                    _rightHasBeenCalculated = false;
                    _aftHasBeenCalculated = false;
                    LastUpdate = DateTime.Now;

                    await Task.Delay(timeToWaitAfterUpdate, cancellationToken);
                    //TODO: Verify if this is an ok way to do it. Fire-and-Forget seems to be discouraged. However this code has been suggested for situations like this.
                    // https://stackoverflow.com/questions/30462079/run-async-method-regularly-with-specified-interval
                    // https://blogs.msdn.microsoft.com/benwilli/2016/06/30/asynchronous-infinite-loops-instead-of-timers/
                }
            }
            catch (OperationCanceledException oce)
            {
                if(string.IsNullOrEmpty(Message)) Message = "Lidar collectors operation has been cancelled";
            }
            catch (Exception e)
            {
                Message = $"A collector error occured:\n{e.Message}\n\nStackTrace below\n{e.StackTrace}";
                HasUnacknowledgedError = true;
                RunCollector = false;
            }
        }
    }
}
