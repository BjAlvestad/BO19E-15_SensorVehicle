using System;
using VehicleEquipment;
using Xunit;

namespace VehicleEquipment.UnitTests
{
    public class ArrayConverterTests
    {
        [Fact]
        public void ToByteArray_ValidImputData_ValidOutput()
        {
            byte[] result = ArrayConverter.ToByteArray(23, Device.EncoderLeft, MessageCode.NoMessage, int.MaxValue, int.MinValue, 0, +100, -100);

            Assert.Equal((byte)Device.EncoderLeft, result[0]);
            Assert.Equal((byte)MessageCode.NoMessage, result[1]);
            Assert.Equal(5, result[2]);

            // int.MaxValue
            Assert.Equal(0b0111_1111, result[3]);
            Assert.Equal(0b1111_1111, result[4]);
            Assert.Equal(0b1111_1111, result[5]);
            Assert.Equal(0b1111_1111, result[6]);

            // int.MinValue
            Assert.Equal(0b1000_0000, result[7]);
            Assert.Equal(0b0000_0000, result[8]);
            Assert.Equal(0b0000_0000, result[9]);
            Assert.Equal(0b0000_0000, result[10]);

            // 0
            Assert.Equal(0b0000_0000, result[11]);
            Assert.Equal(0b0000_0000, result[12]);
            Assert.Equal(0b0000_0000, result[13]);
            Assert.Equal(0b0000_0000, result[14]);

            // +100
            Assert.Equal(0b0000_0000, result[15]);
            Assert.Equal(0b0000_0000, result[16]);
            Assert.Equal(0b0000_0000, result[17]);
            Assert.Equal(0b0110_0100, result[18]);

            // -100
            Assert.Equal(0b1111_1111, result[19]);
            Assert.Equal(0b1111_1111, result[20]);
            Assert.Equal(0b1111_1111, result[21]);
            Assert.Equal(0b1001_1100, result[22]);
        }

        [Fact]
        public void AssembleDataFromVehicle_ValidData_ValidOutput()
        {
            VehicleDataPacket result = ArrayConverter.AssembleDataFromVehicle(FakeVehicleBytePacket);

            Assert.Equal(Device.EncoderLeft, result.DeviceAddress);
            Assert.Equal(MessageCode.NoMessage, result.Code);
            Assert.Equal(int.MaxValue, result.Integers[0]);
            Assert.Equal(int.MinValue, result.Integers[1]);
            Assert.Equal(0, result.Integers[2]);
            Assert.Equal(100, result.Integers[3]);
            Assert.Equal(-100, result.Integers[4]);
        }

        [Fact]
        public void AssembleDataFromVehicle_RequestMoreIntsThanArraySizeAllows_ThrowIndexOutOfRangeException()
        {
            byte[] incorrectNumberOfIntsArray =
            {
                (byte) Device.EncoderLeft, (byte) MessageCode.NoMessage, 2,
                0b0111_1111, 0b1111_1111, 0b1111_1111, 0b1111_1111, // int.MaxValue
            };

            Assert.Throws<IndexOutOfRangeException>(() => ArrayConverter.AssembleDataFromVehicle(incorrectNumberOfIntsArray));
        }


        private byte[] FakeVehicleBytePacket = new byte[23]
        {
            (byte)Device.EncoderLeft, (byte)MessageCode.NoMessage, 5, 
            0b0111_1111, 0b1111_1111, 0b1111_1111, 0b1111_1111, // int.MaxValue
            0b1000_0000, 0b0000_0000, 0b0000_0000, 0b0000_0000,  // int.MinValue
            0b0000_0000, 0b0000_0000, 0b0000_0000, 0b0000_0000, // 0
            0b0000_0000, 0b0000_0000, 0b0000_0000, 0b0110_0100, // +100
            0b1111_1111, 0b1111_1111, 0b1111_1111, 0b1001_1100
        };
    }
}
