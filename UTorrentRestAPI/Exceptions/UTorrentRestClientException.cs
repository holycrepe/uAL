namespace UTorrentRestAPI.RestClient
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization;

    public class UTorrentRestClientException : ApplicationException
    {
        #region Properties
        public WebExceptionStatus Status { get; }
        public SocketError Error { get; }
        public bool EndpointUnavailable { get; } = false;
        #endregion
        public UTorrentRestClientException() { }

        public UTorrentRestClientException(string message) : base(message) { }

        public UTorrentRestClientException(string message, Exception innerException) : base(message, innerException) {
            var webEx = innerException as WebException;
            if (webEx != null)
            {
                Status = webEx.Status;
                var socketEx = webEx.InnerException as SocketException;
                if (socketEx != null)
                {
                    Error = socketEx.SocketErrorCode;
                    if (Status == WebExceptionStatus.ConnectFailure && Error == SocketError.ConnectionRefused)
                    {
                        EndpointUnavailable = socketEx.Message.Contains("target machine actively refused");
                    }
                }
            }
        }

        protected UTorrentRestClientException(SerializationInfo info, StreamingContext context) : base(info, context) {}

        static string CreateMessage(string message = null)
            =>
                "Error retrieving response from uTorrent Web API" + (message == null ? "" : ": " + message)
                + ".  Check inner details for more info.";

        public static UTorrentRestClientException New(string message = null)
            => new UTorrentRestClientException(CreateMessage(message));

        public static UTorrentRestClientException New(Exception ex, string message = null)
            => new UTorrentRestClientException(CreateMessage(message), ex);

        public static implicit operator bool(UTorrentRestClientException ex)
            => ex != null;
    }
}
