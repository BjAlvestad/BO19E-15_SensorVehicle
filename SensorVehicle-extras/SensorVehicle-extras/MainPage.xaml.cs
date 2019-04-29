using SensorVehicle_extras.Configuration;
using SensorVehicle_extras.Connection;
using SensorVehicle_extras.Devices;
using SensorVehicle_extras.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
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

        public ConnectionInfo ConnInfo { get =>_connInfo; private set => _connInfo = value; }
        public Camera Camera { get => _camera; private set => _camera = value; }
        public HttpServer HttpServer { get => _httpServer; private set => _httpServer = value; }

        public MainPage()
        {
            this.InitializeComponent();
            ConnInfo = new ConnectionInfo();
        }       

        private async void MainPage_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            var camera = new Camera();
            var mediaFrameFormats = await camera.GetMediaFrameFormatsAsync();

            VideoSetting videoSetting = new VideoSetting
            {
                VideoResolution = VideoResolution.SD640_480,
                VideoSubtype = VideoSubtype.NV12,
                VideoQuality = 0.2,
                UsedThreads = 1
            };
            await camera.Initialize(videoSetting);
            camera.Start();

            var httpServer = new HttpServer(camera);
            httpServer.Start();
        }

        private async void BtnStart_Toggled(object sender, RoutedEventArgs e)
        {
            await Run();            
        }
        private async Task Run()
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
            await Camera.Initialize(videoSetting);
            if(!ConnInfo.IsStreaming)
            {
                Camera.Start();
                ConnInfo.IsStreaming = true;
            }
            else
            {
                await Camera.Stop();
                ConnInfo.IsStreaming = false;
            }

            
        }
    }
}
