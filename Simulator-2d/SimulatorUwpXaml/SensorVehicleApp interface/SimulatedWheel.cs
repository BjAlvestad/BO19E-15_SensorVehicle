using Windows.Foundation.Collections;

namespace SimulatorUwpXaml.SensorVehicleApp_interface
{
    public class SimulatedWheel
    {
        private VehicleSprite _vehicle;

        public SimulatedWheel(VehicleSprite vehicle)
        {
            _vehicle = vehicle;
        }

        public void ExecuteWheelCommand(ValueSet message)
        {
            int[] data = message["DATA"] as int[];

            if (data == null)
            {
                _vehicle.SpeedLeftWheel = 0;
                _vehicle.SpeedRightWheel = 0;
                return;
            }

            _vehicle.SpeedLeftWheel = data[0];
            _vehicle.SpeedRightWheel = data[1];
        }
    }
}
