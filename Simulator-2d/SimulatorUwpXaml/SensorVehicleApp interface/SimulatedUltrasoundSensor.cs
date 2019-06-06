using System;
using Windows.Foundation.Collections;

namespace SimulatorUwpXaml.SensorVehicleApp_interface
{
    public class SimulatedUltrasoundSensor
    {
        private const int NumberOfUltrasoundSensors = 4;
        private const float MaximumDistance = 4.00f;
        private const float TimeOutDistance = 4.01f;
        private readonly float _sensorOffsetFwd;
        private readonly float _sensorOffsetSide;
        private Lidar _distances;

        public SimulatedUltrasoundSensor(Lidar distances, float sensorOffsetFwd, float sensorOffsetSide)
        {
            _distances = distances;
            _sensorOffsetFwd = sensorOffsetFwd;
            _sensorOffsetSide = sensorOffsetSide;
        }

        public ValueSet ReturnData()
        {
            ValueSet data = new ValueSet();

            data.Add("ADDRESS", (int)Device.Ultrasonic);
            data.Add("MESSAGE", 0);
            data.Add("NUMBER_OF_INTS", NumberOfUltrasoundSensors);

            data.Add("DATA", DistanceData());

            return data;
        }

        private int[] DistanceData()
        {
            int[] distanceData = new int[NumberOfUltrasoundSensors];

            int distanceLeftInCm = GetUltrasoundDistanceFromLidarData(270, _sensorOffsetFwd);
            int distanceFwdInCm = GetUltrasoundDistanceFromLidarData(0, _sensorOffsetSide);
            int distanceRightInCm = GetUltrasoundDistanceFromLidarData(90, _sensorOffsetSide);

            distanceData[0] = Math.Max(distanceLeftInCm, 0);
            distanceData[1] = Math.Max(distanceFwdInCm, 0);  // FwdRight
            distanceData[2] = Math.Max(distanceRightInCm, 0);
            distanceData[3] = Math.Max(distanceFwdInCm, 0);  // FwdLeft

            return distanceData;
        }

        private int GetUltrasoundDistanceFromLidarData(int mountingAngle, float sensorOffset)
        {
            const int closeRangeHalfBeam = 15;
            const float closeRangeMaxDistance = 1.00f;
            const int longRangeHalfBeam = 5;

            float closeRangeDistance = _distances.GetSmallestDistanceInRange(mountingAngle - closeRangeHalfBeam, mountingAngle + closeRangeHalfBeam);
            float longRangeDistance = _distances.GetSmallestDistanceInRange(mountingAngle - longRangeHalfBeam, mountingAngle + longRangeHalfBeam);

            if (float.IsNaN(closeRangeDistance)) closeRangeDistance = TimeOutDistance;
            if (float.IsNaN(longRangeDistance)) longRangeDistance = TimeOutDistance;

            float shortestDistanceInCm = (closeRangeDistance > closeRangeMaxDistance) ? longRangeDistance : Math.Min(closeRangeDistance, longRangeDistance) - sensorOffset;

            return (int)(Math.Min(shortestDistanceInCm, MaximumDistance) * 100);
        }
    }
}
