using System;
using Windows.System;
using Prism.Unity.Windows;
using Prism.Windows.Mvvm;

namespace Application.ViewModels
{
    public class InfoViewModel : ViewModelBase
    {
        public bool SimulatorAvailable => ((App) PrismUnityApplication.Current).RunAgainstSimulatorInsteadOfMock == false;

        public InfoViewModel()
        {

        }

        public string SimulatorAvailabilityMessage
        {
            get
            {
                if (((App) PrismUnityApplication.Current).IsRunningOnPhysicalCar)
                {
                    return "The code is currently running on the pysical sensor car (IoT) device.\n" +
                           "Application is connected up against real microcontrollers";
                }
                if (((App) PrismUnityApplication.Current).RunAgainstSimulatorInsteadOfMock)
                {
                    return "The application is set up to run against simulator.\n" +
                           "The simulator should have launced automatically. Do not close simulator before this app is closed.\n" +
                           "\n" +
                           "Instead of connecting up against real microcontrollers, it will use simulated data from the simulator.\n" +
                           "You can test run your logic as if you were on the physical car";
                }

                if (((App) PrismUnityApplication.Current).SimulatorAppAvailabilityStatus == LaunchQuerySupportStatus.AppNotInstalled)
                {
                    return "Running against mock data.\n" +
                           "If you wish to be able to actually test your code without the physical car, you can install our simulator.";
                    //TODO: Add instruction on where to find, or button which links to the App in the AppStore
                }

                return $"Code is not running on physical car (IoT device), so tried to look for simulator, but something strange occured.\n" +
                       $"Simulator availability status: {((App) PrismUnityApplication.Current).SimulatorAppAvailabilityStatus}.";
            }
        }
    }
}
