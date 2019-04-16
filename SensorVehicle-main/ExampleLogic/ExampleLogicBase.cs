using System;
using System.Threading;
using System.Threading.Tasks;
using Helpers;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic
{
    public abstract class ExampleLogicBase : ThreadSafeNotifyPropertyChanged
    {
        private IWheel _wheel;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Contains the information that will be displayed in the GUI. 
        /// It should be initialized, and all its fields set to your desired values in the constructor of the child-class.
        /// </summary>
        public abstract ExampleLogicDetails Details { get; }
        public Error Error { get; }

        protected ExampleLogicBase(IWheel wheel)
        {
            _wheel = wheel;
            Error = new Error();
        }

        public abstract void Initialize();
        public abstract void Run(CancellationToken cancellationToken);

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
                    StartControlLogicTask();
                }
                else
                {
                    _cancellationTokenSource?.Cancel();
                }

                RaiseSyncedPropertyChanged();
            }
        }

        private void StartControlLogicTask()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                if (Error.Unacknowledged)
                {
                    RunExampleLogic = false;
                    return;
                }

                try
                {
                    Initialize();
                    while (RunExampleLogic && !_cancellationTokenSource.IsCancellationRequested)
                    {
                        Run(_cancellationTokenSource.Token);
                    }
                    _wheel.SetSpeed(0, 0);
                }
                catch (Exception e)
                {
                    RunExampleLogic = false;
                    Error.Unacknowledged = true;
                    Error.Message = e.Message;
                    Error.DetailedMessage = e.ToString();
                    _wheel.SetSpeed(0, 0);
                }
            }, _cancellationTokenSource.Token);
        }
    }
}
