using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace SensorVehicle_extras.Connection
{
    public class ConnectionInfo : INotifyPropertyChanged
    {
        private IPAddress _ipAddr;
        private string _ssid;
        private string _deviceName;
        private string _cameraMessage;
        private bool _isStreaming;

        public IPAddress IPAddr
        {
            get { return _ipAddr; }
            private set
            {
                _ipAddr = value; RaisePropertyChanged("IPAddr");
            }
        }
        public string Ssid
        {
            get { return _ssid; }
            private set { _ssid = value; RaisePropertyChanged("Ssid"); }
        }
        public string DeviceName
        {
            get { return _deviceName; }
            private set { _deviceName = value; RaisePropertyChanged("DeviceName"); }
        }
        public string CameraMessage
        {
            get { return _cameraMessage; }
            set { _cameraMessage = value; RaisePropertyChanged("CameraMessage"); }
        }
        public bool IsStreaming
        {
            get { return _isStreaming; }
            set { _isStreaming = value; RaisePropertyChanged("IsStreaming"); }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public ConnectionInfo()
        {
            GetIpAddress();
            GetCurrentNetworkName();
            GetDeviceName();
            CameraMessage = "Webcam is off";
            IsStreaming = false;
        }

        private void GetIpAddress()
        {
            var hosts = NetworkInformation.GetHostNames();
            foreach (var host in hosts)
            {
                if (!IPAddress.TryParse(host.DisplayName, out _ipAddr))
                {
                    IPAddr = _ipAddr;
                    continue;
                }

                if (_ipAddr.AddressFamily != AddressFamily.InterNetwork)
                {
                    continue;
                }
            }
        }
        private void GetCurrentNetworkName()
        {
            try
            {
                var icp = NetworkInformation.GetInternetConnectionProfile();
                if (icp != null)
                {
                    Ssid = icp.ProfileName;
                }
            }
            catch (Exception ex)
            {
                Ssid = "NoInternetConnection";
            }

        }
        public void GetDeviceName()
        {
            try
            {
                var hostName = NetworkInformation.GetHostNames().FirstOrDefault(x => x.Type == HostNameType.DomainName);
                if (hostName != null)
                {
                    DeviceName = hostName.DisplayName;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                DeviceName = "NoDeviceNameText";

            }
        }

    }
}
