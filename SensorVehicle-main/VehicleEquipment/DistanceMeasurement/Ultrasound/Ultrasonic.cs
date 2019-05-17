using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    public class Ultrasonic : ThreadSafeNotifyPropertyChanged, IUltrasonic
    {
        private static readonly object DistanceUpdateSyncLock = new object();
        private readonly IVehicleCommunication _vehicleCommunication;
        private readonly IGpioPin _newDataAvailablePin;

        public Ultrasonic(IVehicleCommunication comWithUltrasonic, IGpioPin powerPin, IGpioPin ultrasoundInterruptPin)
        {
            _vehicleCommunication = comWithUltrasonic;
            _power = powerPin;
            TimeStamp = DateTime.Now;
            Error = new Error();

            _newDataAvailablePin = ultrasoundInterruptPin;
            Power = true;
        }

        private readonly IGpioPin _power;
        public bool Power
        {
            get { return _power.PinHigh; }
            set
            {
                if (value == false)
                {
                    RaiseNotificationForSelective = false;
                }

                try
                {
                    _power.PinHigh = value;
                }
                catch (Exception e)
                {
                    Error.Message = $"An error occured when trying to switch ultrasonic power {(value ? "on" : "off")}\n{e.Message}";
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
        
        private bool _refreshUltrasonicContinously;
        public bool RefreshUltrasonicContinously
        {
            get { return _refreshUltrasonicContinously; }
            set
            {
                if (value && !_refreshUltrasonicContinously)
                {
                    _newDataAvailablePin.PinValueInputChangedHigh += _newDataAvailablePin_PinValueInputChangedHigh;
                    UpdateDistanceProperties();
                }
                else if (!value && _refreshUltrasonicContinously)
                {
                    _newDataAvailablePin.PinValueInputChangedHigh -= _newDataAvailablePin_PinValueInputChangedHigh;
                }
                SetProperty(ref _refreshUltrasonicContinously, value);
            }
        }

        private void _newDataAvailablePin_PinValueInputChangedHigh(object sender, EventArgs e)
        {
            UpdateDistanceProperties();
        }

        private void UpdateDistanceProperties()
        {
            lock (DistanceUpdateSyncLock)
            {
                if (!_newDataAvailablePin.PinHigh) return;

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
                    RefreshUltrasonicContinously = false;
                }
            }
        }
    }
}
