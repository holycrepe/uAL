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
    using Newtonsoft.Json;
    [DebuggerDisplay("{DebuggerDisplay()}")]
    public class RestNestedArray : List<RestArray>, IDebuggerDisplay
    {
        public string DebuggerDisplay(int level = 1)
            => this.GetDebuggerDisplay(level);
        public string DebuggerDisplaySimple(int level = 1)
            => this.GetDebuggerDisplaySimple(level);
    }

    [DebuggerDisplay("{DebuggerDisplay()}")]
    public class RestArray : RestArray<RestToken, JToken>
    {
        //public string DebuggerDisplay()
        //    => this.GetDebuggerDisplay();
        //public string DebuggerDisplaySimple()
        //    => this.GetDebuggerDisplaySimple();

        //public static RestArray FromList(IEnumerable<RestStruct> list)
        //{
        //    var newList = new RestArray();
        //    newList.AddRange(list);
        //    return newList;
        //} 
        
        public RestArray() : base() {}
        public RestArray(IEnumerable<RestToken> list) : base() {
            if (list != null)
            {
                AddRange(list);
            }
        }
        public RestArray(IEnumerable<JToken> list) : base()
        {
            if (list != null)
            {
                AddRange(list.Select(item => new RestToken(item)));
            }
        }
        public RestArray(IEnumerable<object> list) : base() {
            if (list != null)
            {
                foreach (JToken item in list)
                {
                    RestStruct t = JsonConvert.DeserializeObject<RestStruct>(item.ToString());
                    Add(t);
                }
            }
        }

        protected override bool TryCast(object item, out RestStruct newItem)
        {           
            try {
                newItem = new RestStruct(item);
                return true;
            } catch (InvalidCastException) { }
			return base.TryCast(item, out newItem);
        }
        public new static RestArray FromList(IEnumerable<object> list)
            => new RestArray(list);
        public new static RestArray FromList(IEnumerable<JToken> list)
            => new RestArray(list);
        public new static RestArray FromList(IEnumerable<RestStruct> list)
        	=> new RestArray(list);
        public static implicit operator RestArray(List<object> list)
            => FromList(list);
        public static implicit operator RestArray(object[] list)
            => FromList(list);
        public static implicit operator RestArray(List<JToken> list)
            => FromList(list);
        public static implicit operator RestArray(JToken[] list)
            => FromList(list);
        public static implicit operator RestArray(RestStruct[] list)
            => FromList(list);
        //public static implicit operator RestArray(JToken json)
        //    => FromList(json.Children());
        public static implicit operator RestArray(JArray json)
            => FromList(json);
        public static implicit operator RestArray(JsonArray json)
            => FromList(json);
    }

    [DebuggerDisplay("{DebuggerDisplay()}")]
    public class RestArray<T, JType> : List<T>, IDebuggerDisplay where T: IRestJsonItem<JType>, IDebuggerDisplay
    {
        public string DebuggerDisplay(int level = 1)
            => this.GetDebuggerDisplay(level);
        public string DebuggerDisplaySimple(int level = 1)
            => this.GetDebuggerDisplaySimple(level);
 
        
        public RestArray() : base() {}
        public RestArray(IEnumerable<T> list) {
        	AddRange(list);
        }
        public RestArray(IEnumerable<JType> list)
        {            
            foreach(JType item in list)
            {
                T t = JsonConvert.DeserializeObject<T>(item.ToString());
                Add(t);
            }
        }
        public RestArray(IEnumerable<object> list) {
        	foreach (var item in list) {
                T newItem;
                if (TryCast(item, out newItem)) {
                    Add(newItem);
                }
            }
        }
        
        protected virtual bool TryCast(object item, out T newItem)
        {
            try
            {
                newItem = (T)item;
                return true;
            }
            catch (InvalidCastException) { } 
            newItem = default(T);
            return false;
        }
        public static RestArray<T, JType> FromList(IEnumerable<object> list)
            => new RestArray<T, JType>(list);
        public static RestArray<T, JType> FromList(IEnumerable<JType> list)
            => new RestArray<T, JType>(list);
        public static RestArray<T, JType> FromList(IEnumerable<T> list)
        	=> new RestArray<T, JType>(list);
        public static implicit operator RestArray<T, JType>(List<JType> list)
            => FromList(list);
        public static implicit operator RestArray<T, JType>(List<object> list)
            => FromList(list);
        public static implicit operator RestArray<T, JType>(object[] list)
            => FromList(list);    
        public static implicit operator RestArray<T, JType>(T[] list)
            => FromList(list);
        //public static implicit operator RestArray<T>(JToken json)
        //    => FromList(json.Children());
        public static implicit operator RestArray<T, JType>(JArray json)
            => FromList(json);
        public static implicit operator RestArray<T, JType>(JsonArray json)
            => FromList(json);
    }
}