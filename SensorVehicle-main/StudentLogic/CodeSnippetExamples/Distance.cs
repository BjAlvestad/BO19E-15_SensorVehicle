using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

namespace StudentLogic.CodeSnippetExamples
{
    class Distance : StudentLogicBase
    {
        public Distance(IWheel wheels, IEncoders encoders) : base(wheels)
        {
            Details = new StudentLogicDescription
            {
                Title = "Encoder  ",
                Author = "BO19E-15",

                Description = "Simple encoder control using if-statement"
            };
            this._wheels = wheels;
            this._encoders = encoders;
        }
        public override StudentLogicDescription Details { get; }

        private IWheel _wheels;
        private IEncoders _encoders;

        public override void Initialize()
        {
            _encoders.CollectAndResetDistanceFromEncoders();
            _encoders.ResetTotalDistanceTraveled();

            _wheels.SetSpeed(70, 70);
        }

        public override void Run(CancellationToken cancellationToken)
        {
            Thread.Sleep(50);
            _encoders.CollectAndResetDistanceFromEncoders();
            if (_wheels.CurrentSpeedRight > 0)
            {
                if (_encoders.Left.TotalDistanceTravelled > 200)
                {
                    _wheels.SetSpeed(-70, -70);
                }
            }
            else
            {
                if (_encoders.Left.TotalDistanceTravelled < 0)
                {
                    _wheels.SetSpeed(70, 70);
                }
            }
        }
    }
}
