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
    public class MockGpioOutputPin : IGpioOutputPin
    {
        public MockGpioOutputPin(int gpioNumber)
        {
            PinNumber = gpioNumber;
        }

        public int PinNumber { get; }
        public bool SetOutput { get; set; }
        public bool ErrorWhenOpeningPin { get; }
    }
}
