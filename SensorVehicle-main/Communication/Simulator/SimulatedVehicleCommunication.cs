using System;
using VehicleEquipment;

namespace Communication.Simulator
{
    public class SimulatedVehicleCommunication : IVehicleCommunication
    {
        private readonly Device _simulatedDevice;
        private readonly string _nameOfDevice;
        private readonly SimulatorAppServiceClient _simulatorCommunication;

        public SimulatedVehicleCommunication(Device deviceToSimulate, SimulatorAppServiceClient simulatorCommunication)
        {
            _simulatorCommunication = simulatorCommunication;
            _simulatedDevice = deviceToSimulate;
            _nameOfDevice = Enum.GetName(typeof(Device), _simulatedDevice);
        }

        public void Write(MessageCode message, params int[] data)
        {
            throw new NotImplementedException();
        }

        public VehicleDataPacket Read()
        {
            throw new NotImplementedException();
        }
    }
}
