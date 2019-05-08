using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Helpers;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    public interface ILidarDistance : INotifyPropertyChanged
    {
        /// <summary>
        /// Selects if properties should raise notify property changed (so that they update automatically in GUI).
        /// </summary>
        /// <remarks>
        /// Setting this to true will cause code to run less efficiently, since PropertyChanged-events will be fired at each value update.
        /// </remarks>
        bool RaiseNotificationForSelective { get; set; }

        /// <summary>
        /// Power On/Off for Lidar. <para />
        /// Note that there is a 5 second startup delay after the Lidar receives power. 
        /// </summary>
        bool Power { get; set; }

        /// <summary>
        /// Contains error information (and error state).
        /// </summary>
        Error Error { get; }

        /// <summary>
        /// Start/Stop collecting Lidar Data. This must be <see langword="true"/> for the lidar distances to update.
        /// </summary>
        bool RunCollector { get; set; }

        //TODO: Refactor into LidarDistanceConfiguration class
        double MinRange { get; set; }
        double MaxRange { get; set; }

        //TODO: Consider integrating into RunCollector
        void StartCollector();
        void StopCollector();

        /// <summary>
        /// Gets the point with largest distance (in the currently selected <see cref="DefaultVerticalAngle"/>)
        /// </summary>
        HorizontalPoint LargestDistance { get; }

        /// <summary>
        /// Gets the distance towards the front (spanning a default opening angle). <para />
        /// Internaly it uses <see cref="GetDistance"/>, with from/to angle with <see cref="DefaultHalfBeamOpening"/> as -/+ offset from 0 degrees, and using <see cref="DefaultVerticalAngle"/>, <see cref="DefaultCalculationType"/>. 
        /// </summary>
        float Fwd { get; }

        /// <summary>
        /// Gets the distance towards the left (spanning a default opening angle). <para />
        /// Internaly it uses <see cref="GetDistance"/>, with from/to angle with <see cref="DefaultHalfBeamOpening"/> as -/+ offset from 270 degrees, and using <see cref="DefaultVerticalAngle"/>, <see cref="DefaultCalculationType"/>. 
        /// </summary>
        float Left { get; }

        /// <summary>
        /// Gets the distance towards the right (spanning a default opening angle). <para />
        /// Internaly it uses <see cref="GetDistance"/>, with from/to angle with <see cref="DefaultHalfBeamOpening"/> as -/+ offset from 90 degrees, and using <see cref="DefaultVerticalAngle"/>, <see cref="DefaultCalculationType"/>. 
        /// </summary>
        float Right { get; }

        /// <summary>
        /// Gets the distance towards the aft (spanning a default opening angle). <para />
        /// Internaly it uses <see cref="GetDistance"/>, with from/to angle with <see cref="DefaultHalfBeamOpening"/> as -/+ offset from 180 degrees, and using <see cref="DefaultVerticalAngle"/>, <see cref="DefaultCalculationType"/>. 
        /// </summary>
        float Aft { get; }

        //TODO: Refactor into LidarDistanceConfiguration class
        int DefaultHalfBeamOpening { get; set; }
        CalculationType DefaultCalculationType { get; set; }
        VerticalAngle DefaultVerticalAngle { get; set; }
        ExclusiveSynchronizedObservableCollection<VerticalAngle> ActiveVerticalAngles { get; }
        int NumberOfCycles { get; set; }

        /// <summary>
        /// Collection of interpreted directions from Lidar.  <para />
        /// Normally you would not need to interact with this collection directly (use the other provided methods instead - which performs calculations / extracts data from this collection for you). 
        /// However the collection has been made available so that you extract data from it yourself, in ways that we have not provided methods for (e.g. to get closest distance). <para />
        /// </summary>
        ReadOnlyDictionary<VerticalAngle, List<HorizontalPoint>> Distances { get; }

        /// <summary>
        /// The time when the last lidar distance update completed. (Or the time collector was started, if no collection has completed yet).
        /// </summary>
        DateTime LastUpdate { get; }

        /// <summary>
        /// The duration elapsed while collecting data packets (in milliseconds) - for last distance update.
        /// </summary>
        long LastCollectionDuration { get; }

        /// <summary>
        /// The duration elapsed while interpreting data packets (in milliseconds) - for last distance update.
        /// </summary>
        long LastDataInterpretationDuration { get; }

        /// <summary>
        /// Gets the largest distance in the selected range. (The selected range is allowed to span zero). <para />
        /// Note: Only the distances for the currently selected <see cref="DefaultVerticalAngle"/> are evaluated
        /// </summary>
        /// <param name="fromAngle">Left/from angle [0, 360)</param>
        /// <param name="toAngle">Right/to angle (0, 360]</param>
        /// <returns>Horizontal point with largest distance (contains both the distance and angle)</returns>
        HorizontalPoint LargestDistanceInRange(float fromAngle = 260, float toAngle = 100);

        /// <summary>
        /// Gets distance from a range based on specified CalculationType (e.g. smallest distance in range) <para />
        /// To get a list of all distances in the range see <seealso cref="GetDistancesInRange"/>
        /// </summary>
        /// <param name="fromAngle">From angle (horizontal)</param>
        /// <param name="toAngle">To angle (horizontal)</param>
        /// <param name="verticalAngle">The vertical to evaluate distances for</param>
        /// <param name="calculationType">Type of distance to be selected (e.g. smallest, largest, average etc.)</param>
        /// <returns>Distance in range based on selected calculation type</returns>
        float GetDistance(float fromAngle, float toAngle, VerticalAngle verticalAngle, CalculationType calculationType);

        /// <summary>
        /// Get a list of all distances in specified range (for the specified vertical angle) <para />
        /// To also get the angles in addtition to the distances, see <seealso cref="GetHorizontalPointsInRange"/>
        /// </summary>
        /// <param name="fromAngle">From angle (horizontal)</param>
        /// <param name="toAngle">To angle (horizontal)</param>
        /// <param name="verticalAngle">The vertical to evaluate distances for</param>
        /// <returns>A list of all distances in the specified range (for the specified vertical angle)</returns>
        List<float> GetDistancesInRange(float fromAngle, float toAngle, VerticalAngle verticalAngle);

        /// <summary>
        /// Get a list of all horizontal points (distance and horizontal angle) in specified range (for the specified vertical angle) <para />
        /// To get a list of only the distances, use <see cref="GetDistancesInRange(float, float, VerticalAngle)"/> instead.
        /// </summary>
        /// <param name="fromAngle">From angle (horizontal)</param>
        /// <param name="toAngle">To angle (horizontal)</param>
        /// <param name="verticalAngle">The vertical to evaluate distances for</param>
        /// <returns>A list of all horizontal points (distance and horizontal angle) in the specified range (for the specified vertical angle)</returns>
        List<HorizontalPoint> GetHorizontalPointsInRange(float fromAngle, float toAngle, VerticalAngle verticalAngle);
    }
}
