namespace Helpers
{
    public class Error : ThreadSafeNotifyPropertyChanged
    {
        private bool _unacknowledged;
        /// <summary>
        /// True if there is an error which hasn't been acknowledged with <see cref="Clear"/>
        /// </summary>
        public bool Unacknowledged
        {
            get { return _unacknowledged; }
            set { SetProperty(ref _unacknowledged, value); }
        }
        
        private string _Message;
        /// <summary>
        /// Descriptive error message (without stack-trace)
        /// </summary>
        public string Message
        {
            get { return _Message; }
            set { SetProperty(ref _Message, value); }
        }

        private string _detailedMessage;
        /// <summary>
        /// Detailed error message (including stack trace)
        /// </summary>
        public string DetailedMessage
        {
            get { return _detailedMessage; }
            set { SetProperty(ref _detailedMessage, value); }
        }

        private bool _showDetailedMessage;
        /// <summary>
        /// <see langword="true"/> indicates wish to see <see cref="Message"/>. <see langword="false"/> indicates wish to se DetailedMessage.
        /// </summary>
        public bool ShowDetailedMessage
        {
            get { return _showDetailedMessage; }
            set { SetProperty(ref _showDetailedMessage, value); }
        }

        /// <summary>
        /// Acknowledges error by clearing <see cref="Message"/> and <see cref="DetailedMessage"/>, and setting <see cref="Unacknowledged"/> to <see langword="false"/>
        /// </summary>
        public void Clear()
        {
            Message = "";
            DetailedMessage = "";
            Unacknowledged = false;
        }
    }
}
