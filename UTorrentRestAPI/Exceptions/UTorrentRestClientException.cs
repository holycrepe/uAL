namespace UTorrentRestAPI.RestClient
{
    using System;
    using System.Runtime.Serialization;

    public class UTorrentRestClientException : ApplicationException
    {
        public UTorrentRestClientException() { }

        public UTorrentRestClientException(string message) : base(message) {}

        public UTorrentRestClientException(string message, Exception innerException) : base(message, innerException) { }

        protected UTorrentRestClientException(SerializationInfo info, StreamingContext context) : base(info, context) { }

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