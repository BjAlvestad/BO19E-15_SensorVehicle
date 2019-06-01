using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
        private readonly IGpioPin _powerPin;
        private CancellationTokenSource _collectorCancelToken;
        private Stopwatch _collectionCycleStopwatch = new Stopwatch();


        public LidarDistance(ILidarPacketReceiver packetReceiver, IGpioPin powerPin, params VerticalAngle[] verticalAngles)
        {
            _packetReceiver = packetReceiver;
            _powerPin = powerPin;
            Config = new LidarDistanceConfiguration(verticalAngles);

            Error = new Error();
        }

        public bool Power
        {
            get { return _powerPin.PinHigh; }
            set
            {
                if (value == false)
                {
                    RunCollector = false;
                }

                try
                {
                    _powerPin.PinHigh = value;
                }
                catch (Exception e)
                {
                    Error.Message = $"An error occured when trying to switch lidar power {(value ? "on" : "off")}\n{e.Message}";
                    Error.DetailedMessage = e.ToString();
                    Error.Unacknowledged = true;
                    RunCollector = false;
                }
                RaiseSyncedPropertyChanged();
            }
        }

        public Error Error { get; }

        public LidarDistanceConfiguration Config { get; }

        public ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>> Distances { get; private set; }

        private DateTime _lastUpdate;
        public DateTime LastUpdate
        {
            get { return _lastUpdate; }
            private set { SetProperty(ref _lastUpdate, value); }
        }

        private long _lastCollectionDuration;
        public long LastCollectionDuration
        {
            get { return _lastCollectionDuration; }
            private set { SetPropertyRaiseSelectively(ref _lastCollectionDuration, value); }
        }

        private long _lastDataInterpretationDuration;
        public long LastDataInterpretationDuration
        {
            get { return _lastDataInterpretationDuration; }
            private set { SetPropertyRaiseSelectively(ref _lastDataInterpretationDuration, value); }
        }

        public bool IsCollectorRunning { get; private set; }
        
        private bool _runCollector;
        public bool RunCollector
        {
            get { return _runCollector; }
            set
            {
                SetProperty(ref _runCollector, value);
                //TODO: Change logic to start thread directly, and then remove the no longer needed 'IsCollectorRunning', 'StartCollector()', 'StopCollector()'
                if(value) StartCollector();
                else
                {
                    StopCollector();
                    RaiseNotificationForSelective = false;
                }
            }
        }

        private HorizontalPoint _largestDistance;
        public HorizontalPoint LargestDistance
        {
            get
            {
                if (_largestDistanceHasBeenCalculated) return _largestDistance;
                if(Distances == null || !Distances.ContainsKey(Config.DefaultVerticalAngle)) return new HorizontalPoint(float.NaN, float.NaN);

                _largestDistanceHasBeenCalculated = true;
                SetPropertyRaiseSelectively(ref _largestDistance, Distances[Config.DefaultVerticalAngle].OrderByDescending(i => i.Distance).First());
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
                SetPropertyRaiseSelectively(ref _fwd, GetDistance((360 - Config.DefaultHalfBeamOpening), (0 + Config.DefaultHalfBeamOpening)));
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
                SetPropertyRaiseSelectively(ref _left, GetDistance(270 - Config.DefaultHalfBeamOpening, 270 + Config.DefaultHalfBeamOpening));
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
                SetPropertyRaiseSelectively(ref _right, GetDistance(90 - Config.DefaultHalfBeamOpening, 90 + Config.DefaultHalfBeamOpening));
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

                _aft = GetDistance(180 - Config.DefaultHalfBeamOpening, 180 + Config.DefaultHalfBeamOpening);
                _aftHasBeenCalculated = true;
                return _aft;
            }
        }

        public HorizontalPoint LargestDistanceInRange(float fromAngle, float toAngle)
        {
            if(Distances == null || !Distances.ContainsKey(Config.DefaultVerticalAngle)) return new HorizontalPoint(float.NaN, float.NaN);

            bool angleSpansZero = fromAngle > toAngle;
            if (angleSpansZero)
            {
                return Distances[Config.DefaultVerticalAngle].OrderByDescending(i => i.Distance).First(i => (i.Angle < toAngle || i.Angle > fromAngle));
            }
            else
            {
                return Distances[Config.DefaultVerticalAngle].OrderByDescending(i => i.Distance).First(i => (i.Angle > fromAngle && i.Angle < toAngle));
            }
            //TODO: Check what happens if there is no available distance in range
        }

        public float GetDistance(float fromAngle, float toAngle)
        {
            return GetDistance(fromAngle, toAngle, Config.DefaultVerticalAngle, Config.DefaultCalculationType);
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

        private async Task PeriodicUpdateDistancesAsync(TimeSpan timeToWaitAfterUpdate, CancellationToken cancellationToken)
        {
            try
            {
                while (true)
                {
                    _collectionCycleStopwatch.Start();
                    Queue<byte[]> lidarPackets = await _packetReceiver.GetQueueOfDataPacketsAsync((byte)Config.NumberOfCycles);
                    LastCollectionDuration = _collectionCycleStopwatch.ElapsedMilliseconds;

                    _collectionCycleStopwatch.Restart();
                    Distances = LidarPacketInterpreter.InterpretData(lidarPackets, Config.ActiveVerticalAngles, (float)Config.MinRange);
                    LastDataInterpretationDuration = _collectionCycleStopwatch.ElapsedMilliseconds;
                    _collectionCycleStopwatch.Reset();

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
                // This is not an error, but an expected exception when collector is stopped (cancelled)
                Debug.WriteLine("Lidar collector was stopped (cancelled).");
            }
            catch (Exception e)
            {
                Error.Message = $"A collector error occured:\n{e.Message}";
                Error.DetailedMessage = e.ToString();
                Error.Unacknowledged = true;
                RunCollector = false;
            }
        }
    }
}
