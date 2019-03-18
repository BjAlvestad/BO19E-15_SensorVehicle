using System;
using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace VehicleEquipment.Locomotion.Encoder
{
    public class Encoders : ThreadSafeNotifyPropertyChanged, IEncoders
    {
        public Encoders(Encoder encoderLeft, Encoder encoderRight)
        {
            Left = encoderLeft;
            Right = encoderRight;
            CollectionInterval = TimeSpan.FromMilliseconds(500);
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

        private TimeSpan _collectionInterval;
        public TimeSpan CollectionInterval
        {
            get { return _collectionInterval; }
            set { SetProperty(ref _collectionInterval, value); }
        }

        private bool _collectContinously;
        public bool CollectContinously
        {
            get { return _collectContinously; }
            set
            {
                SetProperty(ref _collectContinously, value);
                if (value)
                {
                    Task.Run(() =>
                    {
                        while (CollectContinously)
                        {
                            CollectAndResetDistanceFromEncoders();
                            Thread.Sleep(CollectionInterval.Milliseconds);
                        }
                    });
                }
            }
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
