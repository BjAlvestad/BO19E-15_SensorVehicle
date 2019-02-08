using System;
using Windows.Devices.I2c;
using VehicleEquipment;

namespace Communication.Vehicle
{
    public class VehicleCommunication : IVehicleCommunication
    {
        private I2cDevice _device;

        internal VehicleCommunication(Device address)
        {
            SetUpCommunication(address);
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

        public void Write(byte[] data)
        {
            _device.Write(data);
        }

        public byte[] Read()
        {
            byte[] data = new byte[20];
            _device.Read(data);

            return data;
        }
    }
}
