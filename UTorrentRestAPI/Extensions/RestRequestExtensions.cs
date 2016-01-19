namespace UTorrentRestAPI.Extensions
{
    using System.IO;
    using RestSharp;
    using Torrent.Extensions;

    public static class RestRequestExtensions
    {        
        public static IRestRequest AddHash(this IRestRequest request, string hash) 
            => request.AddUrlQuery(nameof(hash), hash);
        public static IRestRequest AddPath(this IRestRequest request, string path) 
            => request.AddUrlQuery(nameof(path), path);

        public static IRestRequest AddToken(this IRestRequest request, string token, bool useToken = true)
            => request;//(token == null ? null : request.AddUrlQuery(nameof(token), token, useToken));
        public static IRestRequest SetAction(this IRestRequest request, string action) 
            => request.AddUrlQuery(nameof(action), action);
        public static IRestRequest AddFile(this IRestRequest request, string name, Stream stream, string fileName, string contentType = null)
            => request.AddFile(name, stream.CopyTo, fileName, contentType);

        public static IRestRequest AddUrlQueries(this IRestRequest request, params object[] parameters)
        {
            for (var i = 0; i < parameters.Length; i += 2) {
                request = request.AddUrlQuery(parameters[i], parameters[i + 1]);
            }
            return request;
        }
        public static IRestRequest AddUrlQuery(this IRestRequest request, object name, object value, bool doAdd = true, string prefix = "&")
        {
            if (doAdd)
            {
                if (request.Resource == null) {
                    request.Resource = "";
                    prefix = "?";
                }
                request.Resource += $"{prefix}{name.ToString().ToSnakeCase()}={value}";
            }
            return request;
        }
    }
}