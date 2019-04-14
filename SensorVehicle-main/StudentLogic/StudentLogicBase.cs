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

        private bool _hasUnacknowledgedError;
        public bool HasUnacknowledgedError
        {
            get { return _hasUnacknowledgedError; }
            set { SetProperty(ref _hasUnacknowledgedError, value); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get { return _errorMessage; }
            private set { SetProperty(ref _errorMessage, value); }
        }

        protected StudentLogicBase(IWheel wheel)
        {
            _wheel = wheel;
        }

        public abstract void Initialize();
        public abstract void Run(CancellationToken cancellationToken);

        public void ClearMessage()
        {
            ErrorMessage = "";
            HasUnacknowledgedError = false;
        }

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
                if (HasUnacknowledgedError)
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
                    HasUnacknowledgedError = true;
                    ErrorMessage = $"CONTROL LOGIC ERROR - '{Details.Title}' generated the following exception:\n{e.Message}";
                    _wheel.SetSpeed(0, 0);
                }
            }, _cancellationTokenSource.Token);
        }
    }
}
