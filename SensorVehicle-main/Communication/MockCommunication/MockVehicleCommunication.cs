using System;
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

        public void Write(byte[] data)
        {
            Debug.WriteLine($"Mocked a write of byte array to '{_nameOfDevice}'. Array contained the following elements:\n\t{String.Join(", ", data)}");
        }

        public byte[] Read()
        {
            byte[] mockData = new byte[20];
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

            return mockData;
        }

        private byte[] UltrasonicMockRead()
        {
            byte[] bytes = new byte[20];

            bytes[0] = Convert.ToByte('0' + _random.Next(0, 4));
            bytes[1] = Convert.ToByte('0' + _random.Next(0, 10));
            bytes[2] = Convert.ToByte('1');
            bytes[3] = Convert.ToByte('-');
            bytes[4] = Convert.ToByte('0' + _random.Next(0, 4));
            bytes[5] = Convert.ToByte('0' + _random.Next(0, 10));
            bytes[6] = Convert.ToByte('2');
            bytes[7] = Convert.ToByte('-');
            bytes[8] = Convert.ToByte('0' + _random.Next(0, 4));
            bytes[9] = Convert.ToByte('0' + _random.Next(0, 10));
            bytes[10] = Convert.ToByte('3');

            return bytes;
        }

        private byte[] EncoderMockRead()
        {
            byte[] response = new byte[20];

            int distance = _random.Next(-500, 500);
            int time = _random.Next(500, 5000);

            response = BreakIntsIntoByteArray(0x30, distance, time);

            return response;
        }

        private byte[] BreakIntsIntoByteArray(byte addressByte, params int[] integersToBreakDown)
        {
            byte[] bytes = new byte[integersToBreakDown.Length*sizeof(Int32) + 1];

            bytes[0] = addressByte;
            for (int i = 0; i < integersToBreakDown.Length; i++)
            {
                bytes[4 * i + 1] = (byte) (integersToBreakDown[i] >> 24);
                bytes[4 * i + 2] = (byte) (integersToBreakDown[i] >> 16);
                bytes[4 * i + 3] = (byte) (integersToBreakDown[i] >> 8);
                bytes[4 * i + 4] = (byte) (integersToBreakDown[i]);
            }

            return bytes;
        }
    }
}
