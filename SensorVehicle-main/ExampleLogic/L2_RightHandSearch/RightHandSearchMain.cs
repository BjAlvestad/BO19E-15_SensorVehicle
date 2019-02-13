using System;
using System.Diagnostics;
using System.Threading;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic.L2_RightHandSearch
{
    public class RightHandSearchMain : IExampleLogic
    {
        private readonly IWheel _wheels;
        private readonly IUltrasonic _ultrasonic;

        public ExampleLogicDetails Details { get; }

        public RightHandSearchMain(IWheel wheels, IUltrasonic ultrasonic)
        {
            Details = new ExampleLogicDetails
            {
                Title = "L2 - Right hand search",
                Author = "BO19-E15",
                SuitableForSubjects = "Industrial IT\nRobotics",

                Description = "At current time it is only used for testing.\n" +
                              "Will print to debug when Initialize method is run, and will keep prining and sleep every time Run is called."
            };

            
            _wheels = wheels;
            _ultrasonic = ultrasonic;
        }

        private int i = 0;
        public void Initialize()
        {
            Debug.WriteLine($"Ran Initialize() method in {Details.Title}");
        }

        public void Run()
        {
            ++i;
            Debug.WriteLine($"Run() method in {Details.Title} is on iteration no. {i}. Sleeping for {i} milli seconds");
            Thread.Sleep(i);
        }
    }
}
