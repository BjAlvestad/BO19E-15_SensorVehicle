using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.Networking.Sockets;
using Communication.ExternalCommunication.Handler;
using Communication.ExternalCommunication.Handler.Constants;
using Newtonsoft.Json;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

// https://docs.microsoft.com/en-us/windows/uwp/networking/sockets
namespace Communication.ExternalCommunication.StreamSocketServer
{
    public class SocketServer
    {
        public const string PortNumber = "51915";
        private StreamSocketListener _streamSocketListener;
        private bool _clientConnectionOpen;
        private RequestHandler _requestHandler;
        private IWheel _wheel;

        public SocketServer(IWheel wheel, IUltrasonic ultrasonic, ILidarDistance lidar, IEncoders encoders)
        {
            _requestHandler = new RequestHandler(wheel, ultrasonic, lidar, encoders);
            _wheel = wheel;
        }

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

        // SOME OF THE COMMANDS THAT CAN BE SENT TO SOCKET SERVER:
        // Set wheel speed:     { "REQUEST_TYPE": "Command", "COMPONENT": "Wheel", "LEFT": "0", "RIGHT": "0" }
        // Request Ultrasound distance:     { "REQUEST_TYPE": "Data", "COMPONENT": "Ultrasound" }
        // Request Wheel and Ultrasound:     { "REQUEST_TYPE": "Data", "COMPONENT": "Wheel Ultrasound" }
        // Send exit message to server:     { "REQUEST_TYPE": "Exit" }
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

                        Dictionary<string, string> requestKeyValuePair = JsonConvert.DeserializeObject<Dictionary<string, string>>(request);
                        Dictionary<string, string> responseKeyValuePair = _requestHandler.HandleRequest(requestKeyValuePair);
                        string response = JsonConvert.SerializeObject(responseKeyValuePair);

                        await streamWriter.WriteLineAsync(response);
                        await streamWriter.FlushAsync();
                        Debug.WriteLine($"server sent back the response: \"{response}\"", "TcpSocketServer");

                        if (responseKeyValuePair.ContainsKey(Key.ExitConfirmation)) _clientConnectionOpen = false;
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
                Debug.WriteLine("Server stopped communication with client, and awaits new connection ...", "TcpSocketServer");
                _wheel.SetSpeed(0 ,0, false);
            }
        }
    }
}
