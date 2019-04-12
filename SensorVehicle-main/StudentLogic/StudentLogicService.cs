using System.Collections.ObjectModel;
using Helpers;

using StudentLogic.CodeSnippetExamples;

using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

namespace StudentLogic
{
    public class StudentLogicService : ThreadSafeNotifyPropertyChanged
    {
        public ObservableCollection<StudentLogicBase> StudentLogics { get; set; }

        private StudentLogicBase _activeStudentLogic;
        public StudentLogicBase ActiveStudentLogic
        {
            get { return _activeStudentLogic; }
            set { SetProperty(ref _activeStudentLogic, value); }
        }

        // Any inteface/class registered as a container may be added to the constructor without any further actions
        public StudentLogicService(IWheel wheels, IEncoders encoders, ILidarDistance lidar, IUltrasonic ultrasonic)
        {
            StudentLogics = new ObservableCollection<StudentLogicBase>
            {
                // Child classes instatiated in the StudentLogics collection will automatically appear in the GUI
                // Pass the sensors to be used as arguments (the ones specified in the constructor of the child class).
                new SteerBlindlyToLargestDistance(wheels, lidar)
            };
        }
    }
}
