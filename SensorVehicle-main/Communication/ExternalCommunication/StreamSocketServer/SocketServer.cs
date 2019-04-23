using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Windows.Networking.Sockets;

// https://docs.microsoft.com/en-us/windows/uwp/networking/sockets
namespace Communication.ExternalCommunication.StreamSocketServer
{
    public class SocketServer
    {
        static string PortNumber = "11000";
        private StreamSocketListener streamSocketListener;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        //public async void StartUpServer()
        //{
        //    cancellationTokenSource = new CancellationTokenSource();
        //    cancellationToken = cancellationTokenSource.Token;
        //}

        public async void StartServer()
        {
            try
            {
                streamSocketListener = new StreamSocketListener();

                // The ConnectionReceived event is raised when connections are received.
                streamSocketListener.ConnectionReceived += StreamSocketListener_ConnectionReceived;

                // Start listening for incoming TCP connections on the specified port. You can specify any port that's not currently in use.
                await streamSocketListener.BindServiceNameAsync(PortNumber);

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
            if (streamSocketListener == null) return;

            streamSocketListener.ConnectionReceived -= StreamSocketListener_ConnectionReceived;
            await streamSocketListener.CancelIOAsync();
            streamSocketListener.Dispose();
            //TODO: Check if steamSocketListener is null after disposing
        }

        private async void StreamSocketListener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Debug.WriteLine($"server received a connection");
            //TODO: consider keeping connection open, instead of closing after each request / response
            string request;
            using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
            {
                request = await streamReader.ReadLineAsync();
            }
            
            Debug.WriteLine($"server received the request: \"{request}\"", "TcpSocketServer");

            // Echo the request back as the response.
            using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
            {
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    await streamWriter.WriteLineAsync(request);
                    await streamWriter.FlushAsync();
                }
            }

            Debug.WriteLine($"server sent back the response: \"{request}\"", "TcpSocketServer");

            sender.Dispose();

            Debug.WriteLine("server closed its socket", "TcpSocketServer");
        }
    }
}