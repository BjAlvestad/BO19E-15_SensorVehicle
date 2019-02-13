namespace VehicleEquipment.Locomotion.Wheels
{
    public interface IWheel
    {
        int CurrentSpeedLeft { get; }
        int CurrentSpeedRight { get; }

        void SetSpeed(int leftValue, int rightValue);

        void Fwd(int speed);
        void TurnLeft(int speed);
        void TurnRight(int speed);
        void Reverse(int speed);
        void Stop();
    }
}
