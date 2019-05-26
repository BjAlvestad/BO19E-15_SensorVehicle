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
                               "If you wish to be able to actually test your code without the physical car, you can install our simulator.\n\n" +
                               $"Simulator availability status: {((App) PrismUnityApplication.Current).SimulatorAppAvailabilityStatus}.";
                    case RunningState.AgainstSimulator:
                        return "The application has automatically configured itself to communicate with the simulator (by use of 'AppService').\n" +
                               "The simulator should have launched automatically.\n" +
                               "\n" +
                               "Instead of connecting up against real micro-controllers, it will use simulated data from the simulator app.\n" +
                               "You can test-run your logic as if you were on the physical car.\n" +
                               "\n" +
                               "You can also manually manipulate the position/orientation of the car, and zoom-level of the map in the simulator:\n" +
                               "\t- Left mouse button:\t Move car.\n" +
                               "\t- Right mouse button:\t Rotate car.\n" +
                               "\t- PageUp/PageDown:\t Zoom in/out\n" +
                               "\t- Home:\t\t\t Reset zoom to default level.\n";
                    case RunningState.OnPhysicalCar:
                        return "The code is currently running on the physical Sensor Vehicle (an IoT) device.\n" +
                               "The application has automatically configured itself to communicate with real micro-controllers via I2c.";
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
