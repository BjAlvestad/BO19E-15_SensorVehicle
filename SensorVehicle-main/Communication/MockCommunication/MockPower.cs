using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Communication.MockCommunication
{
    public class MockPower : IPower
    {
        public bool Lidar { get; set; }
        public bool Ultrasound { get; set; }
        public bool Wheels { get; set; }
        public bool Encoder { get; set; }
        public bool Spare1 { get; set; }
        public bool Spare2 { get; set; }
        public bool Spare3 { get; set; }
    }
}
