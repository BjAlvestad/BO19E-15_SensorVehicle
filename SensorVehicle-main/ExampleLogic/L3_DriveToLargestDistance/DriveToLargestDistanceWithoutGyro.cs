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
    public class DriveToLargestDistanceWithoutGyro : ExampleLogicBase
    {
        private IWheel _wheels;
        private ILidarDistance _lidar;

        public DriveToLargestDistanceWithoutGyro(IWheel wheel, ILidarDistance lidar) : base(wheel)
        {
            Details = new ExampleLogicDetails
            {
                Title = "L3 - Drive to largest distance",
                Author = "BO19-E15",
                SuitableForSubjects = "Robotics",

                Description = "Uses LIDAR to detect largest distance, and drives towards it.\n" +
                              "This control logic does NOT utilize any gyro."
            };

            _wheels = wheel;
            _lidar = lidar;
        }

        public override ExampleLogicDetails Details { get; }

        public override void Initialize()
        {
            _lidar.RunCollector = true;
            _lidar.NumberOfCycles = 1;  // Code in Initialize seems to not take effect
        }

        public override void Run(CancellationToken cancellationToken)
        {
            const int rotateLimit = 60;
            if (_lidar.LargestDistance.Angle > rotateLimit && _lidar.LargestDistance.Angle < 360 - rotateLimit) TurnTowardsLargestDistance(cancellationToken);
            else SteerTowardsLargestDistance(100);

            Thread.Sleep(50);
        }

        private void SteerTowardsLargestDistance(int baseSpeed)
        {
            float angleDeviation = _lidar.LargestDistance.Angle;

            int leftSpeedReduction = angleDeviation > 180 ? 360 - (int)angleDeviation : 0;
            int rightSpeedReduction = angleDeviation < 180 ? (int)angleDeviation : 0;

            _wheels.SetSpeed(baseSpeed - leftSpeedReduction, baseSpeed - rightSpeedReduction);
        }

        private void TurnTowardsLargestDistance(CancellationToken cancellationToken)
        {
            const float errorMargin = 5;
            DateTime lastPause = DateTime.Now;
            DateTime lastCommand = DateTime.Now;

            while (!IsPointingTowardsLargestDistance(errorMargin) && !cancellationToken.IsCancellationRequested)
            {
                float angle = _lidar.LargestDistance.Angle;
                bool gettingCloseToTarget = angle < 5 * errorMargin || angle > (360 - angle) * 5;

                //if (gettingCloseToTarget && (_lidar.LastUpdate - lastCommand) < TimeSpan.FromMilliseconds(600))
                //{
                //    lastPause = DateTime.Now;
                //    _wheels.Stop();
                //    Thread.Sleep(300);
                //}
                if (_lidar.LastUpdate > lastCommand)
                {
                    RotateTowardsLargestDistance(angle, gettingCloseToTarget ? 30 : 100);
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
            if (angle < 180)
            {
                 _wheels.TurnRight(turnSpeed);
            }
            else
            {
                _wheels.TurnLeft(turnSpeed);
            }
        }
    }
}
