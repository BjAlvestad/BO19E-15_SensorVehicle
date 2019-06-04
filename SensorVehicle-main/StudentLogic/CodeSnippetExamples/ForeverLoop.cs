using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VehicleEquipment.Locomotion.Wheels;

namespace StudentLogic.CodeSnippetExamples
{
    class ForeverLoop : StudentLogicBase
    {

        public ForeverLoop(IWheel wheels) : base(wheels)
        {
            Details = new StudentLogicDescription
            {
                Title = "WHEEL  ",
                Author = "BO19E-15",

                Description = "Simple wheel control using for-loops"
            };
            this._wheels = wheels;
        }
        public override StudentLogicDescription Details { get; }

        private IWheel _wheels;

        private int speedLeft;
        private int speedRight;

        public override void Initialize()
        {
            speedRight = 0;
            speedLeft = 0;
        }


        public override void Run(CancellationToken cancellationToken)
        {
            for (int i = 0; i < 100 && !cancellationToken.IsCancellationRequested; i++)
            {
                _wheels.SetSpeed(speedLeft++, speedRight);
                Thread.Sleep(10);
            }
            for (int i = 0; i < 100 && !cancellationToken.IsCancellationRequested; i++)
            {
                _wheels.SetSpeed(speedLeft--, speedRight);
                Thread.Sleep(10);
            }
            for (int i = 0; i < 100 && !cancellationToken.IsCancellationRequested; i++)
            {
                _wheels.SetSpeed(speedLeft, speedRight--);
                Thread.Sleep(10);
            }
            for (int i = 0; i < 100 && !cancellationToken.IsCancellationRequested; i++)
            {
                _wheels.SetSpeed(speedLeft, speedRight++);
                Thread.Sleep(10);
            }
        }
    }
}
