namespace UTorrentRestAPI
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using RestSharp;
    using Torrent.Infrastructure;
    using Newtonsoft.Json.Linq;
    /// <summary>
    /// Struct That Automatically Casts A String To/From A Primitive Value
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay()}")]
    public partial class RestToken
    {
        //public static implicit operator RestToken(String value)
        //    => new RestToken(value);

        #region Cast To RestToken
        //public static implicit operator RestToken(RestDict value)
        //	=> new RestToken(value);
        //public static implicit operator RestToken(RestList value)
        //	=> new RestToken(value);
        public static implicit operator RestToken(TorrentStatus value)
        	=> new RestToken(value);
        public static implicit operator RestToken(RestType value)
        	=> new RestToken(value);
        public static implicit operator RestToken(string value)
            => new RestToken(value);
        public static implicit operator RestToken(bool value)
            => new RestToken(value);
        public static implicit operator RestToken(long value)
            => new RestToken(value);
        public static implicit operator RestToken(int value)
            => new RestToken(value);
        #endregion
        
        #region Cast From RestToken
        //public static implicit operator RestDict(RestToken value)
        //	=> value.AsDict ?? new RestDict();
        //public static implicit operator RestList(RestToken value)
        //	=> value.AsList ?? new RestList();
        public static implicit operator TorrentStatus(RestToken value)
            => (TorrentStatus) (int) value;
        public static implicit operator RestType(RestToken value)
        	=> (RestType) (int) value;
        public static implicit operator string(RestToken value)
            => value.Value.ToString();
        public static implicit operator bool(RestToken value)
            => value.Type == JTokenType.Boolean ? value.ToBool : !string.IsNullOrEmpty(value?.RValue);
        public static implicit operator long(RestToken value)
            => value.Type == JTokenType.Integer ? (long) value.Value : 0;
            //=> value.AsLong ?? 0;
        public static implicit operator int(RestToken value)
            => value.Type == JTokenType.Integer ? (int)value.Value : 0;
        public static implicit operator double(RestToken value)
            => value.Type == JTokenType.Float ? (double)value.Value : 
               value.Type == JTokenType.Integer? Convert.ToDouble(value.Value) : 0;

        //public static implicit operator JsonObject(RestToken value)
        //{
        //    string strValue = value.AsString;
        //    object objValue = value.Original;
        //    var final = JsonConvert.DeserializeObject<JsonObject>(strValue);
        //    return final;
        //}
        #endregion
    }
}