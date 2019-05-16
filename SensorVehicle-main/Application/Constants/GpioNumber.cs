namespace Application
{
    internal static class GpioNumber
    {
        /// <summary>
        /// GPIO number where Lidar Power is connected <para />
        /// Default: Pull LOW
        /// </summary>
        public const int LidarPower = 12;

        /// <summary>
        /// GPIO number where Ultrasound Power is connected <para />
        /// Default: Pull LOW
        /// </summary>
        public const int UltrasoundPower = 16;

        /// <summary>
        /// GPIO number where Wheel Power is connected <para />
        /// Default: Pull LOW
        /// </summary>
        public const int WheelPower = 20;

        /// <summary>
        /// GPIO number where Encoder Power is connected <para />
        /// Default: Pull LOW
        /// </summary>
        public const int EncoderPower = 21;

        /// <summary>
        /// GPIO number where "new data available" signal from Ultrasound is connected <para />
        /// Default: Pull LOW
        /// </summary>
        public const int UltrasoundInterrupt = 19;

        /// <summary>
        /// GPIO number where Power for spare Arduino is connected  <para />
        /// Default: Pull LOW
        /// </summary>
        public const int SpareArduinoPower = 13;

        /// <summary>
        /// GPIO on Raspberry card that is not connected to anything. <para />
        /// Default: Pull HIGH
        /// </summary>
        public const int NotInUse1 = 5;

        /// <summary>
        /// GPIO on Raspberry card that is not connected to anything. <para />
        /// Default: Pull HIGH
        /// </summary>
        public const int NotInUse2 = 6;

        /// <summary>
        /// GPIO on Raspberry card that is not connected to anything. <para />
        /// Default: Pull LOW
        /// </summary>
        public const int NotInUse3 = 26;
    }
}
