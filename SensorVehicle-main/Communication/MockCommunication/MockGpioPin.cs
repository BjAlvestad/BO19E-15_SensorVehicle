using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleEquipment;

namespace Communication.MockCommunication
{
    public class MockGpioPin : IGpioPin
    {
        public MockGpioPin(int gpioNumber)
        {
            PinNumber = gpioNumber;
        }

        public event EventHandler PinValueInputChangedLow;
        public event EventHandler PinValueInputChangedHigh;

        public int PinNumber { get; }
        public bool PinHigh { get; set; }
        public bool ErrorWhenOpeningPin { get; }
    }
}
