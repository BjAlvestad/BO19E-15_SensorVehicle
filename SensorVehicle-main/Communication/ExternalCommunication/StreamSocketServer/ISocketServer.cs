using System.ComponentModel;
using Helpers;

namespace Communication.ExternalCommunication.StreamSocketServer
{
    public interface ISocketServer : INotifyPropertyChanged
    {
        string PortNumber { get; }

        Error Error { get; }

        int NumberOfClientsConnected { get; }

        bool ListenForConnections { get; set; }
    }
}
