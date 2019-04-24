using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic.ConceptLogics
{
    public class TurnToLargestDistance : ExampleLogicBase
    {
        private IWheel _wheels;
        private ILidarDistance _lidar;

        public TurnToLargestDistance(IWheel wheel, ILidarDistance lidar) : base(wheel)
        {
            Details = new ExampleLogicDetails
            {
                Title = "Turn towards greatest distance",
                Author = "BO19-E15",
                DemoType = "Sensor update time demo",

                Description = "Uses LIDAR to detect largest distance, and turns towards it.\n" +
                              "This code does NOT utilize any gyro.\n" +
                              "Demo suggestion:\n" +
                              "Try changing collection cycles on the LIDAR page while control logic is running, and observe how this affects the vehicles ability to point towards the correct heading."
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
            ThrowExceptionIfCollectorIsStopped();

            const int rotateLimit = 60;

            float angleToLargestDistance = _lidar.LargestDistance.Angle;

            if (float.IsNaN(angleToLargestDistance))  //TODO: Make LIDAR class throw exception when calling methods (or when accessing Distance) when power is off, or collector not running.
            {
                _wheels.Stop();
                Debug.WriteLine("STOPPED WHEELS due to no LIDAR distance found!", "ControlLogic");
                Thread.Sleep(200);
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

            while (!IsPointingTowardsLargestDistance(errorMargin) && !cancellationToken.IsCancellationRequested && _lidar.RunCollector)
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
