using System;
using System.Diagnostics;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;

namespace SimulatorUwpXaml.SensorVehicleApp_interface
{
    // Based on https://docs.microsoft.com/en-us/windows/uwp/launch-resume/convert-app-service-in-process
    public class SimulatorAppServiceProvider
    {
        private AppServiceConnection _simulatorAppServiceConnection;
        private BackgroundTaskDeferral _simulatorAppServiceDeferral;

        public void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            IBackgroundTaskInstance taskInstance = args.TaskInstance;
            AppServiceTriggerDetails appService = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            _simulatorAppServiceDeferral = taskInstance.GetDeferral();
            taskInstance.Canceled += OnAppServicesCanceled;
            _simulatorAppServiceConnection = appService.AppServiceConnection;
            _simulatorAppServiceConnection.RequestReceived += OnAppServiceRequestReceived;
            _simulatorAppServiceConnection.ServiceClosed += AppServiceConnection_ServiceClosed;

            //TODO: Finn en m[te [ sende fra SimulatedVehicleCommunication klassen (Mottak er mulighens ikke noe problem da vi kan subscribe en metode fra her)
            //_simulatorAppServiceConnection.SendMessageAsync(new ValueSet {{"Melding", "Hvordan faa sendt direkte fra min klasse?"}});
        }

        public async void SendToSimulator(Device device, object dataToSend)
        {
            await _simulatorAppServiceConnection.SendMessageAsync(new ValueSet {{device.ToString(), dataToSend}});
        }

        public void ReadFromSimulator(Device device)
        {
            _simulatorAppServiceConnection.SendMessageAsync(new ValueSet {{"REQUEST", device}});
        } 

        private async void OnAppServiceRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            AppServiceDeferral messageDeferral = args.GetDeferral();
            ValueSet message = args.Request.Message;
            Debug.WriteLine($"OnAppServiceRequestReceived:  {string.Join(", ", message.Values)}");
            //string text = message["Lidar"] as string;
            //Device device = (Device)message["Address"];
            //Debug.WriteLine($"OnAppServiceRequestReceived:  {text}");
            //if ("Value" == text)
            //{
            //    ValueSet returnMessage = new ValueSet {{"Response", "True"}};
            //    await args.Request.SendResponseAsync(returnMessage);
            //}
            messageDeferral.Complete();
        }

        private void OnAppServicesCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _simulatorAppServiceDeferral.Complete();
        }

        private void AppServiceConnection_ServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            _simulatorAppServiceDeferral.Complete();
        }
    }
}
