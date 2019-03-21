using System;
using Windows.Foundation.Collections;

namespace SimulatorUwpXaml.SensorVehicleApp_interface
{
    public class SimulatedUltrasoundSensor
    {
        private const int NumberOfUltrasoundSensors = 4;
        private const int MaximumDistanceInCm = 400;
        private const int TimeOutDistanceInCm = 401;
        private Lidar _distances;

        public SimulatedUltrasoundSensor(Lidar distances)
        {
            _distances = distances;
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

            int distanceLeftInCm = float.IsNaN(_distances.Left) ? TimeOutDistanceInCm : Math.Min((int)(_distances.Left * 100), MaximumDistanceInCm);
            int distanceFwdInCm = float.IsNaN(_distances.Fwd) ? TimeOutDistanceInCm : Math.Min((int)(_distances.Fwd * 100), MaximumDistanceInCm);
            int distanceRightInCm = float.IsNaN(_distances.Right) ? TimeOutDistanceInCm : Math.Min((int)(_distances.Right * 100), MaximumDistanceInCm);

            distanceData[0] = distanceLeftInCm;
            distanceData[1] = Math.Min(distanceFwdInCm, MaximumDistanceInCm);  // FwdRight
            distanceData[2] = distanceRightInCm;
            distanceData[3] = distanceFwdInCm;  // FwdLeft

            return distanceData;
        }
    }
}
