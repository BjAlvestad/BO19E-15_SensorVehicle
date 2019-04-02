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
    public class SteerBlindlyToLargestDistance : ExampleLogicBase
    {
        private IWheel _wheels;
        private ILidarDistance _lidar;

        #region Description
        public SteerBlindlyToLargestDistance(IWheel wheel, ILidarDistance lidar) : base(wheel)
        {
            Details = new ExampleLogicDetails
            {
                Title = "L3b - Steer to greatest distance",
                Author = "BO19-E15",
                SuitableForSubjects = "Simple code usage demo",

                Description = "Uses LIDAR to detect largest distance, and steers towards it (without gyro).\n" +
                              "NB: " +
                              "Does not check for obstructions on side (or in front)." +
                              "Will steer blindly towards greatest distance"
            };

            _wheels = wheel;
            _lidar = lidar;
        }

        public override ExampleLogicDetails Details { get; }
        #endregion

        #region Initialization_RunsOnceWhenControlLogicStarts
        public override void Initialize()
        {
            _lidar.RunCollector = true;
            _lidar.NumberOfCycles = 1;  // Code in Initialize seems to not take effect
            _lidar.ActiveVerticalAngles.Add(VerticalAngle.Up1);
            _lidar.DefaultVerticalAngle = VerticalAngle.Up1;
            _lidar.MinRange = 0.5;
            _lidar.DefaultCalculationType = CalculationType.Min;
        }
        #endregion

        #region ControlLogic

        public override void Run(CancellationToken cancellationToken)
        {
            float angleToLargestDistance = _lidar.LargestDistanceInRange(260, 100).Angle;

            if (float.IsNaN(angleToLargestDistance))
            {
                _wheels.Stop();
                Debug.WriteLine("STOPPED due to no LIDAR distance found in range!", "ControlLogic");
                Thread.Sleep(2000);
            }
            else
            {
                SteerTowardsAngle(angleToLargestDistance, 100);
            }

            Thread.Sleep(50);
        }

        private void SteerTowardsAngle(float angleDeviation, int baseSpeed)
        {
            int leftSpeedReduction = angleDeviation > 180 ? 360 - (int)angleDeviation : 0;
            int rightSpeedReduction = angleDeviation < 180 ? (int)angleDeviation : 0;

            _wheels.SetSpeed(baseSpeed - leftSpeedReduction, baseSpeed - rightSpeedReduction);
        }

        #endregion
    }
}
