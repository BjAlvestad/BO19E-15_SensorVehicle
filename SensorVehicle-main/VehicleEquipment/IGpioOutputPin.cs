namespace VehicleEquipment
{
    public interface IGpioOutputPin
    {
        int PinNumber { get; }

        bool SetOutput { get; set; }

        bool ErrorWhenOpeningPin { get; }
    }
}
