namespace VehicleEquipment
{
    public interface IGpioPin
    {
        int PinNumber { get; }

        bool PinHigh { get; set; }

        bool ErrorWhenOpeningPin { get; }
    }
}
