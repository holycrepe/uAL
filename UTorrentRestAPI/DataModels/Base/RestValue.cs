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
    [DebuggerDisplay("{DebuggerDisplay()}")]
    public partial class RestValue : JValue, IDebuggerDisplay
    {
        [XmlIgnore]
        public string Constructor { get; set; } = "";

        [XmlIgnore]
        public RestType RType { get; set; } = RestType.Unknown;
        [XmlIgnore]
        public object Original { get; set; }

        private string _value;

        [XmlIgnore]
        public string RValue
        {
            get { return _value; }
            set { _value = value; SetValues("Property"); }
        }
        [XmlIgnore]
        public string AsString { get; set; }
        public RestValue() : base(string.Empty)
        {
            Original = null;
            _value = AsString = string.Empty;
            SetValues("Default");
        }
        public RestValue(string value) : base(value)
        {
            Original = value;
            _value = AsString = value;
            SetValues("String");
        }
        public RestValue(JValue value) : base(value)
        {
            Original = value;
            _value = AsString = value?.ToString();
            SetValues("JValue");
        }
        public RestValue(object value) : base(value)
        {
            Original = value;
            _value = AsString = value?.ToString();
            SetValues("Object");
        }
        public void SetValues(string source)
            => Constructor += "{source},";
        public bool ToBool
            => base.Type == JTokenType.Boolean ? (bool) base.Value : false;
        

        public string DebuggerDisplayValue(int level = 1)
            => RType.IsBool() || Type == JTokenType.Boolean
                        ? ToBool ? "Y" : "N"
                        //: RType.IsDictionary()
                        //? AsDict.DebuggerDisplay(level)
                        //: RType.IsEnumerable()
                        //? AsList.DebuggerDisplay(level)
                        : DebuggerDisplayValue(level);

        public string DebuggerDisplayValueSimple(int level = 1)
            => DebuggerDisplayValue(level);
        //=> RType.IsDictionary()
        //            ? AsDict.DebuggerDisplaySimple(level)
        //            : RType.IsEnumerable()
        //            ? AsList.DebuggerDisplaySimple(level)
        //            : DebuggerDisplayValue(level);


        public string QualifiedName
            => nameof(RestValue) +  $".{Type}");
        public string DebuggerDisplaySimple(int level = 1)
        	=> DebuggerDisplayValueSimple(level);
        public string DebuggerDisplay(int level = 1)
            => $"{QualifiedName}: {DebuggerDisplayValue(level)}";
    }
}