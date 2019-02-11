namespace VehicleEquipment.Locomotion.Wheels
{
    public interface IWheel
    {
        int CurrentSpeedLeft { get; }
        int CurrentSpeedRight { get; }

        void SetSpeed(int leftValue, int rightValue);
    }
}
