﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Helpers
{
    // Used Prisms BindableBase code as a starting point for this class. https://github.com/PrismLibrary/Prism/blob/master/Source/Prism/Mvvm/BindableBase.cs (22.02.2019)
    public class ThreadSafeNotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly SynchronizationContext _uiSyncContext;

        /// <summary>
        /// Instansiates a class for raising property changed notification from threads other than UI thread.
        /// NB: This class must be instantiated from a UI thread, or the UI threads must be passed inn as an argument in the constructor.
        /// </summary>
        public ThreadSafeNotifyPropertyChanged()
        {
            _uiSyncContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Instansiates a class for raising property changed notification from threads other than UI thread.
        /// </summary>
        /// <param name="uiSyncContext">The synchronization context for the UI thread</param>
        public ThreadSafeNotifyPropertyChanged(SynchronizationContext uiSyncContext)
        {
            _uiSyncContext = uiSyncContext;
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and notifies listeners only when necessary and only if raisNotificationSwitch argument is true.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="raiseNotificationSwitch">Raises notification if true, does not rais notification if false.</param>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This value is optional and can be provided automatically when invoked from compilers that support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the desired value.</returns>
        public bool SetPropertyRaiseSelectively<T>(ref T storage, T value, bool raiseNotificationSwitch, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            if (raiseNotificationSwitch) RaiseSyncedPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners. This value is optional and can be provided automatically when invoked from compilers that support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the desired value.</returns>
        public virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            RaiseSyncedPropertyChanged(propertyName);

            return true;
        }

        /// <summary>
        /// Raises this object's PropertyChanged event on the UI thread SyncContext (assuming that was the sync context given when instantiating this class).
        /// </summary>
        /// <param name="propertyName">Name of the property used to notify listeners. This
        /// value is optional and can be provided automatically when invoked from compilers
        /// that support <see cref="CallerMemberNameAttribute"/>.</param>
        public void RaiseSyncedPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (_uiSyncContext == null)
            {
                OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
            }
            else
            {
                _uiSyncContext.Post((_) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName)), null);
            }
        }
        
        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="args">The PropertyChangedEventArgs</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
    }
}