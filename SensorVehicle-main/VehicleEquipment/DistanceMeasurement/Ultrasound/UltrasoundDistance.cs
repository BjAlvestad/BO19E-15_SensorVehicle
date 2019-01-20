using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleEquipment.DistanceMeasurement.Ultrasound
{
    public class UltrasoundDistance : IDistance
    {
        public float Fwd { get; private set; }
        public float Left { get; private set; }
        public float Right { get; private set; }
        public float MinRange { get; private set; }
        public float MaxRange { get; private set; }
        public float Resolution { get; private set; }

        //TODO
    }
}
