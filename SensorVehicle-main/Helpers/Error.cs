namespace Helpers
{
    public class Error : ThreadSafeNotifyPropertyChanged
    {
        private bool _unacknowledged;
        public bool Unacknowledged
        {
            get { return _unacknowledged; }
            set { SetProperty(ref _unacknowledged, value); }
        }
        
        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { SetProperty(ref _Message, value); }
        }

        private string _detailedMessage;
        public string DetailedMessage
        {
            get { return _detailedMessage; }
            set { SetProperty(ref _detailedMessage, value); }
        }

        private bool _showDetailedMessage;
        public bool ShowDetailedMessage
        {
            get { return _showDetailedMessage; }
            set { SetProperty(ref _showDetailedMessage, value); }
        }

        public void Clear()
        {
            Message = "";
            DetailedMessage = "";
            Unacknowledged = false;
        }
    }
}
