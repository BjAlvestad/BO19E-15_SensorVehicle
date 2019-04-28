using SensorVehicle_extras.Configuration;
using SensorVehicle_extras.Devices;
using SensorVehicle_extras.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
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
        private IPAddress addr;
        public MainPage()
        {
            this.InitializeComponent();
            GetIpAddress();

            Loaded += MainPage_Loaded;
        }

        public IPAddress GetIpAddress()
        {
            var hosts = NetworkInformation.GetHostNames();
            foreach (var host in hosts)
            {
                if (!IPAddress.TryParse(host.DisplayName, out addr))
                {
                    continue;
                }

                if (addr.AddressFamily != AddressFamily.InterNetwork)
                {
                    IpValue.Text = addr.ToString();
                    continue;
                }
                IpValue.Text = addr.ToString();
                return addr;
            }
            return null;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs eventArgs)
        {
            var camera = new Camera();
            var mediaFrameFormats = await camera.GetMediaFrameFormatsAsync();
            //ConfigurationFile.SetSupportedVideoFrameFormats(mediaFrameFormats);
            //var videoSetting = await ConfigurationFile.Read(mediaFrameFormats);

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
    }
}
