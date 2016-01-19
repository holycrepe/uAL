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
    public partial class RestValue
    {
        //public static implicit operator RestToken(String value)
        //    => new RestToken(value);

        #region Cast To RestToken
        //public static implicit operator RestToken(RestDict value)
        //	=> new RestToken(value);
        //public static implicit operator RestToken(RestList value)
        //	=> new RestToken(value);
        public static implicit operator RestValue(TorrentStatus value)
        	=> new RestValue(value);
        public static implicit operator RestValue(RestType value)
        	=> new RestValue(value);
        public static implicit operator RestValue(string value)
            => new RestValue(value);
        public static implicit operator RestValue(bool value)
            => new RestValue(value);
        public static implicit operator RestValue(long value)
            => new RestValue(value);
        public static implicit operator RestValue(int value)
            => new RestValue(value);
        #endregion
        
        #region Cast From RestToken
        //public static implicit operator RestDict(RestToken value)
        //	=> value.AsDict ?? new RestDict();
        //public static implicit operator RestList(RestToken value)
        //	=> value.AsList ?? new RestList();
        public static implicit operator TorrentStatus(RestValue value)
            => (TorrentStatus) (int) value;
        public static implicit operator RestType(RestValue value)
        	=> (RestType) (int) value;
        public static implicit operator string(RestValue value)
            => value.Value.ToString();
        public static implicit operator bool(RestValue value)
            => value.Type == JTokenType.Boolean ? value.ToBool : !string.IsNullOrEmpty(value?.RValue);
        public static implicit operator long(RestValue value)
            => value.Type == JTokenType.Integer ? (long) value.Value : 0;
            //=> value.AsLong ?? 0;
        public static implicit operator int(RestValue value)
            => value.Type == JTokenType.Integer ? (int)value.Value : 0;
        public static implicit operator double(RestValue value)
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