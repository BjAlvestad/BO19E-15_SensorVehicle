using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    public class Ultrasonic : ThreadSafeNotifyPropertyChanged, IUltrasonic
    {
        public readonly int MinimumSensorRequestsInterval = 30;
        private static readonly object DistanceUpdateSyncLock = new object();
        private readonly IVehicleCommunication _vehicleCommunication;
        private readonly IGpioOutputPin _ultrasoundI2cIsolationPin;

        public Ultrasonic(IVehicleCommunication comWithUltrasonic, IGpioOutputPin ultrasoundI2cIsolationPin)
        {
            _vehicleCommunication = comWithUltrasonic;
            _ultrasoundI2cIsolationPin = ultrasoundI2cIsolationPin;
            PermissableDistanceAge = 300;
            TimeStamp = DateTime.Now;
            Error = new Error();
        }

        public bool DeisolateI2cCommunciation
        {
            get { return _ultrasoundI2cIsolationPin.SetOutput; }
            set
            {
                if (value == false)
                {
                    RaiseNotificationForSelective = false;
                }

                try
                {
                    _ultrasoundI2cIsolationPin.SetOutput = value;
                }
                catch (Exception e)
                {
                    Error.Message = $"An error occured when trying to {(value ? "de-isolate" : "isolate")} ultrasonics I2c communication\n{e.Message}";
                    Error.DetailedMessage = e.ToString();
                    Error.Unacknowledged = true;
                }
                RaiseSyncedPropertyChanged();
            }
        }

        private bool _raiseNotificationForSelective;
        public override bool RaiseNotificationForSelective
        {
            get { return _raiseNotificationForSelective; }
            set
            {
                SetProperty(ref _raiseNotificationForSelective, value);
                if (value == false)
                {
                    RefreshUltrasonicContinously = false;
                }
            }
        }

        public Error Error { get; }

        private int _permissableDistanceAge;
        public int PermissableDistanceAge //TODO: Consider name change. Suggestions:  DistanceExpirationLimit  SensorDataExpirationLimit  PermissableSensorDataAge    SensorDataRequestInterval  RequestNewSensorDataLimit
        {
            get => _permissableDistanceAge;
            set
            {
                int newPermissableDistanceAge = (value > MinimumSensorRequestsInterval) ? value : MinimumSensorRequestsInterval;
                SetProperty(ref _permissableDistanceAge, newPermissableDistanceAge);
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
                            Thread.Sleep(PermissableDistanceAge / 2);
                        }
                    });
                }
            }
        }

        private void UpdateDistanceProperties()
        {
            lock (DistanceUpdateSyncLock)
            {
                if ((DateTime.Now - TimeStamp).TotalMilliseconds <= PermissableDistanceAge) return;

                if (Error.Unacknowledged)
                {
                    RefreshUltrasonicContinously = false;
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
                    Error.Message = e.Message;
                    Error.DetailedMessage = e.ToString();
                    Error.Unacknowledged = true;
                }
            }
        }
    }
}
