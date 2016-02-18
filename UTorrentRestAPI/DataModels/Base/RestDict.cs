namespace UTorrentRestAPI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using RestSharp;
    using Torrent.Extensions;
    using Torrent.Infrastructure;
    using Newtonsoft.Json.Linq;

    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class RestDict : Dictionary<string, RestStruct>, IDebuggerDisplay
    {
        public string DebuggerDisplay(int level = 1)
            => this.GetDebuggerDisplay(level);

        public string DebuggerDisplaySimple(int level = 1)
            => this.GetDebuggerDisplaySimple(level);


        public RestDict() : base() { }

        public static RestDict FromDict(IDictionary<string, object> dict)
        {
            var newDict = new RestDict();
            foreach (var kvp in dict) {
                newDict.Add(kvp.Key, new RestStruct(kvp.Value));
            }
            return newDict;
        }

        public static RestDict FromDict(IDictionary<string, JToken> dict)
        {
            var newDict = new RestDict();
            foreach (var kvp in dict) {
                newDict.Add(kvp.Key, new RestStruct(kvp.Value));
            }
            return newDict;
        }

        //public static implicit operator RestDict(Dictionary<string, object> dict)
        //    => FromDict(dict);

        //public static implicit operator RestDict(Dictionary<string, JToken> dict)
        //    => FromDict(dict);

        public static implicit operator RestDict(JObject dict)
            => FromDict(dict);
    }
}
