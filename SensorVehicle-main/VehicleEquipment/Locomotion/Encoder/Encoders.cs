﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Helpers;

namespace VehicleEquipment.Locomotion.Encoder
{
    public class Encoders : ThreadSafeNotifyPropertyChanged, IEncoders
    {
        private const int MinimumCollectionIntervalInMilliseconds = 50;

        public Encoders(Encoder encoderLeft, Encoder encoderRight)
        {
            Left = encoderLeft;
            Right = encoderRight;
            CollectionInterval = 500;
        }

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
            Left.ResetTotalDistanceTraveled();
            Right.ResetTotalDistanceTraveled();
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
