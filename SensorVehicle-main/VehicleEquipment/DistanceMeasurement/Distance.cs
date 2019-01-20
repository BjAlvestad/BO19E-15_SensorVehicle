using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;

namespace VehicleEquipment.DistanceMeasurement
{
    public class Distance
    {
        public Distance(params IDistance[] distanceMeasurementsToBlend)
        {

        }
        //TODO
        // Consider having this take IDistance in constructor (use params modifier), and have LidarDistance and UltrasoundDistance (and any future distance measurement systems) implement IDistance interface.
        // Should get tied connected together in the main UWP app.
    }
}
