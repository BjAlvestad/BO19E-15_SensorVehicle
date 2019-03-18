using System;
using System.Threading;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic.L1_CenterCorridor
{
    public class CenterCorridorNoStopMain : ExampleLogicBase
    {
        private readonly IWheel _wheels;
        private readonly IUltrasonic _ultrasonic;

        public CenterCorridorNoStopMain(IWheel wheel, IUltrasonic ultrasonic) : base(wheel)
        {
            Details = new ExampleLogicDetails()
            {
                Title = "L1b - Keep Center of Coridor \n\t(rotate on block)",
                Author = "BO19-E15",
                SuitableForSubjects = "Control systems",

                Description = "Simple cross-connected feedback loop between distance sensor and wheel causes the vehicle to steer towards the center of the corridor.\n" +
                              "The left wheel speed is controlled by the distance from the right ultrasound sensor.\n" +
                              "The right wheel speed is controlled by the distance from the left ultrasound sensor.\n" +
                              "Overall speed is affected by distance in front. When meeting opstruction, the car will turn until no obstruction"
            };
            
            _wheels = wheel;
            _ultrasonic = ultrasonic;
        }

        public override ExampleLogicDetails Details { get; }

        public override void Initialize()
        {

        }

        public override void Run()
        {
            if (_ultrasonic.Fwd < 0.5)
            {
               RotateAwayFromObstruction(desiredFrontalClearance: 1.0f, rotationSpeedPercentage: 50);
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
        /// <param name="desiredFrontalClearance">Distance required in front for it to stop rotating (in meters)</param>
        /// <param name="rotationSpeedPercentage">Value between -100 and +100</param>
        private void RotateAwayFromObstruction(float desiredFrontalClearance, int rotationSpeedPercentage)
        {
            DateTime startedRotationAt = DateTime.Now;
            bool rightHasLargestDistance = _ultrasonic.Right > _ultrasonic.Left;
            while (_ultrasonic.Fwd < desiredFrontalClearance)
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
    }
}
