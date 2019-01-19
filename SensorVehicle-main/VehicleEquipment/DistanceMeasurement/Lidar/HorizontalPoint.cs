using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VehicleEquipment.DistanceMeasurement.Lidar
{
    // Consider to set as readonly so that it can efficiently be passed as refrence via the new in parameter.
    // The readonly modifier was introduced in C#7.2 and ensures the struct is immutable. Currently this project is set to C#7.0, and thus it can't be used.
    // Source: https://docs.microsoft.com/en-us/dotnet/csharp/write-safe-efficient-code
    public class HorizontalPoint : IComparable
    {
        public float Angle { get; }
        public float Distance { get; }

        public HorizontalPoint(float angle, float distance)
        {
            Angle = angle;
            Distance = distance;
        }

        /// <summary>
        /// <para>Returns +1 if this instances angle is larger than obj angle.</para>
        /// <para>Returns -1 if obj angle is larger than this instances angle.</para>
        /// <para>(Returns 0 if they are the same).</para>
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            HorizontalPoint other = (HorizontalPoint) obj;

            return Math.Sign(other.Angle - Angle);
        }
    }
}
