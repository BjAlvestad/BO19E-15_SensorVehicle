using System;
using Prism.Mvvm;

namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    public class Ultrasonic : BindableBase, IUltrasonic
    {
        private static readonly object DistanceUpdateSyncLock = new object();
        private readonly IVehicleCommunication _vehicleCommunication;

        public Ultrasonic(IVehicleCommunication comWithUltrasonic)
        {
            _vehicleCommunication = comWithUltrasonic;
        }
        
        private float _fwd;
        public float Fwd
        {
            get
            {
                UpdateDistanceProperties();
                return _fwd;
            }
            private set { SetProperty(ref _fwd, value); }
        }

        private float _left;
        public float Left
        {
            get
            {
                UpdateDistanceProperties();
                return _left;
            }
            private set { SetProperty(ref _left, value); }
        }

        private float _right;
        public float Right
        {
            get
            {
                UpdateDistanceProperties();
                return _right;
            }
            private set { SetProperty(ref _right, value); }
        }

        private DateTime _timeStamp;
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            private set { SetProperty(ref _timeStamp, value); }
        }

        private TimeSpan _permissableDistanceAge;
        public TimeSpan PermissableDistanceAge
        {
            get { return _permissableDistanceAge; }
            set { SetProperty(ref _permissableDistanceAge, value); }
        }

        private void UpdateDistanceProperties()
        {
            lock (DistanceUpdateSyncLock)
            {
                if (DateTime.Now - TimeStamp <= PermissableDistanceAge) return;

                string[] distances = GetNewDistances();

                Left = Convert.ToSingle(distances[0]);
                Fwd = Convert.ToSingle(distances[1]);
                Right = Convert.ToSingle(distances[2]);
                TimeStamp = DateTime.Now;        
            }
        }

        private string[] GetNewDistances()
        {
            string text = "";
            byte[] bytes = _vehicleCommunication.Read();
            foreach (byte b in bytes)
            {
                text = text + (char)b;
            }

            return text.Split('-');
        }
    }
}
