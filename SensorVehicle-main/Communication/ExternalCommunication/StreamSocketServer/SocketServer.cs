using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Networking.Sockets;
using Communication.ExternalCommunication.Handler;
using Communication.ExternalCommunication.Handler.Constants;
using Helpers;
using Newtonsoft.Json;
using VehicleEquipment.DistanceMeasurement.Lidar;
using VehicleEquipment.DistanceMeasurement.Ultrasound;
using VehicleEquipment.Locomotion.Encoder;
using VehicleEquipment.Locomotion.Wheels;

// https://docs.microsoft.com/en-us/windows/uwp/networking/sockets
namespace Communication.ExternalCommunication.StreamSocketServer
{
    public class SocketServer : ThreadSafeNotifyPropertyChanged, ISocketServer
    {
        private readonly RequestHandler _requestHandler;
        private readonly IWheel _wheel;
        private StreamSocketListener _streamSocketListener;
        private bool _clientConnectionOpen;

        public Error Error { get; }

        public SocketServer(IWheel wheel, IUltrasonic ultrasonic, ILidarDistance lidar, IEncoders encoders)
        {
            _requestHandler = new RequestHandler(wheel, ultrasonic, lidar, encoders);
            _wheel = wheel;
            Error = new Error();
            PortNumber = "51915";
        }

        public string PortNumber { get; }

        private int _numberOfClientsConnected;
        public int NumberOfClientsConnected
        {
            get { return _numberOfClientsConnected; }
            private set { SetProperty(ref _numberOfClientsConnected, value); }
        }

        private bool _listenForConnections;
        public bool ListenForConnections
        {
            get { return _listenForConnections; }
            set
            {
                if (value == _listenForConnections) return;

                if (value)
                {
                    StartServer();
                }
                else
                {
                    StopServer();
                }

                SetProperty(ref _listenForConnections, value);
            }
        }

        private async void StartServer()
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
                string errorMessage = webErrorStatus.ToString() != "Unknown" ? webErrorStatus.ToString() : ex.Message;
                Debug.WriteLine(errorMessage);
                Error.Message = errorMessage;
                Error.DetailedMessage = ex.ToString();
                Error.Unacknowledged = true;
                ListenForConnections = false;
            }
        }

        private async void StopServer()
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
            // TODO: Prevent multiple people connecting to the vehicle (for control) at the same time
            try
            {
                Debug.WriteLine($"server received a connection");
                _clientConnectionOpen = true;
                NumberOfClientsConnected += 1;

                using (var streamReader = new StreamReader(args.Socket.InputStream.AsStreamForRead()))
                using (Stream outputStream = args.Socket.OutputStream.AsStreamForWrite())
                using (var streamWriter = new StreamWriter(outputStream))
                {
                    while (_clientConnectionOpen)
                    {
                        Debug.WriteLine("server is awaiting request.");
                        string request = await streamReader.ReadLineAsync();
                        Debug.WriteLine($"server received the request: \"{request}\"", "TcpSocketServer");

                        Dictionary<string, string> responseKeyValuePair;
                        try
                        {
                            Dictionary<string, string> requestKeyValuePair = JsonConvert.DeserializeObject<Dictionary<string, string>>(request);
                            responseKeyValuePair = _requestHandler.HandleRequest(requestKeyValuePair);
                        }
                        catch (Exception e) when (e is JsonReaderException || e is ArgumentNullException)
                        {
                            Error.Message = GetJsonReaderErrorMessage(request);
                            Error.DetailedMessage = e.ToString();
                            Error.Unacknowledged = true;

                            responseKeyValuePair = new Dictionary<string, string> {{Key.Error, Error.Message}};
                        }

                        string response = JsonConvert.SerializeObject(responseKeyValuePair);

                        await streamWriter.WriteLineAsync(response);
                        await streamWriter.FlushAsync();
                        Debug.WriteLine($"server sent back the response: \"{response}\"", "TcpSocketServer");

                        if (responseKeyValuePair.ContainsKey(Key.ExitConfirmation)) _clientConnectionOpen = false;
                    }

                    Debug.WriteLine("Server received exit message...");
                }
            }
            catch (IOException ioe)
            {
                Error.Message = $"{ioe.Message}\n\n" +
                                $"Note: Client connection closed, but Socket Server is still open for new connections.\n" +
                                $"Reconnect and try again";
                Error.DetailedMessage = ioe.ToString();
                Error.Unacknowledged = true;
            }
            catch (Exception e)
            {
                ListenForConnections = false;
                Error.Message = $"{e.Message}\n\n" +
                                $"Note: Async Server has been shut down";
                Error.DetailedMessage = e.ToString();
                Error.Unacknowledged = true;
            }
            finally
            {
                NumberOfClientsConnected -= 1;
                _clientConnectionOpen = false;
                _wheel.SetSpeed(0 ,0, false);
            }
        }

        private static string GetJsonReaderErrorMessage(string incorrectlyFormattedJsonString)
        {
            return "Could not deserialize received json string to Key-Value pair format.\n" +
                   "************************************\n" +
                   "String received:\n" +
                   $"{incorrectlyFormattedJsonString}\n" +
                   "************************************\n" +
                   "\n" +
                   "The string should be on the following format:\n" +
                   "{ \"KEY\": \"Value\"}, { \"KEY\": \"Value\"}\n" +
                   "\n" +
                   "Example to request Ultrasound data:\n" +
                   "{ \"REQUEST_TYPE\": \"Data\", \"COMPONENT\": \"Ultrasound\" }\n" +
                   "\n" +
                   "Note: Client connection is still open. Reformat your text correctly and try again.";
        }
    }
}
