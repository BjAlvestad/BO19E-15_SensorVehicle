using System;
using System.Diagnostics;
using System.Threading;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic.AutonomyLogics
{
    public class DriveToLargestDistance : ExampleLogicBase
    {
        private IWheel _wheels;
        private ILidarDistance _lidar;
        private IUltrasonic _ultrasonic;

        #region Constructor and Description
        public DriveToLargestDistance(IWheel wheel, ILidarDistance lidar, IUltrasonic ultrasonic) : base(wheel)
        {
            Details = new ExampleLogicDetails
            {
                Title = "Drive to largest distance",
                Author = "BO19E-15",
                DemoType = "AUTONOMY DEMO (simple)",

                Description = "Uses LIDAR to detect largest distance, and drives towards it.\n" +
                              "It also uses Ultrasound to keep distance from wall.\n" +
                              "Will turn away from any obstructions in front.\n\n" +
                              "If lidar and collector is not running, it will be started automatically.\n" +
                              "Please note that this takes several seconds, and therefore it may take some time before the vehicle starts driving."
            };

            _wheels = wheel;
            _lidar = lidar;
            _ultrasonic = ultrasonic;
        }

        public override ExampleLogicDetails Details { get; }
        #endregion

        #region Initialization_RunsOnceWhenControlLogicStarts
        public override void Initialize()
        {
            _ultrasonic.Power = true;
            _lidar.Power = true;
            _lidar.RunCollector = true;
            _lidar.Config.NumberOfCycles = 1;
            _lidar.Config.ActiveVerticalAngles.Add(VerticalAngle.Up1);
            _lidar.Config.DefaultVerticalAngle = VerticalAngle.Up1;
            _lidar.Config.MinRange = 0.5;
        }
        #endregion

        #region ControlLogic 

        private const float clearanceLimitFwd = 0.5f; // Med 0.5 kjører den ikke over tråskådler. Men det gjør den med 0.3. Juster UL sensorer oppover.
        private const float sideClearanceLimitLow = 0.7f;
        private const float sideClearanceLimitLowLow = 0.3f;
        private const int BaseSpeed = 100;
        private int _speedLeft;
        private int _speedRight;
        public override void Run(CancellationToken cancellationToken)
        {
            ThrowExceptionIfLidarCollectorIsStoppedOrSensorError();

            float angleToLargestDistance = _lidar.LargestDistanceInRange(260, 100).Angle;

            if (float.IsNaN(angleToLargestDistance))
            {
                _wheels.Stop();
                Debug.WriteLine("STOPPED due to no LIDAR distance found in range!", "ControlLogic");
                Thread.Sleep(200);
            }
            else
            {
                _speedLeft = BaseSpeed;
                _speedRight = BaseSpeed;

                CompensateSpeedTowardsAngle(angleToLargestDistance);

                CompensateSpeedFromSideClearance();

                if (_ultrasonic.Fwd < clearanceLimitFwd)
                {
                    Debug.WriteLine("Emergency steer Fwd started.", "ControlLogic");
                    EmergencySteerFromObstacleInFront(1.3f, 50, cancellationToken);
                    Debug.WriteLine("Emergency steer Fwd completed.", "ControlLogic");
                }
                else
                {
                    _wheels.SetSpeed(_speedLeft, _speedRight);
                }
            }

            Thread.Sleep(50);
        }

        private void EmergencySteerFromObstacleInFront(float desiredClearance, int rotationPowerPercentage, CancellationToken cancellationToken)
        {
            DateTime startedRotationAt = DateTime.Now;
            bool rightHasLargestDistance = _lidar.LargestDistanceInRange(260, 100).Angle < 180;
            while (_ultrasonic.Fwd < desiredClearance && !cancellationToken.IsCancellationRequested)
            {
                if(rightHasLargestDistance) _wheels.TurnRight(rotationPowerPercentage);
                else _wheels.TurnLeft(rotationPowerPercentage);

                Thread.Sleep(50);

                if (DateTime.Now - startedRotationAt > TimeSpan.FromSeconds(5))
                {
                    Debug.WriteLine($"Inside started rotation. where rotationPowerPrecentage is {rotationPowerPercentage}");
                    if (rotationPowerPercentage < 100)
                    {
                        rotationPowerPercentage += 10;
                        Debug.WriteLine($"Increased power by 10. Is now {rotationPowerPercentage}");
                    }
                    else
                    {
                        _wheels.Stop();
                        Thread.Sleep(5000);
                    }

                    startedRotationAt = DateTime.Now;
                }
            }

            _wheels.Stop();
        }

        private void CompensateSpeedFromSideClearance()
        {
            const float requiredClearanceOnOppositeSide = sideClearanceLimitLowLow + 0.3f;
            int desiredSpeedDifference = (int)((Math.Abs(_ultrasonic.Left - _ultrasonic.Right) / 4) * 100);

            if ((_ultrasonic.Left < sideClearanceLimitLow && _ultrasonic.Right > requiredClearanceOnOppositeSide) || _ultrasonic.Left < sideClearanceLimitLowLow)
            {
                _speedLeft += desiredSpeedDifference;
                if (_speedLeft > 100)
                {
                    _speedRight -= _speedLeft - 100;
                }
            }
            else if((_ultrasonic.Left > requiredClearanceOnOppositeSide && _ultrasonic.Right < sideClearanceLimitLow) || _ultrasonic.Right < sideClearanceLimitLowLow)
            {
                _speedRight += desiredSpeedDifference;
                if (_speedRight > 100)
                {
                    _speedLeft -= _speedRight - 100;
                }
            }
        }

        private void CompensateSpeedTowardsAngle(float angleDeviation)
        {
            int leftSpeedReduction = angleDeviation > 180 ? 360 - (int)angleDeviation : 0;
            int rightSpeedReduction = angleDeviation < 180 ? (int)angleDeviation : 0;

            _speedLeft -= leftSpeedReduction;
            _speedRight -= rightSpeedReduction;
        }

        private void ThrowExceptionIfLidarCollectorIsStoppedOrSensorError()
        {
            string errorMessage = "";

            if (_lidar.RunCollector == false)
            {
                errorMessage += "LIDAR collector stopped unexpectedly!\n" +
                    "Check LIDAR page, and rectify error before starting this control logic again.";
            }
            if (_ultrasonic.Error.Unacknowledged)
            {
                if (errorMessage != "") errorMessage += "\n\n";
                errorMessage += "Ultrasound sensor has an unacknowledged error.\n" +
                                "See Ultrasonic page for details.";
            }
            if (_wheels.Error.Unacknowledged)
            {
                if (errorMessage != "") errorMessage += "\n\n";
                errorMessage += "Wheels has an unacknowledged error.\n" +
                                "See Wheels page for details.";
            }

            if (errorMessage != "")
            {
                throw new Exception(errorMessage);
            }
        }
        #endregion
    }
}
