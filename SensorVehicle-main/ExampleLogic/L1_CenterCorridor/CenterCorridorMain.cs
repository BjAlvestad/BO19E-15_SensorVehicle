using System;
using System.Diagnostics;
using System.Threading;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic.L1_CenterCorridor
{
    public class CenterCorridorMain : ExampleLogicBase
    {
        private readonly IWheel _wheels;
        private readonly IUltrasonic _ultrasonic;

        public override ExampleLogicDetails Details { get; }

        public CenterCorridorMain(IWheel wheels, IUltrasonic ultrasonic)
        {
            Details = new ExampleLogicDetails
            {
                Title = "L1 - Keep Center of Coridor",
                Author = "BO19-E15",
                SuitableForSubjects = "Control systems",

                Description = "Simple cross-connected feedback loop between distance sensor and wheel causes the vehicle to keep to the center of the corridor.\n" +
                              "The left wheel speed is controlled by the distance from the right ultrasound sensor.\n" +
                              "The right wheel speed is controlled by the distance from the left ultrasound sensor."
            };

            _wheels = wheels;
            _ultrasonic = ultrasonic;
        }


        public override void Initialize()
        {
            Debug.WriteLine($"Ran Initialize() method in {Details.Title}");
        }

        public override void Run()
        {
            float distanceLeft = _ultrasonic.Left;
            float distanceRight = _ultrasonic.Right;

            float speedScaler = 100 / Math.Max(distanceLeft, distanceRight);

            int leftSpeed = (int)(distanceRight * speedScaler);
            int rightSpeed = (int)(distanceLeft * speedScaler);

            _wheels.SetSpeed(leftSpeed, rightSpeed);

            Debug.WriteLine($"DISTANCE:  Left={distanceLeft} Right={distanceRight},  SPEED: Left={leftSpeed} Right={rightSpeed}");
            Thread.Sleep(50);

            float distanceFwd = _ultrasonic.Fwd;
            if (distanceFwd < 0.5 && !float.IsNaN(distanceFwd))
            {
                int sleepTime = 20000;
                Debug.WriteLine($"Distance fwd is <0.5m ({distanceFwd}m), sleeping for {sleepTime} milliseconds.");
                Thread.Sleep(sleepTime);
            }
        }
    }
}
