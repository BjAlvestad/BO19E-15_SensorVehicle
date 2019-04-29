using SensorVehicle_extras.Configuration;
using SensorVehicle_extras.Connection;
using SensorVehicle_extras.Devices;
using SensorVehicle_extras.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SensorVehicle_extras
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ConnectionInfo _connInfo;
        private Camera _camera;
        private HttpServer _httpServer;
        private Timer _periodicTimer; //TODO: Check if this is redundant
        private ExtendedExecutionSession _session = null;

        public ConnectionInfo ConnInfo { get => _connInfo; private set => _connInfo = value; }
        public Camera Camera { get => _camera; private set => _camera = value; }
        public HttpServer HttpServer { get => _httpServer; private set => _httpServer = value; }

        public MainPage()
        {
            this.InitializeComponent();
            ConnInfo = new ConnectionInfo();
        }

       
        private async void BtnStart_Toggled(object sender, RoutedEventArgs e)
        {
            bool success = await Run();
            if (success)
            {
                if (!ConnInfo.IsStreaming)
                {
                    Camera.Start();
                    BtnStart.IsOn = true;
                    ConnInfo.IsStreaming = true;
                    ConnInfo.CameraMessage = $"Streaming to http://{ConnInfo.IPAddr}";
                }
                else
                {
                    ConnInfo.CameraMessage = "Stopping the camera...";
                    await Camera.Stop();
                    ConnInfo.IsStreaming = false;
                    BtnStart.IsOn = false;
                    ConnInfo.CameraMessage = "Webcam is off";
                }
            }
            else if (BtnStart.IsOn)
            {
                BtnStart.IsOn = false;
                ConnInfo.IsStreaming = false;
                ConnInfo.CameraMessage = "No camera found";
                DisplayNoCamFound();
            }
            else if (!BtnStart.IsOn)
            {
                ConnInfo.CameraMessage = "No camera found";
            }
        }
        private void BtnLaunchApp_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DisplayLaunchDialog();            
        }

        private async Task<bool> Run()
        {
            if (Camera == null)
            {
                Camera = new Camera();
            }

            VideoSetting videoSetting = new VideoSetting
            {
                VideoResolution = VideoResolution.SD640_480,
                VideoSubtype = VideoSubtype.NV12,
                VideoQuality = 0.2,
                UsedThreads = 1
            };
            if (HttpServer == null)
            {
                HttpServer = new HttpServer(Camera);
                HttpServer.Start();
            }
            if (BtnStart.IsOn)
            {
                ConnInfo.CameraMessage = "Searching for webcam...";
            }
            
            bool success = await Camera.Initialize(videoSetting);
            return success;
        }

        private async void DisplayExitAppDialog()
        {
            ContentDialog exitAppDialog = new ContentDialog
            {
                Title = "Are you sure you want to exit?",
                Content = "This app will be exited and SensorVehicle Main App will be launced",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await exitAppDialog.ShowAsync();

            // Delete the file if the user clicked the primary button.
            /// Otherwise, do nothing.
            if (result == ContentDialogResult.Primary)
            {
                CoreApplication.Exit();
            }
        }
        private async void DisplayRestartSysDialog()
        {
            ContentDialog restartSysDialog = new ContentDialog
            {
                Title = "Are you sure you want to restart the system?",
                Content = "The system will restart within 5 seconds",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await restartSysDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                //TODO: Remove these comments
                // Restarts the device within 5 seconds:
                //ShutdownManager.BeginShutdown(ShutdownKind.Restart, TimeSpan.FromSeconds(5));
            }
        }
        private async void DisplayShutDownDialog()
        {
            ContentDialog shutDownDialog = new ContentDialog
            {
                Title = "Are you sure you want to shut down the system?",
                Content = "The system will be shut down",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await shutDownDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                //TODO: Remove these comments
                //ShutdownManager.BeginShutdown(ShutdownKind.Shutdown, TimeSpan.FromSeconds(0));
            }
        }
        private async void DisplayNoCamFound()
        {
            ContentDialog noCameraFoundDialog = new ContentDialog
            {
                Title = "No camera found",
                Content = "Check that there is a camera connected and try again",
                CloseButtonText = "Ok",
            };
            //ConnInfo.CameraMessage = "No camera found";
            ContentDialogResult result = await noCameraFoundDialog.ShowAsync();
            //ConnInfo.IsStreaming = false;
        }
        private async void DisplayLaunchDialog()
        {
            ContentDialog launchMainAppDialog = new ContentDialog
            {
                Title = "Are you sure you want to launch the main app?",
                Content = "If the webcam is started, this app will run in the background. If not, the app is closed.",
                PrimaryButtonText = "Yes",
                CloseButtonText = "Cancel"
            };

            ContentDialogResult result = await launchMainAppDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                if (BtnStart.IsOn)
                {
                    BeginExtendedExecution();                    
                }
                else
                {
                    EndExtendedExecution();
                }

                Uri _mainAppUri = new Uri("hvl-sensorvehicle-mainapp:");
                var success = await Launcher.LaunchUriAsync(_mainAppUri);
            }
        }

        private async void BeginExtendedExecution()
        {
            ClearExtendedExecution();

            var newSession = new ExtendedExecutionSession();
            newSession.Reason = ExtendedExecutionReason.Unspecified;
            newSession.Revoked += SessionRevoked;
            ExtendedExecutionResult result = await newSession.RequestExtensionAsync();
            _session = newSession;
        }
        void ClearExtendedExecution()
        {
            if (_session != null)
            {
                _session.Revoked -= SessionRevoked;
                _session.Dispose();
                _session = null;
            }

            if (_periodicTimer != null)
            {
                _periodicTimer.Dispose();
                _periodicTimer = null;
            }
        }
        private async void SessionRevoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (args.Reason)
                {
                    case ExtendedExecutionRevokedReason.Resumed:
                        //rootPage.NotifyUser("Extended execution revoked due to returning to foreground.", NotifyType.StatusMessage);
                        break;

                    case ExtendedExecutionRevokedReason.SystemPolicy:
                        //rootPage.NotifyUser("Extended execution revoked due to system policy.", NotifyType.StatusMessage);
                        break;
                }

                EndExtendedExecution();
            });
        }
        private void EndExtendedExecution()
        {
            ClearExtendedExecution();
        }

    }
}
