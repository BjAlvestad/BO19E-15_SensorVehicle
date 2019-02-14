using System;
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
            throw new NotImplementedException();
        }

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
