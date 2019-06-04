using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

namespace StudentLogic.CodeSnippetExamples
{
    class WallAndBack : StudentLogicBase
    {
        public WallAndBack(IWheel wheel, IEncoders encoders, IUltrasonic ultrasonic) : base(wheel)
        {
            Details = new StudentLogicDescription
            {
                Title = "Ultrasonic and Encoder  ",
                Author = "BO19E-15",

                Description = "Simple control logic using encoder and ultrasonic" +
                              "\n Drive to a wall, turn around and drive back"
            };
            this._wheel = wheel;
            this._encoders = encoders;
            this._ultrasonic = ultrasonic;
        }
        public override StudentLogicDescription Details { get; }

        private IWheel _wheel;
        private IUltrasonic _ultrasonic;
        private IEncoders _encoders;

        private double distanceTraveled;
        private bool forwad;

        public override void Initialize()
        {
            _encoders.CollectAndResetDistanceFromEncoders();
            _encoders.ResetTotalDistanceTraveled();
            _encoders.CollectContinously = true;
            _encoders.CollectionInterval = 200;
            distanceTraveled = 0;
            forwad = true;

            _wheel.SetSpeed(80, 80);
        }


        public override void Run(CancellationToken cancellationToken)
        {
            if (forwad && _ultrasonic.Fwd < 0.50)
            {
                distanceTraveled = _encoders.Left.TotalDistanceTravelled;

                _wheel.SetSpeed(100, -100);
                Thread.Sleep(1550);

                forwad = false;

                _encoders.ResetTotalDistanceTraveled();

                _wheel.SetSpeed(80, 80);
            }

            if (!forwad && _encoders.Left.TotalDistanceTravelled > distanceTraveled)
            {
                _wheel.SetSpeed(100, -100);
                Thread.Sleep(1550);

                base.RunStudentLogic = false;
            }
            Thread.Sleep(50);
        }
    }
}
