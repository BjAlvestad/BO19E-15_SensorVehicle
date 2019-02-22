using System.Threading.Tasks;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic
{
    public abstract class ExampleLogicBase
    {
        private IWheel _wheel;

        public abstract ExampleLogicDetails Details { get; }

        protected ExampleLogicBase(IWheel wheel)
        {
            _wheel = wheel;
        }

        public abstract void Initialize();
        public abstract void Run();

        private bool _runExampleLogic;
        public bool RunExampleLogic
        {
            get { return _runExampleLogic; }
            set
            {
                if (value == _runExampleLogic) return;

                _runExampleLogic = value;
                if (value)
                {
                    Task.Run(() =>
                    {
                        Initialize();
                        while (RunExampleLogic)
                        {
                            Run();
                        }
                        _wheel.SetSpeed(0, 0);
                    });
                }
            }
        }

    }
}
