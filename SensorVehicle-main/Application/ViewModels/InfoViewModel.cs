using System;
using Windows.System;
using Application.Helpers;
using Prism.Unity.Windows;
using Prism.Windows.Mvvm;

namespace Application.ViewModels
{
    public class InfoViewModel : ViewModelBase
    {
        public RunningState ProgramRunningState => ((App) PrismUnityApplication.Current).ProgramRunningState;

        public InfoViewModel()
        {

        }

        public string SimulatorAvailabilityMessage
        {
            get
            {
                switch (ProgramRunningState)
                {
                    case RunningState.AgainstMockData:
                        return "Running against mock data.\n" +
                               "If you wish to be able to actually test your code without the physical car, you can install our simulator.\n" +
                               "Simulator can be found at the GitHub repository for BO19-15 Sensor Vehicle\n\n" +
                               $"Simulator availability status: {((App) PrismUnityApplication.Current).SimulatorAppAvailabilityStatus}.";
                    case RunningState.AgainstSimulator:
                        return "The application is set up to run against simulator.\n" +
                               "The simulator should have launched automatically. Do not close simulator before this app is closed.\n" +
                               "\n" +
                               "Instead of connecting up against real micro-controllers, it will use simulated data from the simulator.\n" +
                               "You can test run your logic as if you were on the physical car";
                    case RunningState.OnPhysicalCar:
                        return "The code is currently running on the physical sensor car (IoT) device.\n" +
                               "Application is connected up against real micro-controllers";
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        // Launches https://www.microsoft.com/en-us/p/hvl-sensorvehicle-simulator/9nbs6gn8sqlg
        public void OpenSimulatorDownloadPage()
        {
            Uri simulatorProductDetailsPageUri = new Uri("ms-windows-store://pdp/?ProductId=9nbs6gn8sqlg");

            var success = Launcher.LaunchUriAsync(simulatorProductDetailsPageUri);
        }
    }
}
