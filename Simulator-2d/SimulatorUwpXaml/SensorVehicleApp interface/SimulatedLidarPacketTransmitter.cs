using Windows.Foundation.Collections;

namespace SimulatorUwpXaml.SensorVehicleApp_interface
{
    public class SimulatedLidarPacketTransmitter
    {
        private Lidar _distances;

        public SimulatedLidarPacketTransmitter(Lidar distances)
        {
            _distances = distances;
        }

        public ValueSet ReturnData()
        {
            ValueSet data = new ValueSet();

            data.Add("ADDRESS", 0);
            data.Add("MESSAGE", 0);

            data.Add("DATA", _distances.DistanceReadings.ToArray());

            return data;
        }
    }
}
