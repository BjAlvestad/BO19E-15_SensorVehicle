using System;
using System.Diagnostics;
using System.Threading;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.Locomotion.Wheels;

namespace StudentLogic.CodeSnippetExamples
{
    public class SteerBlindlyToLargestDistance : StudentLogicBase
    {
        private IWheel _wheels;
        private ILidarDistance _lidar;

        #region Description
        public SteerBlindlyToLargestDistance(IWheel wheel, ILidarDistance lidar) : base(wheel)
        {
            Details = new StudentLogicDescription
            {
                Title = "LIDAR - Steer to greatest distance",
                Author = "BO19E-15",

                Description = "Uses LIDAR to detect largest distance in front sector, and steers towards it (without gyro).\n" +
                              "NB: " +
                              "Does not check for obstructions on side (or in front)." +
                              "Will steer blindly towards greatest distance"
            };

            _wheels = wheel;
            _lidar = lidar;
        }

        public override StudentLogicDescription Details { get; }
        #endregion

        #region Initialization_RunsOnceWhenControlLogicStarts
        public override void Initialize()
        {
            _lidar.Power = true;
            _lidar.RunCollector = true;
            _lidar.Config.NumberOfCycles = 1;  // Code in Initialize seems to not take effect
            _lidar.Config.ActiveVerticalAngles.Add(VerticalAngle.Up1);
            _lidar.Config.DefaultVerticalAngle = VerticalAngle.Up1;
            _lidar.Config.MinRange = 0.5;
            _lidar.Config.DefaultCalculationType = CalculationType.Min;
        }
        #endregion

        #region ControlLogic

        public override void Run(CancellationToken cancellationToken)
        {
            ThrowExceptionIfCollectorIsStopped();

            float angleToLargestDistance = _lidar.LargestDistanceInRange(260, 100).Angle;

            if (float.IsNaN(angleToLargestDistance))
            {
                _wheels.Stop();
                Debug.WriteLine("STOPPED WHEELS due to no LIDAR distance found in range!", "ControlLogic");
                Thread.Sleep(200);
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

        private void ThrowExceptionIfCollectorIsStopped()
        {
            if (_lidar.RunCollector == false)
            {
                throw new Exception(
                    "LIDAR collector stopped unexpectedly!\n" +
                    "Check LIDAR page, and rectify error before starting this control logic again.");
            }
        }
    }
}
