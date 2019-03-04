﻿using System;
using System.Threading.Tasks;
using Helpers;
using VehicleEquipment.Locomotion.Wheels;

namespace ExampleLogic
{
    public abstract class ExampleLogicBase : ThreadSafeNotifyPropertyChanged
    {
        private IWheel _wheel;

        /// <summary>
        /// Contains the information that will be displayed in the GUI. 
        /// It should be initialized, and all its fields set to your desired values in the constructor of the child-class.
        /// </summary>
        public abstract ExampleLogicDetails Details { get; }

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

        protected ExampleLogicBase(IWheel wheel)
        {
            _wheel = wheel;
        }

        public abstract void Initialize();
        public abstract void Run();

        public void ClearMessage()
        {
            ErrorMessage = "";
            HasUnacknowledgedError = false;
        }

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
            }
        }

        private void StartControlLogicTask()
        {
            Task.Run(() =>
            {
                if (HasUnacknowledgedError) return;

                try
                {
                    Initialize();
                    while (RunExampleLogic)
                    {
                        Run();
                    }
                    _wheel.SetSpeed(0, 0);
                }
                catch (Exception e)
                {
                    RunExampleLogic = false;
                    HasUnacknowledgedError = true;
                    ErrorMessage = $"CONTROL LOGIC ERROR - '{Details.Title}' generated the following exception:\n{e.Message}";
                    _wheel.SetSpeed(0, 0);
                }
            });
        }
    }
}
