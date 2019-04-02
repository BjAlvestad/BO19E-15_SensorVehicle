using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic.L3_DriveToLargestDistance
{
    public class TurnToLargestDistance : ExampleLogicBase
    {
        private IWheel _wheels;
        private ILidarDistance _lidar;

        public TurnToLargestDistance(IWheel wheel, ILidarDistance lidar) : base(wheel)
        {
            Details = new ExampleLogicDetails
            {
                Title = "L3a - Turns to the greatest distance",
                Author = "BO19-E15",
                SuitableForSubjects = "Simple code usage demo",

                Description = "Uses LIDAR to detect largest distance, and turns towards it." +
                              "You can use this as inspiration for part of your own control logic." +
                              "If you use a Gyro, you can simplify this code."
            };

            _wheels = wheel;
            _lidar = lidar;
        }

        public override ExampleLogicDetails Details { get; }

        public override void Initialize()
        {
            _lidar.RunCollector = true;
            _lidar.NumberOfCycles = 1;  // Code in Initialize seems to not take effect
            _lidar.ActiveVerticalAngles.Add(VerticalAngle.Up1);
            _lidar.DefaultVerticalAngle = VerticalAngle.Up1;
            _lidar.MinRange = 0.5;
            _lidar.DefaultCalculationType = CalculationType.Min;
        }

        public override void Run(CancellationToken cancellationToken)
        {
            const int rotateLimit = 60;

            float angleToLargestDistance = _lidar.LargestDistance.Angle;

            if (float.IsNaN(angleToLargestDistance))  //TODO: Make LIDAR class throw exception when calling methods (or when accessing Distance) when power is off, or collector not running.
            {
                _wheels.Stop();
                Debug.WriteLine("STOPPED due to no LIDAR distance found!\nIs LIDAR powered on, and collector running?", "ControlLogic");
                Thread.Sleep(2000);
            }
            else if (angleToLargestDistance > rotateLimit && angleToLargestDistance < 360 - rotateLimit)
            {
                TurnTowardsLargestDistance(cancellationToken);
            }

            Thread.Sleep(50);
        }

        private void TurnTowardsLargestDistance(CancellationToken cancellationToken)
        {
            const float errorMargin = 5;
            DateTime lastCommand = DateTime.Now;

            while (!IsPointingTowardsLargestDistance(errorMargin) && !cancellationToken.IsCancellationRequested)
            {
                float angle = _lidar.LargestDistance.Angle;
                bool gettingCloseToTarget = angle < 5 * errorMargin || angle > (360 - angle) * 5;

                if (_lidar.LastUpdate > lastCommand)
                {
                    RotateTowardsLargestDistance(angle, gettingCloseToTarget ? 40 : 100); // power 30 is not enough to turn on 3 battery lines - getting close to 2 (is ok on full battery / fresh 3 lines)
                    lastCommand = DateTime.Now;
                }
            }

            _wheels.Stop();
        }

        private bool IsPointingTowardsLargestDistance(float acceptableErrorMargin)
        {
            float angle = _lidar.LargestDistance.Angle;
            if (angle > acceptableErrorMargin) return false;  //Returns false if angle is it too large.
            if (angle > 360 - acceptableErrorMargin) return false;

            return true;
        }

        private void RotateTowardsLargestDistance(float angle, int turnSpeed)
        {
            if (angle < 180) _wheels.TurnRight(turnSpeed);
            else _wheels.TurnLeft(turnSpeed);
        }
    }
}
