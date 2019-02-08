using System;
using System.Diagnostics;
using Communication.Vehicle;
using VehicleEquipment;

namespace Communication.MockCommunication
{
    public class MockVehicleCommunication : IVehicleCommunication
    {
        private readonly Device _mockedDevice;
        private string nameOfDevice;

        public MockVehicleCommunication(Device deviceToMock)
        {
            _mockedDevice = deviceToMock;
            nameOfDevice = Enum.GetName(typeof(Device), _mockedDevice);
        }

        public void Write(byte[] data)
        {
            Debug.WriteLine($"Mocked a write of byte array to '{nameOfDevice}'. Array contained the following elements:\n\t{String.Join(',', data)}");
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
                    Debug.WriteLine($"Attempted to read from '{nameOfDevice}', but this has not been implemented in '{nameof(MockVehicleCommunication)}'.");
                    break;
            }

            return mockData;
        }

        private byte[] UltrasonicMockRead()
        {
            byte[] bytes = new byte[20];

            bytes[0] = Convert.ToByte('1');
            bytes[1] = Convert.ToByte('2');
            bytes[2] = Convert.ToByte('3');
            bytes[3] = Convert.ToByte('-');
            bytes[4] = Convert.ToByte('4');
            bytes[5] = Convert.ToByte('5');
            bytes[6] = Convert.ToByte('-');
            bytes[7] = Convert.ToByte('6');
            bytes[8] = Convert.ToByte('7');

            return bytes;
        }

        private byte[] EncoderMockRead()
        {
            byte[] response = new byte[20];

            response[0] = 1;
            response[1] = 34;
            response[2] = 67;
            response[3] = 90;

            return response;
        }
    }
}
