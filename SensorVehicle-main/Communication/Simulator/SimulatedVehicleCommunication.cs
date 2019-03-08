using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
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

            _simulatorCommunication.OnMessageReceived += SimulatorCommunicationOnOnMessageReceived;
        }

        public void Write(MessageCode message, params int[] data)
        {
            ValueSet valuesToSend = new ValueSet();
            valuesToSend.Add("ADDRESS", (int)_simulatedDevice);
            valuesToSend.Add("MESSAGE", (int)message);
            valuesToSend.Add("NUM_OF_INTS", data.Length);
            valuesToSend.Add("DATA", data);

            _simulatorCommunication.SendMessageAsync(valuesToSend);
        }

        public VehicleDataPacket Read()
        {
            ValueSet valuesToSend = new ValueSet();
            valuesToSend.Add("ADDRESS", (int)_simulatedDevice);
            valuesToSend.Add("REQUEST", "");

            ValueSet dataReceived = Task.Run(() => _simulatorCommunication.RequestDataAsync(valuesToSend)).GetAwaiter().GetResult(); //TODO: This is a temporary hack that blocks the async method. WARNING MAY CAUSE ISSUES!. See if Read() can be rewritten to be async.  See Figure 7 "The Thread Pool Hack" on https://msdn.microsoft.com/en-us/magazine/mt238404.aspx

            return ConvertData(dataReceived);
        }

        private VehicleDataPacket ConvertData(ValueSet data)
        {
            if(data.Count == 0) throw new Exception($"Data received from simulated {_simulatedDevice} was empty. Unable to convert data");

            VehicleDataPacket convertedData = new VehicleDataPacket
            {
                DeviceAddress = (Device) data["ADDRESS"],
                Code = (MessageCode) data["MESSAGE"],
                Integers = new List<int>(data["DATA"] as int[])
            };

            return convertedData;
        }

        private void SimulatorCommunicationOnOnMessageReceived(ValueSet valueSet)
        {
            Debug.WriteLine($"Received from Simulator: \n\tKeys: {string.Join(", ", valueSet.Keys)} \n\tValues: {string.Join(", ", valueSet.Values)} ");
        }
    }
}
