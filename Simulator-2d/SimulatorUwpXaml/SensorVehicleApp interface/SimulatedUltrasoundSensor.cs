using Windows.Foundation.Collections;

namespace SimulatorUwpXaml.SensorVehicleApp_interface
{
    public class SimulatedUltrasoundSensor
    {
        private const int NumberOfUltrasoundSensors = 3;
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

            distanceData[0] = (int)(_distances.Left * 100);
            distanceData[1] = (int)(_distances.Fwd * 100);
            distanceData[2] = (int)(_distances.Right * 100);

            return distanceData;
        }
    }
}
