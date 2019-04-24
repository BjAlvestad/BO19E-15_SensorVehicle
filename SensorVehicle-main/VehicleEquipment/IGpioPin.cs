using System;

namespace VehicleEquipment
{
    public interface IGpioPin
    {
        event EventHandler PinValueInputChangedLow;
        event EventHandler PinValueInputChangedHigh;

        int PinNumber { get; }

        bool PinHigh { get; set; }

        bool ErrorWhenOpeningPin { get; }
    }
}
