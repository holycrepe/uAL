namespace UTorrentRestAPI
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using Puchalapalli.Infrastructure.Interfaces;
    using RestSharp;
    using Torrent.Infrastructure;

    /// <summary>
    /// Struct That Automatically Casts A String To/From A Primitive Value
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public partial class RestStruct : IDebuggerDisplay
    {
        //public static implicit operator RestStruct(String value)
        //    => new RestStruct(value);

        #region Cast To RestStruct

        public static implicit operator RestStruct(RestDict value)
            => new RestStruct(value);

        public static implicit operator RestStruct(RestList value)
            => new RestStruct(value);

        public static implicit operator RestStruct(TorrentStatus value)
            => new RestStruct(value);

        public static implicit operator RestStruct(RestType value)
            => new RestStruct(value);

        public static implicit operator RestStruct(string value)
            => new RestStruct(value);

        public static implicit operator RestStruct(bool value)
            => new RestStruct(value);

        public static implicit operator RestStruct(long value)
            => new RestStruct(value);

        public static implicit operator RestStruct(int value)
            => new RestStruct(value);

        #endregion

        #region Cast From RestStruct

        public static implicit operator RestDict(RestStruct value)
            => value.AsDict ?? new RestDict();

        public static implicit operator RestList(RestStruct value)
            => value.AsList ?? new RestList();

        public static implicit operator TorrentStatus(RestStruct value)
            => (TorrentStatus) (int) value;

        public static implicit operator RestType(RestStruct value)
            => (RestType) (int) value;

        public static implicit operator string(RestStruct value)
            => value?.AsString;

        public static implicit operator bool(RestStruct value)
            => value.AsBool ?? !string.IsNullOrEmpty(value?.Value);

        public static implicit operator long(RestStruct value)
            => value.AsLong ?? 0;

        public static implicit operator int(RestStruct value)
            => value.AsInt ?? 0;

        public static implicit operator double(RestStruct value)
            => value.AsDouble ?? 0;

        public static implicit operator JsonObject(RestStruct value)
        {
            string strValue = value.AsString;
            object objValue = value.Original;
            var final = JsonConvert.DeserializeObject<JsonObject>(strValue);
            return final;
        }

        #endregion
    }
}
