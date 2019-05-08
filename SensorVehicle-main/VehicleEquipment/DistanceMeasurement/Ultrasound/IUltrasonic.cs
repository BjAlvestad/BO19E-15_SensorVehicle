using System;
using System.ComponentModel;
using Helpers;

namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    public interface IUltrasonic : INotifyPropertyChanged
    {
        /// <summary>
        /// Allow/Block ultrasonic controller from communicating with the main program via I2c. <para />
        /// Note that this will not power the micro-controller off, since it is required for anti-collision protection, which is running internally between the wheel and encoder micro-controller.
        /// </summary>
        bool DeisolateI2cCommunciation { get; set; }

        /// <summary>
        /// Get distance to the Left (in meters). <para />
        /// Returns the distance from local storage, unless new data is available on micro-controller. In which case it first gets new data for all directions and stores them localy, before returning the value from local storage.
        /// </summary>
        float Left { get; }

        /// <summary>
        /// Get smallest of the distances to the front from <see cref="FwdLeft"/> and <see cref="FwdRight"/> (in meters). <para />
        /// </summary>
        float Fwd { get; }

        /// <summary>
        /// Get distance to the front (from the fwd sensor mounted to the left of center-line). <para />
        /// Returns the distance from local storage, unless new data is available on micro-controller. In which case it first gets new data for all directions and stores them localy, before returning the value from local storage.
        /// </summary>
        float FwdLeft { get; }

        /// <summary>
        /// Get distance to the front (from the fwd sensor mounted to the right of center-line). <para />
        /// Returns the distance from local storage, unless new data is available on micro-controller. In which case it first gets new data for all directions and stores them localy, before returning the value from local storage.
        /// </summary>
        float FwdRight { get; }

        /// <summary>
        /// Get distance to the right (in meters). <para />
        /// Returns the distance from local storage, unless new data is available on micro-controller. In which case it first gets new data for all directions and stores them localy, before returning the value from local storage.
        /// </summary>
        float Right { get; }

        /// <summary>
        /// The time when last collection of data from micro-controller occured.
        /// </summary>
        DateTime TimeStamp { get; }

        //TODO: Implement alternative to collect based on time interval insted of interrupt. Or remove this property.
        int PermissableDistanceAge { get; set; }

        /// <summary>
        /// Selects if distance and time properties should raise notify property changed (so that they update automatically in GUI).
        /// </summary>
        /// <remarks>
        /// Setting this to true will cause code to run less efficiently, since PropertyChanged-events will be fired at each value update.
        /// </remarks>
        bool RaiseNotificationForSelective { get; set; }

        /// <summary>
        /// Contains error information (and error state).
        /// </summary>
        Error Error { get; }

        /// <summary>
        /// When set to true: new distance data is collected from micro-controller as soon as new data is available (notified via pin-interupt). <para />
        /// When set to false: new distance data is collected only when requested (e.g. by calling <see cref="Fwd"/>) - but still only if new data is available.
        /// </summary>
        bool RefreshUltrasonicContinously { get; set; }
    }
}
