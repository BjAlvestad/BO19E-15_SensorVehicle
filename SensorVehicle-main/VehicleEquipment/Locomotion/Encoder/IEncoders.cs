using System;
using System.ComponentModel;
using Helpers;

namespace VehicleEquipment.Locomotion.Encoder
{
    public interface IEncoders : INotifyPropertyChanged
    {
        /// <summary>
        /// Selects if distance and time properties should raise notify property changed (so that they update automatically in GUI).
        /// </summary>
        /// <remarks>
        /// Setting this to true will cause code to run less efficiently, since PropertyChanged-events will be fired at each value update.
        /// </remarks>
        bool RaiseNotificationForSelective { get; set; }

        /// <summary>
        /// Power On/Off for encoder micro-controllers
        /// </summary>
        /// <remarks>
        /// Micro-controllers may take some seconds to start up.
        /// </remarks>
        bool Power { get; set; }

        /// <summary>
        /// Accumulated error state and message for Left and Right encoder. <para />
        /// State gets set set after each call to <see cref="CollectAndResetDistanceFromEncoders"/> (manual or automatic).
        /// </summary>
        Error Error { get; }

        /// <summary>
        /// Data from / state for Left Encoder
        /// </summary>
        Encoder Left { get; }

        /// <summary>
        /// Data from / state for Right Encoder
        /// </summary>
        Encoder Right { get; }

        /// <summary>
        /// The interval at whitch <see cref="CollectAndResetDistanceFromEncoders"/> gets called when <see cref="CollectContinously"/> is set to true.
        /// </summary>
        int CollectionInterval { get; set; }

        /// <summary>
        /// When set to true, <see cref="CollectAndResetDistanceFromEncoders"/> will be called at a fixed time interval specified in <see cref="CollectionInterval"/>
        /// </summary>
        bool CollectContinously { get; set; }

        /// <summary>
        /// Resets the total distance traveled from Left and Right encoders. <para />
        /// This will not affect the distance accumulated on the micro-controller since last collect using <see cref="CollectAndResetDistanceFromEncoders"/>.
        /// </summary>
        void ResetTotalDistanceTraveled();

        /// <summary>
        /// Collects distance accumulated on micro-controller since last request, and adds it to total distance traveled. <para />
        /// If there is an unacnowledged error in <see cref="Error"/> the method will simply return, after switching off automatic collection, if it was running.
        /// </summary>
        void CollectAndResetDistanceFromEncoders();

        /// <summary>
        /// Clears any active errors for the encoders.
        /// </summary>
        void ClearAllEncoderErrors();
    }
}
