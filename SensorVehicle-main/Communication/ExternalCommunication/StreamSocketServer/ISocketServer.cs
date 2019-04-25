using Helpers;

namespace Communication.ExternalCommunication.StreamSocketServer
{
    public interface ISocketServer
    {
        Error Error { get; }

        bool ListenForConnections { get; set; }
    }
}
