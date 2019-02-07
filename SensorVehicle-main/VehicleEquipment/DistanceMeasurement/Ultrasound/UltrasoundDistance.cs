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
        private readonly float _left;
        private readonly float _right;
        private readonly float _fwd;

        public float GetFwd()
        {
            return _fwd;
        }

        public float GetLeft()
        {
            return _left;
        }

        public float GetRight()
        {
            return _right;
        }

        public float MinRange { get; private set; }
        public float MaxRange { get; private set; }
        public float Resolution { get; private set; }

        //TODO
    }
}
