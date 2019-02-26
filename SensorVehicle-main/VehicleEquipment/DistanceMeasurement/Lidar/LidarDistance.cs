using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    public class LidarDistance : ThreadSafeNotifyPropertyChanged, ILidarDistance, IDistance
    {
        public CalculationType DefaultCalculationType { get; set; }
        public VerticalAngle DefaultVerticalAngle { get; set; }


        private bool _fwdHasBeenCalculated;
        private bool _leftHasBeenCalculated;
        private bool _rightHasBeenCalculated;
        private bool _aftHasBeenCalculated;
        private ILidarPacketReceiver _packetReceiver;
        private CancellationTokenSource _collectorCancelToken;
        private HashSet<VerticalAngle> _activeVerticalAngles; // HashSet will not contain any duplicate value

        public LidarDistance(ILidarPacketReceiver packetReceiver, params VerticalAngle[] verticalAngles)
        {
            _packetReceiver = packetReceiver;
            DefaultCalculationType = CalculationType.Max; //TEMP

            MinRange = 1.0f;  // According to page 10 of VLP-16 user manual: 'points with distances less than one meter should be ignored'.
            MaxRange = 100.0f;  // According to page 3 of VLP-16 user manual: 'range from 1m to 100m'.
            Resolution = float.NaN;  // Don't know the distance resolution

            NumberOfCycles = 3;
            _activeVerticalAngles = new HashSet<VerticalAngle>(verticalAngles);
        }

        public ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>> Distances { get; private set; }

        public float MinRange { get; private set; }
        public float MaxRange { get; private set; }
        public float Resolution { get; private set; }
        public DateTime LastUpdate { get; private set; }
        public bool IsCollectorRunning { get; private set; }
        public string Message { get; private set; }
        public byte NumberOfCycles { get; set; }

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

        private float _fwd;
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

        private float _left;
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

        private float _right;
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

        private float _aft;
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
            bool angleSpansZero = fromAngle > toAngle;

            int startIndex = Distances[verticalAngle].FindIndex(point => point.Angle > fromAngle);
            if (startIndex == -1) return new List<float>(){float.NaN};

            //TEMP: New logic (and list is first sorted in LidarPacketInterpreter). Check which is fastest. Old or this.
            if (angleSpansZero)
            {
                for (int i = startIndex; i < Distances[verticalAngle].Count; i++)
                {
                    distancesInRange.Add(Distances[verticalAngle][i].Distance);
                }

                int endIndex = Distances[verticalAngle].FindIndex(point2 => point2.Angle > toAngle);
                for (int i = 0; i < endIndex; i++)
                {
                    distancesInRange.Add(Distances[verticalAngle][i].Distance);
                }
            }
            else
            {
                int i = startIndex;
                HorizontalPoint point = Distances[verticalAngle][i];
                while (point.Angle < toAngle && i < Distances[verticalAngle].Count)
                {
                    point = Distances[verticalAngle][i];
                    distancesInRange.Add(point.Distance);
                    ++i;
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
                    values.Sort();  //BUG: This returns the distance of the median angle (not the median distance). Change the sort to sort by distance.
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
                    Queue<byte[]> lidarPackets = await _packetReceiver.GetQueueOfDataPacketsAsync(NumberOfCycles);
                    Distances = LidarPacketInterpreter.InterpretData(lidarPackets, _activeVerticalAngles);  //TODO: Change LidarPacketInterpreter to utilize HashSet instead of List
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
            catch (Exception e)
            {
                Message = $"A collector error occured:\n{e.Message}";
                HasUnacknowledgedError = true;
                StopCollector();
            }
        }
    }
}
