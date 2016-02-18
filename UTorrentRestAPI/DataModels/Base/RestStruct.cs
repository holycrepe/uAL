namespace UTorrentRestAPI
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using RestSharp;
    using Torrent.Infrastructure;
    using Extensions;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Struct That Automatically Casts A String To/From A Primitive Value
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public partial class RestStruct : IDebuggerDisplay
    {
        #region Public Properties

        #region Public Properties: General

        [XmlIgnore]
        public string QualifiedName
            => nameof(RestStruct) + (Type == RestType.Unknown ? "" : $".{Type}");

        #endregion

        #region Public Properties: RestStruct

        #region Public Properties: RestStruct: Info

        [XmlIgnore]
        public string Constructor { get; set; } = "";

        [XmlIgnore]
        public RestType Type { get; set; } = RestType.Unknown;

        #endregion

        #region Public Properties: RestStruct: Values

        #region Public Properties: RestStruct: Values: Original

        [XmlIgnore]
        public object Original { get; set; }

        private string _value;

        [XmlIgnore]
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                SetValues("Property");
            }
        }

        #endregion

        #region Public Properties: RestStruct: Values: Conversions

        #region Public Properties: RestStruct: Values: Conversions: Implicit

        [XmlIgnore]
        public RestDict AsDict { get; set; } = null;

        [XmlIgnore]
        public JObject AsObject { get; set; } = null;

        [XmlIgnore]
        public RestList AsList { get; set; } = null;

        [XmlIgnore]
        public string AsString { get; set; }

        [XmlIgnore]
        public bool? AsBool { get; set; } = null;

        [XmlIgnore]
        public long? AsLong { get; set; } = null;

        [XmlIgnore]
        public int? AsInt { get; set; } = null;

        [XmlIgnore]
        public double? AsDouble { get; set; } = null;

        #endregion

        #region Public Properties: RestStruct: Values: Conversions: Explicit        

        public bool ToBool
            => AsBool ?? false;

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        #region Constructor

        public RestStruct()
        {
            Original = null;
            _value = AsString = string.Empty;
            SetValues("Default");
        }

        public RestStruct(string value)
        {
            Original = value;
            _value = AsString = value;
            SetValues("String");
        }

        public RestStruct(object value)
        {
            Original = value;
            _value = AsString = value?.ToString();
            SetValues("Object");
        }

        #endregion

        #region Set Values        

        protected virtual void SetAdditionalValues() { }

        #region Set Values: Base

        void SetValues(string source)
        {
            Constructor += $"{source};";
            if (SetJsonValues() || SetKeyedValues() || SetPrimitiveValues() || SetEnumerableValues()) {
                return;
            }
            SetAdditionalValues();
        }

        #endregion

        #region Set Values: Keyed

        bool SetKeyedValues()
        {
            var asDict = Original as Dictionary<string, object>;
            if (asDict != null) {
                return SetDict(RestDict.FromDict(asDict));
            }
            return false;
        }

        bool SetDict(RestDict dict, RestType type = RestType.Dict)
        {
            if (dict == null) {
                return false;
            }
            AsDict = dict;
            _value = AsDict.DebuggerDisplaySimple();
            Type = type;
            return true;
        }

        bool SetJObject()
        {
            var asJObject = Original as JObject;
            if (asJObject != null) {
                return SetDict(asJObject, RestType.JObject);
            }
            return true;
        }

        #endregion

        #region Set Values: Json

        bool SetJsonValues()
        {
            var asJToken = Original as JToken;
            if (asJToken != null) {
                switch (asJToken.Type) {
                    case JTokenType.String:
                    case JTokenType.Integer:
                    case JTokenType.Boolean:
                    case JTokenType.Float:
                        SetPrimitiveValues();
                        return true;
                    case JTokenType.Array:
                        SetJArray();
                        return true;
                    case JTokenType.Object:
                        SetJObject();
                        return true;
                    default:
                        Debugger.Break();
                        return false;
                }
            }
            return false;
        }

        bool SetJArray()
        {
            var asJArray = Original as JArray;
            if (asJArray != null) {
                return SetList(asJArray, RestType.JArray);
            }
            return true;
        }

        #endregion

        #region Set Values: Enumerable

        bool SetEnumerableValues()
        {
            // TODO Remove JsonArray
            var asJsonArray = Original as JsonArray;
            if (asJsonArray != null) {
                return SetList(asJsonArray, RestType.JsonArray);
            }
            //if (SetList(Original as JArray, RestType.Array) || SetList(Original as JToken, RestType.Array) || SetList(Original as JsonArray, RestType.JsonArray)) {
            //    return true;
            //}
            var asList = Original as IEnumerable<object>;
            if (asList != null) {
                return SetList(new RestList(asList));
            }
            return false;
        }

        bool SetList(RestList list, RestType type = RestType.List)
        {
            if (list == null) {
                return false;
            }
            AsList = list;
            _value = AsList.DebuggerDisplaySimple();
            Type = type;
            return true;
        }

        #endregion

        #region Set Values: Primitive Values

        bool SetPrimitiveValues()
        {
            string asString = Value;
            if (!string.IsNullOrEmpty(asString)) {
                AsString = asString;
                Type = RestType.String;
            }
            double asDouble;
            if (double.TryParse(Value, out asDouble)) {
                AsDouble = asDouble;
                Type = RestType.Double;
            }
            long asLong;
            if (long.TryParse(Value, out asLong)) {
                AsLong = asLong;
                Type = RestType.Long;
            }
            int asInt;
            if (int.TryParse(Value, out asInt)) {
                AsInt = asInt;
                Type = RestType.Int;
            }
            bool asBool;
            if (bool.TryParse(Value, out asBool)) {
                AsBool = asBool;
                Type = RestType.Bool;
            }
            return Type != RestType.Unknown && Type != RestType.String;
        }

        public void SetBool()
        {
            if (Value == "Y") {
                AsBool = true;
                Type = RestType.Bool;
            } else if (Value == "N") {
                AsBool = false;
                Type = RestType.Bool;
            }
        }

        #endregion

        #endregion

        #region Interfaces

        #region Interfaces: IDebuggerDisplay

        public string DebuggerDisplayValue(int level = 1)
            => Type.IsBool()
                   ? ToBool ? "Y" : "N"
                   : Type.IsDictionary()
                         ? AsDict.DebuggerDisplay(level)
                         : Type.IsEnumerable()
                               ? AsList.DebuggerDisplay(level)
                               : string.Format(Type.GetFormatString(), Value);

        public string DebuggerDisplayValueSimple(int level = 1)
            => Type.IsDictionary()
                   ? AsDict.DebuggerDisplaySimple(level)
                   : Type.IsEnumerable()
                         ? AsList.DebuggerDisplaySimple(level)
                         : DebuggerDisplayValue(level);

        public string DebuggerDisplaySimple(int level = 1)
            => DebuggerDisplayValueSimple(level);

        public string DebuggerDisplay(int level = 1)
            => $"{QualifiedName}: {DebuggerDisplayValue(level)}";

        #endregion

        #endregion
    }
}
