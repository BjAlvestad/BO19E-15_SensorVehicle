using System;

namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    public class Ultrasonic : IUltrasonic
    {
        public readonly TimeSpan MinimumSensorRequestsInterval = TimeSpan.FromMilliseconds(30);
        private static readonly object DistanceUpdateSyncLock = new object();
        private readonly IVehicleCommunication _vehicleCommunication;

        public Ultrasonic(IVehicleCommunication comWithUltrasonic)
        {
            _vehicleCommunication = comWithUltrasonic;
            PermissableDistanceAge = TimeSpan.FromMilliseconds(300);
            TimeStamp = DateTime.Now;
        }

        public DateTime TimeStamp { get; private set; }

        private TimeSpan _permissableDistanceAge;
        public TimeSpan PermissableDistanceAge // Consider namechange. Suggestions:  DistanceExpirationLimit  SensorDataExpirationLimit  PermissableSensorDataAge    SensorDataRequestInterval  RequestNewSensorDataLimit
        {
            get => _permissableDistanceAge;
            set => _permissableDistanceAge = (value > MinimumSensorRequestsInterval) ? value : MinimumSensorRequestsInterval;
        }

        private float _fwd;
        public float Fwd
        {
            get
            {
                UpdateDistanceProperties();
                return _fwd;
            }
            private set { _fwd = value; }
        }

        private float _left;
        public float Left
        {
            get
            {
                UpdateDistanceProperties();
                return _left;
            }
            private set { _left = value; }
        }

        private float _right;
        public float Right
        {
            get
            {
                UpdateDistanceProperties();
                return _right;
            }
            private set { _right = value; }
        }

        private void UpdateDistanceProperties()
        {
            lock (DistanceUpdateSyncLock)
            {
                if (DateTime.Now - TimeStamp <= PermissableDistanceAge) return;

                VehicleDataPacket data = _vehicleCommunication.Read();

                Left = data.Integers[0] / 100f;
                Fwd = data.Integers[1] / 100f;
                Right = data.Integers[2] / 100f;

                TimeStamp = DateTime.Now;        
            }
        }
    }
}
