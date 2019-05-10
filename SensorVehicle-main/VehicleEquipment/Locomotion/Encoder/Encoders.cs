using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace VehicleEquipment.Locomotion.Encoder
{
    public class Encoders : ThreadSafeNotifyPropertyChanged, IEncoders
    {
        private const int MinimumCollectionIntervalInMilliseconds = 50;

        private readonly IGpioPin _powerPin;

        public Encoders(Encoder encoderLeft, Encoder encoderRight, IGpioPin powerPin)
        {
            Left = encoderLeft;
            Right = encoderRight;
            _powerPin = powerPin;
            CollectionInterval = 500;
            Error = new Error();

            Power = true;
        }

        public bool Power
        {
            get { return _powerPin.PinHigh; }
            set
            {
                try
                {
                    if (!value)
                    {
                        RaiseNotificationForSelective = false;
                        CollectContinously = false;
                    }

                    _powerPin.PinHigh = value;
                }
                catch (Exception e)
                {
                    Error.Message = $"An error occured when trying to switch encoder power {(value ? "on" : "off")}\n{e.Message}";
                    Error.DetailedMessage = e.ToString();
                    Error.Unacknowledged = true;
                }
                RaiseSyncedPropertyChanged();
            }
        }

        public Error Error { get; }

        public Encoder Left { get; }
        public Encoder Right { get; }

        private int _collectionInterval;
        public int CollectionInterval
        {
            get { return _collectionInterval; }
            set
            {
                int newCollectionInterval = value > MinimumCollectionIntervalInMilliseconds ? value : MinimumCollectionIntervalInMilliseconds;
                SetProperty(ref _collectionInterval, newCollectionInterval);
            }
        }

        //TODO: Standardise CollectContinously code over all the classes that utilizes similar mechanichs (find out what would be the best solution first).
        private CancellationTokenSource _collectorTokenSource;
        private bool _collectContinously;
        public bool CollectContinously
        {
            get { return _collectContinously; }
            set
            {
                if (value == _collectContinously) return;

                _collectContinously = value;

                if (value && _collectorTokenSource == null)
                {
                    _collectorTokenSource = new CancellationTokenSource();
                    Task.Run(async () =>
                    {
                        try
                        {
                            while (_collectContinously)
                            {
                                CollectAndResetDistanceFromEncoders();
                                await Task.Delay(CollectionInterval, _collectorTokenSource.Token);
                            }
                        }
                        catch (TaskCanceledException tce)
                        {
                            Debug.WriteLine("Encoder collection cancelled...");
                        }
                    }, _collectorTokenSource.Token);
                }
                else
                {
                    _collectorTokenSource?.Cancel();
                    _collectorTokenSource = null;
                }

                RaiseSyncedPropertyChanged();
            }
        }

        public void ResetTotalDistanceTraveled()
        {
            Left.ResetTotalDistanceTraveled();
            Right.ResetTotalDistanceTraveled();
            RaiseSyncedPropertyChangedSelectively(nameof(Left));
            RaiseSyncedPropertyChangedSelectively(nameof(Right));
        }

        public void CollectAndResetDistanceFromEncoders()
        {
            if (Error.Unacknowledged)
            {
                CollectContinously = false;
                return;
            }

            Left.CollectAndResetDistanceFromEncoder();
            RaiseSyncedPropertyChangedSelectively(nameof(Left));

            Right.CollectAndResetDistanceFromEncoder();
            RaiseSyncedPropertyChangedSelectively(nameof(Right));

            CheckAndCollectErrors();
        }

        public void ClearAllEncoderErrors()
        {
            Left.Error.Clear();
            Right.Error.Clear();
            Error.Clear();
        }

        private void CheckAndCollectErrors()
        {
            if (Left.Error.Unacknowledged && Right.Error.Unacknowledged)
            {
                Error.Message = $"LEFT ENCODER:\n{Left.Error.Message}\n\n" +
                                $"RIGHT ENCODER:\n{Right.Error.Message}";
                Error.DetailedMessage = $"LEFT ENCODER:\n{Left.Error.DetailedMessage}\n\n" +
                                        $"RIGHT ENCODER:\n{Right.Error.DetailedMessage}";
                Error.Unacknowledged = true;
            }
            else if (Left.Error.Unacknowledged)
            {
                Error.Message = $"LEFT ENCODER:\n{Left.Error.Message}";
                Error.DetailedMessage = $"LEFT ENCODER:\n{Left.Error.DetailedMessage}";
                Error.Unacknowledged = true;
            }
            else if (Right.Error.Unacknowledged)
            {
                Error.Message = $"RIGHT ENCODER:\n{Right.Error.Message}";
                Error.DetailedMessage = $"RIGHT ENCODER:\n{Right.Error.DetailedMessage}";
                Error.Unacknowledged = true;
            }
        }
    }
}
