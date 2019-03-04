using System.Collections.ObjectModel;
using ExampleLogic.L1_CenterCorridor;
using ExampleLogic.L2_RightHandSearch;
using Helpers;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic
{
    public class ExampleLogicService : ThreadSafeNotifyPropertyChanged
    {
        public ObservableCollection<ExampleLogicBase> ExampleLogics { get; set; }

        private ExampleLogicBase _activeExampleLogic;
        public ExampleLogicBase ActiveExampleLogic
        {
            get { return _activeExampleLogic; }
            set { SetProperty(ref _activeExampleLogic, value); }
        }

        // Any inteface/class registered as a container may be added to the constructor without any further actions
        public ExampleLogicService(IWheel wheels, IEncoder encoder, ILidarDistance lidar, IUltrasonic ultrasonic)
        {
            ExampleLogics = new ObservableCollection<ExampleLogicBase>
            {
                // Child classes instatiated in the ExampleLogics collection will automatically appear in the GUI
                // Pass the sensors to be used as arguments (the ones specified in the constructor of the child class).
                new CenterCorridorMain(wheels, ultrasonic),
                new CenterCorridorNoStopMain(wheels, ultrasonic),
                new RightHandSearchMain(wheels, ultrasonic)
            };
        }
    }
}
