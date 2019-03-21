using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    public class Ultrasonic : ThreadSafeNotifyPropertyChanged, IUltrasonic
    {
        public readonly TimeSpan MinimumSensorRequestsInterval = TimeSpan.FromMilliseconds(30);
        private static readonly object DistanceUpdateSyncLock = new object();
        private readonly IVehicleCommunication _vehicleCommunication;

        public Ultrasonic(IVehicleCommunication comWithUltrasonic)
        {
            _vehicleCommunication = comWithUltrasonic;
            PermissableDistanceAge = TimeSpan.FromMilliseconds(300);
            TimeStamp = DateTime.Now;
            Message = "";
        }

        private TimeSpan _permissableDistanceAge;
        public TimeSpan PermissableDistanceAge // Consider namechange. Suggestions:  DistanceExpirationLimit  SensorDataExpirationLimit  PermissableSensorDataAge    SensorDataRequestInterval  RequestNewSensorDataLimit
        {
            get => _permissableDistanceAge;
            set
            {
                _permissableDistanceAge = (value > MinimumSensorRequestsInterval) ? value : MinimumSensorRequestsInterval;
                RaiseSyncedPropertyChanged();
            }
        }

        private DateTime _timeStamp;
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            private set { SetPropertyRaiseSelectively(ref _timeStamp, value); }
        }

        public float Fwd => Math.Min(FwdLeft, FwdRight);

        private float _fwdLeft;
        public float FwdLeft
        {
            get
            {
                UpdateDistanceProperties();
                return _fwdLeft;
            }
            private set { SetPropertyRaiseSelectively(ref _fwdLeft, value); }
        }
        
        private float _fwdRight;
        public float FwdRight
        {
            get
            {
                UpdateDistanceProperties();
                return _fwdRight;
            }
            private set { SetPropertyRaiseSelectively(ref _fwdRight, value); }
        }

        private float _left;
        public float Left
        {
            get
            {
                UpdateDistanceProperties();
                return _left;
            }
            private set { SetPropertyRaiseSelectively(ref _left, value); }
        }

        private float _right;
        public float Right
        {
            get
            {
                UpdateDistanceProperties();
                return _right;
            }
            private set { SetPropertyRaiseSelectively(ref _right, value); }
        }

        private bool _hasUnacknowledgedError;
        public bool HasUnacknowledgedError
        {
            get { return _hasUnacknowledgedError; }
            set { SetProperty(ref _hasUnacknowledgedError, value); }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            private set { SetProperty(ref _message, value); }
        }

        public void ClearMessage()
        {
            Message = "";
            HasUnacknowledgedError = false;
        }
        
        //TODO: This property should be removed once pin-interrupt on new distance data available has been set up (Allready connected and implemented on microcontroller side - GPIO5 on SBC)
        private bool _refreshUltrasonicContinously;
        public bool RefreshUltrasonicContinously
        {
            get { return _refreshUltrasonicContinously; }
            set
            {
                SetProperty(ref _refreshUltrasonicContinously, value);
                if (value)
                {
                    Task.Run(() =>
                    {
                        while (RefreshUltrasonicContinously)
                        {
                            UpdateDistanceProperties();
                            Thread.Sleep(PermissableDistanceAge.Milliseconds / 2);
                        }
                    });
                }
            }
        }

        private void UpdateDistanceProperties()
        {
            lock (DistanceUpdateSyncLock)
            {
                if (DateTime.Now - TimeStamp <= PermissableDistanceAge) return;

                if (HasUnacknowledgedError)
                {
                    Left = float.NaN;
                    FwdLeft = float.NaN;
                    FwdRight = float.NaN;
                    Right = float.NaN;
                    return;
                }

                try
                {
                    VehicleDataPacket data = _vehicleCommunication.Read();

                    Left = data.Integers[0] / 100f;
                    FwdRight = data.Integers[1] / 100f;
                    Right = data.Integers[2] / 100f;
                    FwdLeft = data.Integers[3] / 100f;

                    TimeStamp = DateTime.Now;
                }
                catch (Exception e)
                {
                    Message += $"ERROR OCCURED WHEN UPDAING DISTANCES: \n{e.Message}\n" +
                                    "*************************************\n";
                    HasUnacknowledgedError = true;
                }
            }
        }
    }
}
