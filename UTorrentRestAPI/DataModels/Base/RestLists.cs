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
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class RestNestedList : List<RestList>, IDebuggerDisplay
    {
        public string DebuggerDisplay(int level = 1)
            => this.GetDebuggerDisplay(level);
        public string DebuggerDisplaySimple(int level = 1)
            => this.GetDebuggerDisplaySimple(level);
    }

    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class RestList : RestList<RestStruct>
    {
        //public string DebuggerDisplay()
        //    => this.GetDebuggerDisplay();
        //public string DebuggerDisplaySimple()
        //    => this.GetDebuggerDisplaySimple();

        //public static RestList FromList(IEnumerable<RestStruct> list)
        //{
        //    var newList = new RestList();
        //    newList.AddRange(list);
        //    return newList;
        //} 
        
        public RestList() : base() {}
        public RestList(IEnumerable<RestStruct> list) : base() {
            if (list != null)
            {
                AddRange(list);
            }
        }
        public RestList(IEnumerable<JToken> list) : base()
        {
            if (list != null)
            {
                AddRange(list.Select(item => new RestStruct(item)));
            }
        }
        public RestList(IEnumerable<object> list) : base() {
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
        public new static RestList FromList(IEnumerable<object> list)
            => new RestList(list);
        public new static RestList FromList(IEnumerable<JToken> list)
            => new RestList(list);
        public new static RestList FromList(IEnumerable<RestStruct> list)
        	=> new RestList(list);
        public static implicit operator RestList(List<object> list)
            => FromList(list);
        public static implicit operator RestList(object[] list)
            => FromList(list);
        public static implicit operator RestList(List<JToken> list)
            => FromList(list);
        public static implicit operator RestList(JToken[] list)
            => FromList(list);
        public static implicit operator RestList(RestStruct[] list)
            => FromList(list);
        //public static implicit operator RestList(JToken json)
        //    => FromList(json.Children());
        public static implicit operator RestList(JArray json)
            => FromList(json);
        public static implicit operator RestList(JsonArray json)
            => FromList(json);
    }

    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class RestList<T> : List<T>, IDebuggerDisplay where T: IDebuggerDisplay, new()
    {
        public string DebuggerDisplay(int level = 1)
            => this.GetDebuggerDisplay(level);
        public string DebuggerDisplaySimple(int level = 1)
            => this.GetDebuggerDisplaySimple(level);
 
        
        public RestList() : base() {}
        public RestList(IEnumerable<T> list) {
        	AddRange(list);
        }
        public RestList(IEnumerable<JToken> list)
        {            
            foreach(JToken item in list)
            {
                T t = JsonConvert.DeserializeObject<T>(item.ToString());
                Add(t);
            }
        }
        public RestList(IEnumerable<object> list) {
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
        public static RestList<T> FromList(IEnumerable<object> list)
            => new RestList<T>(list);
        public static RestList<T> FromList(IEnumerable<JToken> list)
            => new RestList<T>(list);
        public static RestList<T> FromList(IEnumerable<T> list)
        	=> new RestList<T>(list);
        public static implicit operator RestList<T>(List<JToken> list)
            => FromList(list);
        public static implicit operator RestList<T>(List<object> list)
            => FromList(list);
        public static implicit operator RestList<T>(object[] list)
            => FromList(list);    
        public static implicit operator RestList<T>(T[] list)
            => FromList(list);
        //public static implicit operator RestList<T>(JToken json)
        //    => FromList(json.Children());
        public static implicit operator RestList<T>(JArray json)
            => FromList(json);
        public static implicit operator RestList<T>(JsonArray json)
            => FromList(json);
    }
}