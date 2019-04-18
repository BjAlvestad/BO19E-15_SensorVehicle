using System;
using System.Threading;
using System.Threading.Tasks;
using Helpers;
using VehicleEquipment.Locomotion.Wheels;

namespace StudentLogic
{
    public abstract class StudentLogicBase : ThreadSafeNotifyPropertyChanged
    {
        private IWheel _wheel;
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Contains the information that will be displayed in the GUI. 
        /// It should be initialized, and all its fields set to your desired values in the constructor of the child-class.
        /// </summary>
        public abstract StudentLogicDescription Details { get; }

        public Error Error { get; }

        protected StudentLogicBase(IWheel wheel)
        {
            _wheel = wheel;
            Error = new Error();
        }

        public abstract void Initialize();
        public abstract void Run(CancellationToken cancellationToken);

        private bool _runStudentLogic;
        public bool RunStudentLogic
        {
            get { return _runStudentLogic; }
            set
            {
                if (value == _runStudentLogic) return;

                _runStudentLogic = value;
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
                    RunStudentLogic = false;
                    return;
                }

                try
                {
                    Initialize();
                    while (RunStudentLogic && !_cancellationTokenSource.IsCancellationRequested)
                    {
                        Run(_cancellationTokenSource.Token);
                    }
                    _wheel.SetSpeed(0, 0);
                }
                catch (Exception e)
                {
                    RunStudentLogic = false;
                    Error.Unacknowledged = true;
                    Error.Message = e.Message;
                    Error.DetailedMessage = e.ToString();
                    _wheel.SetSpeed(0, 0);
                }
            }, _cancellationTokenSource.Token);
        }
    }
}
