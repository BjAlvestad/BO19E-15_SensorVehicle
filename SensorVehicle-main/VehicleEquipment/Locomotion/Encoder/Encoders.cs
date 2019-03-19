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

        public void ResetTotalDistanceTraveled()
        {
            Left.TotalDistanceTravelled = 0;
            Left.TotalTime = TimeSpan.Zero;
            Right.TotalDistanceTravelled = 0;
            Right.TotalTime = TimeSpan.Zero;
            RaiseSyncedPropertyChangedSelectively(nameof(Left));
            RaiseSyncedPropertyChangedSelectively(nameof(Right));
        }

        public void CollectAndResetDistanceFromEncoders()
        {
            if (HasUnacknowledgedError)
            {
                CollectContinously = false;
                return;
            }

            try
            {
                Left.CollectAndResetDistanceFromEncoder();
                RaiseSyncedPropertyChangedSelectively(nameof(Left));
            }
            catch (Exception e)
            {
                Message += $"ERROR: An exception occured when collecting encoder data from Left Encoder:\n{e.Message}\n\n";
                HasUnacknowledgedError = true;
            }

            try
            {
                Right.CollectAndResetDistanceFromEncoder();
                RaiseSyncedPropertyChangedSelectively(nameof(Right));
            }
            catch (Exception e)
            {
                Message += $"ERROR: An exception occured when collecting encoder data from Right Encoder:\n{e.Message}\n\n";
                HasUnacknowledgedError = true;
            }
        }
    }
}
