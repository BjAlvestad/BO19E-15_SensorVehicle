using Xunit;
using VehicleEquipment.DistanceMeasurement.Lidar;

namespace VehicleEquipment.UnitTests.DistanceMeasurement.Lidar
{
    public class LidarPacketInterpreterTests
    {
        [Fact]
        public void GetAzimuthAngle_ValidData_Return123point45degrees()
        {
            float azimuthAngle = LidarPacketInterpreter.GetAzimuthAngle(FakeLidarData.FakeDataBlock1);

            Assert.Equal(123.45f, azimuthAngle, 3);
        }

        [Theory]
        [InlineData(VerticalAngle.Down15)]
        [InlineData(VerticalAngle.Down13)]
        [InlineData(VerticalAngle.Down11)]
        [InlineData(VerticalAngle.Down9)]
        [InlineData(VerticalAngle.Down7)]
        [InlineData(VerticalAngle.Down5)]
        [InlineData(VerticalAngle.Down3)]
        [InlineData(VerticalAngle.Down1)]
        [InlineData(VerticalAngle.Up1)]
        [InlineData(VerticalAngle.Up3)]
        [InlineData(VerticalAngle.Up5)]
        [InlineData(VerticalAngle.Up7)]
        [InlineData(VerticalAngle.Up9)]
        [InlineData(VerticalAngle.Up11)]
        [InlineData(VerticalAngle.Up13)]
        [InlineData(VerticalAngle.Up15)]
        public void GetDistance_ValidData_ReturnExpectedValue(VerticalAngle angle)
        {
            float distance = LidarPacketInterpreter.GetDistance(FakeLidarData.FakeDataBlock1, angle);

            Assert.Equal(FakeLidarData.ExpectedValues[angle], distance, 3);
        }
    }
}
