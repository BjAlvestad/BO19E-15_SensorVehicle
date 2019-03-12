using System;
using System.Collections.Generic;
using System.Diagnostics;
using Communication.Vehicle;
using VehicleEquipment;

namespace Communication.MockCommunication
{
    public class MockVehicleCommunication : IVehicleCommunication
    {
        private readonly Device _mockedDevice;
        private readonly string _nameOfDevice;
        private static Random _random;

        public MockVehicleCommunication(Device deviceToMock)
        {
            _mockedDevice = deviceToMock;
            _nameOfDevice = Enum.GetName(typeof(Device), _mockedDevice);
            _random = new Random();
        }

        public void Write(MessageCode message, params int[] data)
        {
            Debug.WriteLine($"Mocked a write to '{_nameOfDevice}'. Message: {message}. Data (integers): {string.Join(", ", data)}");
        }

        public VehicleDataPacket Read()
        {
            switch (_mockedDevice)
            {
                case Device.Ultrasonic:
                    return UltrasonicMockRead();
                case Device.Encoder:
                    return EncoderMockRead();
                default:
                    Debug.WriteLine($"Attempted to read from '{_nameOfDevice}', but this has not been implemented in '{nameof(MockVehicleCommunication)}'.");
                    break;
            }

            return new VehicleDataPacket();
        }

        private VehicleDataPacket UltrasonicMockRead()
        {
            VehicleDataPacket data = new VehicleDataPacket
            {
                DeviceAddress = _mockedDevice,
                Code = MessageCode.NoMessage,
                Integers = new List<int>
                {
                    _random.Next(0, 400),  // Left distance
                    _random.Next(0, 400),  // FwdRight distance
                    _random.Next(0, 400),  // Right distance
                    _random.Next(0, 400)  // FwdLeft distance
                }
            };

            return data;
        }

        private VehicleDataPacket EncoderMockRead()
        {
            VehicleDataPacket response = new VehicleDataPacket
            {
                DeviceAddress = _mockedDevice,
                Code = MessageCode.NoMessage,
                Integers = new List<int>
                {
                    _random.Next(-500, 500),  // Distance in centimeters
                    _random.Next(500, 5000),  // Time in milliseconds
                }
            };

            return response;
        }
    }
}
