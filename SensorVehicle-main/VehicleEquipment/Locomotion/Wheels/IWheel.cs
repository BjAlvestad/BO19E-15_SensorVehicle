using System.ComponentModel;
using Helpers;

namespace VehicleEquipment.Locomotion.Wheels
{
    public interface IWheel : INotifyPropertyChanged
    {
        /// <summary>
        /// Selects if properties should raise notify property changed (so that they update automatically in GUI).
        /// </summary>
        /// <remarks>
        /// Setting this to true will cause code to run less efficiently, since PropertyChanged-events will be fired at each value update.
        /// </remarks>
        bool RaiseNotificationForSelective { get; set; }

        /// <summary>
        /// Powers the wheels on or off. 
        /// </summary>
        /// <remarks>
        /// Micro-controller may take some seconds to start up.
        /// </remarks>
        bool Power { get; set; }

        /// <summary>
        /// Gets last left wheel power request sent to Wheel micro-controller.
        /// </summary>
        /// <remarks>
        /// This value is NOT collected from the actual micro-controller. It is just the last request that was sent to the micro-controller.<para />
        /// (The actual power on the wheels may be lower. There is a safety function running between the micro-controllers that reduces power when an obstruction is too close.)
        /// </remarks>
        int CurrentSpeedLeft { get; }

        /// <summary>
        /// Gets last right wheel power request sent to Wheel micro-controller.
        /// </summary>
        /// <remarks>
        /// This value is NOT collected from the actual micro-controller. It is just the last request that was sent to the micro-controller.<para />
        /// (The actual power on the wheels may be lower. There is a safety function running between the micro-controllers that reduces power when an obstruction is too close.)
        /// </remarks>
        int CurrentSpeedRight { get; }

        /// <summary>
        /// Sets wheel power with specific values.
        /// </summary>
        /// <param name="leftValue">Power percentage on Left Wheel in range [-100, 100]</param>
        /// <param name="rightValue">Power percentage on Right Wheel in range [-100, 100]</param>
        /// <param name="onlySendIfValuesChanged">
        /// If true (default): new power command will be applied ONLY if it is different than previous. This limits data traffic on communication line.<para />
        /// If false: new power command will ALWAYS be sent.<para />
        /// WARNING: when setting this to false, make sure that the command is not sent too often as it may bog down the communication line.
        /// </param>
        void SetSpeed(int leftValue, int rightValue, bool onlySendIfValuesChanged = true);

        /// <summary>
        /// Sets wheel power to forward.
        /// </summary>
        /// <param name="speed">Speed percentage to set in range [0, 100]</param>
        void Fwd(int speed);

        /// <summary>
        /// Sets wheel power to turn left.
        /// </summary>
        /// <param name="speed">Speed percentage to turn in range [0, 100]</param>
        void TurnLeft(int speed);

        /// <summary>
        /// Sets wheel power to turn right
        /// </summary>
        /// <param name="speed">Speed percentage to turn in range [0, 100]</param>
        void TurnRight(int speed);

        /// <summary>
        /// Sets wheel power to reverse.
        /// </summary>
        /// <param name="speed">Speed percentage to in range [0, 100]</param>
        void Reverse(int speed);

        /// <summary>
        /// <para> Stops wheels (sends command to micro-controller if the previous wheel command was not the same).</para>
        /// To force a new stop signal to be sent regardless, use <see cref="SetSpeed"/> with boolean instead. 
        /// </summary>
        void Stop();

        /// <summary>
        /// Contains error information (and error state).
        /// </summary>
        Error Error { get; }
    }
}
