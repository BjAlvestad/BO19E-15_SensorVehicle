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

        private SimulatedWheel _wheel;
        private SimulatedEncoderSensor _encoderLeft;
        private SimulatedEncoderSensor _encoderRight;
        private SimulatedUltrasoundSensor _ultrasound;
        private SimulatedLidarPacketTransmitter _lidar;

        public void InstantiateSimulatedEquipment(VehicleSprite vehicle, Lidar distances)
        {
            _wheel = new SimulatedWheel(vehicle);
            _encoderLeft = new SimulatedEncoderSensor(vehicle, leftWeelNotRight: true);
            _encoderRight = new SimulatedEncoderSensor(vehicle, leftWeelNotRight: false);

            float ultrasoundOffsetFwd = vehicle.Texture.Width * vehicle.Scale / 2 / 100;
            float ultrasoundOffsetSide = vehicle.Texture.Height * vehicle.Scale / 2 / 100;
            _ultrasound = new SimulatedUltrasoundSensor(distances, ultrasoundOffsetFwd, ultrasoundOffsetSide);

            _lidar = new SimulatedLidarPacketTransmitter(distances);
        }

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

            if (message.ContainsKey("LIDAR"))
            {
                await args.Request.SendResponseAsync(_lidar.ReturnData());
                messageDeferral.Complete();
                return;
            }

            Device receivedFromAddress = (Device) message["ADDRESS"];
            switch (receivedFromAddress)
            {
                case Device.Wheel:
                    _wheel.ExecuteWheelCommand(message);
                    break;
                case Device.Ultrasonic:
                    if (message.ContainsKey("REQUEST")) await args.Request.SendResponseAsync(_ultrasound.ReturnData());
                        break;
                case Device.EncoderLeft:
                    if (message.ContainsKey("REQUEST")) await args.Request.SendResponseAsync(_encoderLeft.ReturnData());
                    break;
                case Device.EncoderRight:
                    if (message.ContainsKey("REQUEST")) await args.Request.SendResponseAsync(_encoderRight.ReturnData());
                    break;
                case Device.GyroAccelerometer:
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Received request from invalid device address {receivedFromAddress}");
            }

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
