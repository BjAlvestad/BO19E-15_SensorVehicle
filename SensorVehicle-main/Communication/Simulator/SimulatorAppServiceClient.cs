using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace Communication.Simulator
{
    // Based on https://github.com/lprichar/UwpMessageRelay/blob/master/UwpMessageRelay.Consumer/Services/MessageRelayService.cs
    public class SimulatorAppServiceClient
    {
        const string AppServiceName = "no.hvl.SensorVehicle2dSimAppService";
        private AppServiceConnection _connection;
        public event Action<ValueSet> OnMessageReceived;

        private async Task<AppServiceConnection> CachedConnection()
        {
            if (_connection != null) return _connection;
            _connection = await MakeConnection();
            _connection.RequestReceived += ConnectionOnRequestReceived;
            _connection.ServiceClosed += ConnectionOnServiceClosed;
            return _connection;
        }

        private async Task<AppServiceConnection> MakeConnection()
        {
            var listing = await AppServiceCatalog.FindAppServiceProvidersAsync(AppServiceName);

            if (listing.Count == 0)
            {
                throw new Exception("Unable to find the simulators app service '" + AppServiceName + "'");
            }

            var packageName = listing[0].PackageFamilyName;

            var connection = new AppServiceConnection
            {
                AppServiceName = AppServiceName,
                PackageFamilyName = packageName
            };

            var status = await connection.OpenAsync();

            if (status != AppServiceConnectionStatus.Success)
            {
                throw new Exception("Could not connect to simulator.\nAppService connection status: " + status);
            }

            return connection;
        }

        private void ConnectionOnServiceClosed(AppServiceConnection sender, AppServiceClosedEventArgs args)
        {
            DisposeConnection();
        }

        private void DisposeConnection()
        {
            if (_connection == null) return;

            _connection.RequestReceived -= ConnectionOnRequestReceived;
            _connection.ServiceClosed -= ConnectionOnServiceClosed;
            _connection.Dispose();
            _connection = null;
        }

        private void ConnectionOnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            var appServiceDeferral = args.GetDeferral();
            try
            {
                ValueSet valueSet = args.Request.Message;
                OnMessageReceived?.Invoke(valueSet);
            }
            finally
            {
                appServiceDeferral.Complete();
            }
        }

        public void CloseConnection()
        {
            DisposeConnection();
        }

        public async Task SendMessageAsync(ValueSet keyValuePair)
        {
            var connection = await CachedConnection();
            var result = await connection.SendMessageAsync(keyValuePair);
            if (result.Status == AppServiceResponseStatus.Success)
            {
                return;
            }

            throw new Exception("Error when sending message to simulator.\n" +
                                $"AppService response status: {result.Status}.\n" +
                                "Is the simulator app running?");
        }

        public async Task<ValueSet> RequestDataAsync(ValueSet keyValuePair)
        {
            var connection = await CachedConnection();
            var result = await connection.SendMessageAsync(keyValuePair);
            if (result.Status == AppServiceResponseStatus.Success)
            {
                return result.Message;
            }
            
            throw new Exception("Error when sending request to simulator.\n" +
                                $"AppService response status: {result.Status}.\n" +
                                (result.Status == AppServiceResponseStatus.Failure ? "Is the simulator app running?" : ""));
        }
    }
}
