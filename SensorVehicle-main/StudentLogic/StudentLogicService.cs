using System.Collections.ObjectModel;
using System.Linq;
using Helpers;

using StudentLogic.CodeSnippetExamples;
using StudentLogic.Student1;
using StudentLogic.Student2;
using StudentLogic.Student3;

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
               
                new Student1A(wheels, encoders, lidar, ultrasonic),
                new Student1B(wheels, encoders, lidar, ultrasonic),
                new Student2A(wheels, encoders, lidar, ultrasonic),
                new Student2B(wheels, encoders, lidar, ultrasonic),
                new Student3A(wheels, encoders, lidar, ultrasonic),
                new Student3B(wheels, encoders, lidar, ultrasonic),
                new SteerBlindlyToLargestDistance(wheels, lidar),
                new ForeverLoop(wheels),
                new Distance(wheels, encoders),
                new WallAndBack(wheels, encoders, ultrasonic)
            };

            ActiveStudentLogic = StudentLogics.FirstOrDefault();
        }
    }
}
