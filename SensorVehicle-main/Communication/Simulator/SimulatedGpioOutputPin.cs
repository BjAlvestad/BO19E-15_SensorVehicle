using System;
using VehicleEquipment;

namespace Communication.Simulator
{
    public class SimulatedGpioOutputPin : IGpioOutputPin
    {
        public SimulatedGpioOutputPin(int gpioNumber)
        {
            try
            {
                PinNumber = gpioNumber;
                
                ErrorWhenOpeningPin = false;
            }
            catch (Exception e)
            {
                ErrorWhenOpeningPin = true;
                throw new Exception($"An error when trying to open GPIO-pin {gpioNumber}:\n{e.Message}\n\nDetails: \n{e}\n****************\n\n)");
            }
        }

        public int PinNumber { get; }
        public bool SetOutput { get; set; }
        public bool ErrorWhenOpeningPin { get; }
    }
}
