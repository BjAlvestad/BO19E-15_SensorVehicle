using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SimulatorUwpXaml
{
    public class Lidar
    {
        private readonly SpriteClass _mountingPlatform;

        public List<float> DistanceReadings { get; private set; }
        public DateTime DistanceReadingAge { get; private set; }

        public Lidar(SpriteClass mountingPlatform)
        {
            _mountingPlatform = mountingPlatform;
            DistanceReadings = new List<float>();
            for (int i = 0; i < 360; i++)
            {
                DistanceReadings.Add(float.NaN);
            }
        }

        public float Fwd
        {
            get
            {
                float min = DistanceReadings.GetRange(0, 15).Min();
                float min2 = DistanceReadings.GetRange(DistanceReadings.Count - 15, 15).Min();
                return Math.Min(min, min2);
            }
        }
        public float Right => DistanceReadings.GetRange(75, 30).Min();
        public float Aft => DistanceReadings.GetRange(165, 30).Min();
        public float Left => DistanceReadings.GetRange(255, 30).Min();

        public void Update360(List<BoundingBox> boundaries)
        {
            DistanceReadings = GetLidarReading(boundaries);
            DistanceReadingAge = DateTime.Now;
        }

        private List<float> GetLidarReading(List<BoundingBox> boundaries)
        {
            List<float> lidarReadings = new List<float>();
            Vector3 lidarPosition3D = new Vector3(_mountingPlatform.Position, 0);
            Ray lidarRay = new Ray {Position = lidarPosition3D};
            for (int i = 0; i < 360; i++)
            {
                lidarRay.Direction = VectorDirection(i);

                float? distanceToClosestWall = null;
                foreach (BoundingBox boundary in boundaries)
                {
                    float? distanceToWall = lidarRay.Intersects(boundary);
                    if ((distanceToWall < distanceToClosestWall || distanceToClosestWall == null) && distanceToWall != null)
                    {
                        distanceToClosestWall = distanceToWall;
                    }
                }
                lidarReadings.Add((distanceToClosestWall/100) ?? float.NaN);
            }

            return lidarReadings;
        }

        private Vector3 VectorDirection(int lidarAngleDegrees)
        {
            Vector3 direction = new Vector3();
            double angle = lidarAngleDegrees * Math.PI / 180;
            angle += _mountingPlatform.Angle;

            direction.X = (float)Math.Cos(angle);
            direction.Y = (float) Math.Sin(angle);
            direction.Z = 0;
            return direction;
        }
    }
}
