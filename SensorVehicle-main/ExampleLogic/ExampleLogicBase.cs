﻿using System.Threading.Tasks;

namespace ExampleLogic
{
    public abstract class ExampleLogicBase
    {
        public abstract ExampleLogicDetails Details { get; }

        public abstract void Initialize();
        public abstract void Run();

        //BUG: Navigating away from side leaves thread running, and resets slider (put slider back to normal, and an additional thread will start up).
        //Also it should not be possible to navigate to other logic pages while a logic is set to running (but should still be possible to navigate to sensor pages).
        //Look into adding storage on the ViewModel to remember button position, and also see if navigation can be blocked there.
        private bool _runExampleLogic;
        public bool RunExampleLogic
        {
            get { return _runExampleLogic; }
            set
            {
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
                    });
                }
            }
        }

    }
}
