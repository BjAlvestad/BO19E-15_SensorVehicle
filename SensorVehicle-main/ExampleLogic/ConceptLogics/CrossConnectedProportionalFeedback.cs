using System;
using System.Threading;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic.ConceptLogics
{
    public class CrossConnectedProportionalFeedback : ExampleLogicBase
    {
        private readonly IWheel _wheels;
        private readonly IUltrasonic _ultrasonic;

        public CrossConnectedProportionalFeedback(IWheel wheel, IUltrasonic ultrasonic) : base(wheel)
        {
            Details = new ExampleLogicDetails()
            {
                Title = "Cross-connected P-feedback",
                Author = "BO19-E15",
                DemoType = "P-feedback (limitation) demo",

                Description = "Simple cross-connected feedback loop between distance sensor and wheel causes the vehicle to steer away from the closest wall.\n" +
                              "The left wheel speed is controlled by the distance from the right ultrasound sensor, and vice versa for other side.\n" +
                              "When meeting obstruction in front, the car will turn (after a short pause) until no obstruction.\n" +
                              "Demo suggestion:\n" +
                              "Observe how just a simple proportional feedback kan give some limited control of the vehicle.\n" +
                              "Try sending the car at an steep angle towards the wall, and observe how it oscillates, and is unable to stay in the middle of the corridor."
            };
            
            _wheels = wheel;
            _ultrasonic = ultrasonic;
        }

        public override ExampleLogicDetails Details { get; }

        public override void Initialize()
        {

        }

        public override void Run(CancellationToken cancellationToken)
        {
            ThrowExceptionOnSensorError();

            if (_ultrasonic.Fwd < 0.5)
            {
                _wheels.Stop();
                Thread.Sleep(500);
               RotateAwayFromObstruction(cancellationToken, desiredFrontalClearance: 1.0f, rotationSpeedPercentage: 50);
            }
            else
            {
                float reductionRate = 1.0f;
                KeepCenterCorridor(reductionRate);
            }
        }

        // reduction rate should be a decimal number (1 is full speed)
        private void KeepCenterCorridor(float argumentSpeedScaler)
        {
            float distanceLeft = _ultrasonic.Left;
            float distanceRight = _ultrasonic.Right;

            float relativeSpeedScaler = 100 / Math.Max(distanceLeft, distanceRight);

            int leftSpeed = (int)(distanceRight * relativeSpeedScaler * argumentSpeedScaler);
            int rightSpeed = (int)(distanceLeft * relativeSpeedScaler * argumentSpeedScaler);

            _wheels.SetSpeed(leftSpeed, rightSpeed);

            Thread.Sleep(10);
        }

        /// <summary>
        /// Rotates away from obstruction
        /// </summary>
        /// <param name="cancellationToken">Used to exit loop if control logic stop is requested</param>
        /// <param name="desiredFrontalClearance">Distance required in front for it to stop rotating (in meters)</param>
        /// <param name="rotationSpeedPercentage">Value between -100 and +100</param>
        private void RotateAwayFromObstruction(CancellationToken cancellationToken, float desiredFrontalClearance, int rotationSpeedPercentage)
        {
            DateTime startedRotationAt = DateTime.Now;
            bool rightHasLargestDistance = _ultrasonic.Right > _ultrasonic.Left;
            while (_ultrasonic.Fwd < desiredFrontalClearance && !cancellationToken.IsCancellationRequested && !UnacknowledgedSensorError())
            {
                if(rightHasLargestDistance) _wheels.TurnRight(rotationSpeedPercentage);
                else _wheels.TurnLeft(rotationSpeedPercentage);

                Thread.Sleep(50);

                if (startedRotationAt - DateTime.Now > TimeSpan.FromSeconds(5))
                {
                    _wheels.Stop();
                    Thread.Sleep(2000);
                }
            }

            _wheels.Stop();
        }

        private void ThrowExceptionOnSensorError()
        {
            if (_ultrasonic.Error.Unacknowledged)
            {
                throw new Exception(
                    "The Ultrasonic sensor has unacknowledged error!\n" +
                    "Control logic stopped as a safety precaution.\n" +
                    "Check ULTRASONIC page, and rectify error before starting this control logic again.");
            }
        }

        private bool UnacknowledgedSensorError()
        {
            return _ultrasonic.Error.Unacknowledged;
        }
    }
}
