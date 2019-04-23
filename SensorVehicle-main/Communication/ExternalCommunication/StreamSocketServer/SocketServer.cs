using System;
using System.Diagnostics;
using System.IO;
using Windows.Networking.Sockets;

// https://docs.microsoft.com/en-us/windows/uwp/networking/sockets
namespace Communication.ExternalCommunication.StreamSocketServer
{
    public class SocketServer
    {
        public const string PortNumber = "51915";
        private StreamSocketListener _streamSocketListener;
        private bool _clientConnectionOpen;

        public async void StartServer()
        {
            try
            {
                _streamSocketListener = new StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                _streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await _streamSocketListener.BindServiceNameAsync(PortNumber);

                Debug.WriteLine("server is listening...", "TcpSocketServer");
            }
            catch (Exception ex)
            {
                SocketErrorStatus webErrorStatus = SocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message);
            }
        }

        public async void StopServer()
        {
            if (_streamSocketListener == null) return;

            _streamSocketListener.ConnectionReceived -= StreamSocketListener_ConnectionReceived;
            await _streamSocketListener.CancelIOAsync();
            _streamSocketListener.Dispose();
        }

        private async void StreamSocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            try
            {
                //if(_clientConnectionOpen) throw new Exception("There is already someone connected to the server");

                Debug.WriteLine($"server received a connection");
                _clientConnectionOpen = true;

                using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
                using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    while (_clientConnectionOpen)
                    {
                        Debug.WriteLine("server is awaiting request.");
                        string request = await streamReader.ReadLineAsync();
                        Debug.WriteLine($"server received the request: \"{request}\"", "TcpSocketServer");
                        _clientConnectionOpen = request != "<EXIT>";

                        if (_clientConnectionOpen)
                        {
                            string response = $"Server confirms receiving request: {request}";
                            await streamWriter.WriteLineAsync(response);
                            await streamWriter.FlushAsync();
                            Debug.WriteLine($"server sent back the response: \"{response}\"", "TcpSocketServer");
                        }
                    }

                    Debug.WriteLine("Server received exit message...");
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Exception occured on SocketServer: {e.Message}");
                //TODO: Add exception display to settings page
            }
            finally
            {
                _clientConnectionOpen = false;
                Debug.WriteLine("server closed its socket", "TcpSocketServer");
            }
        }
    }
}
