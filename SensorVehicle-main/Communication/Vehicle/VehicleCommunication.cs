using System;
using System.IO;
using Windows.Devices.I2c;
using VehicleEquipment;

namespace Communication.Vehicle
{
    public class VehicleCommunication : IVehicleCommunication
    {
        public const int DataRequestSize = 23;  // 3bytes + 5ints == 23bytes, i.e. possible to receive up to 5 ints without increasing this number.
        public const int DataTransmitSize = 23;
        private I2cDevice _device;
        private Device _address;

        public VehicleCommunication(Device address)
        {
            SetUpCommunication(address);
            _address = address;
        }

        private async void SetUpCommunication(Device address)
        {
            var settings = new I2cConnectionSettings((byte)address)  // Slave Address of Arduino Uno 
            {
                BusSpeed = I2cBusSpeed.FastMode // this bus has 400Khz speed
            };
            string advancedQuerySyntaxString = I2cDevice.GetDeviceSelector("I2C1"); // This will return Advanced Query String which is used to select i2c device
            var deviceInformationCollection = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(advancedQuerySyntaxString);
            _device = await I2cDevice.FromIdAsync(deviceInformationCollection[0].Id, settings);
        }

        public void Write(MessageCode message, params int[] data)
        {
            byte[] byteArray = ArrayConverter.ToByteArray(DataTransmitSize, _address, message, data);

            _device.Write(byteArray);
        }

        public VehicleDataPacket Read()
        {
            byte[] receivedData = new byte[DataRequestSize];
            _device.Read(receivedData);

            VehicleDataPacket decodedData = ArrayConverter.AssembleDataFromVehicle(receivedData);
            if(decodedData.DeviceAddress != _address) throw new InvalidDataException($"Expected data from {_address}. Addess stamp on datapacket was {decodedData.DeviceAddress}.");

            return decodedData;
        }
    }
}
