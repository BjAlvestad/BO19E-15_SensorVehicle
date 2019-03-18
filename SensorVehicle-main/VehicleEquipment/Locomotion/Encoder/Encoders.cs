using Helpers;

namespace VehicleEquipment.Locomotion.Encoder
{
    public class Encoders : ThreadSafeNotifyPropertyChanged, IEncoders
    {
        public Encoders(Encoder encoderLeft, Encoder encoderRight)
        {
            Left = encoderLeft;
            Right = encoderRight;
        }

        private Encoder _left;
        public Encoder Left
        {
            get { return _left; }
            set { SetPropertyRaiseSelectively(ref _left, value); }
        }

        private Encoder _right;
        public Encoder Right
        {
            get { return _right; }
            set { SetPropertyRaiseSelectively(ref _right, value); }
        }

        public void ResetTotalDistanceTraveled()
        {
            Left.TotalDistanceTravelled = 0;
            Right.TotalDistanceTravelled = 0;
            RaiseSyncedPropertyChangedSelectively(nameof(Left));
            RaiseSyncedPropertyChangedSelectively(nameof(Right));
        }

        public void CollectAndResetDistanceFromEncoders()
        {
            Left.CollectAndResetDistanceFromEncoder();
            RaiseSyncedPropertyChangedSelectively(nameof(Left));
            Right.CollectAndResetDistanceFromEncoder();
            RaiseSyncedPropertyChangedSelectively(nameof(Right));
        }
    }
}
