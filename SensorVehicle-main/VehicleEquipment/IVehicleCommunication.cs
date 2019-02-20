namespace VehicleEquipment
{
    public interface IVehicleCommunication
    {
        void Write(MessageCode message, params int[] data);

        VehicleDataPacket Read();
    }
}
