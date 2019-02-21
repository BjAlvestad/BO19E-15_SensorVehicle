using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ExampleLogic.L1_CenterCorridor;
using ExampleLogic.L2_RightHandSearch;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic
{
    public class ExampleLogicService
    {
        public ObservableCollection<ExampleLogicBase> ExampleLogics { get; set; }
        public ExampleLogicBase ActiveExampleLogic { get; set; }

        public ExampleLogicService(IWheel wheels, IUltrasonic ultrasonic)
        {
            ExampleLogics = new ObservableCollection<ExampleLogicBase>
            {
                new CenterCorridorMain(wheels, ultrasonic),
                new RightHandSearchMain(wheels, ultrasonic)
            };
        }
    }
}
