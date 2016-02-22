#region License

//   Copyright 2010 John Sheehan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 

#endregion

#region Acknowledgements

// Original JsonSerializer contributed by Daniel Crenna (@dimebrain)

#endregion

using System.IO;
using Newtonsoft.Json;

namespace UTorrentRestAPI.Serializers
{
    using RestSharp;
    using RestSharp.Deserializers;

    /// <summary>
    /// Default JSON serializer for request bodies
    /// Doesn't currently use the SerializeAs attribute, defers to Newtonsoft's attributes
    /// </summary>
    public class JsonDeserializer1 : IDeserializer
    {
        /// <summary>
        /// Default serializer
        /// </summary>
        public JsonDeserializer1()
        {
            ContentType = "application/json";
            //_deserializer = new Newtonsoft.Json.JsonDeserializer
            //{
            //    MissingMemberHandling = MissingMemberHandling.Ignore,
            //    NullValueHandling = NullValueHandling.Include,
            //    DefaultValueHandling = DefaultValueHandling.Include
            //};
        }

        /// <summary>
        /// Default serializer with overload for allowing custom Json.NET settings
        /// </summary>
        //public JsonDeserializer(Newtonsoft.Json.JsonDeserializer deserializer)
        //{
        //    ContentType = "application/json";
        //    _deserializer = deserializer;
        //}
        /// <summary>
        /// Serialize the object as JSON
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>JSON as String</returns>
        public T Deserialize<T>(IRestResponse response) => JsonConvert.DeserializeObject<T>(response.Content);

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string DateFormat { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string RootElement { get; set; }

        /// <summary>
        /// Unused for JSON Serialization
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Content type for serialized content
        /// </summary>
        public string ContentType { get; set; }
    }
}

namespace UTorrentRestAPI.Deserializers
{
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using RestSharp.Deserializers;
    using RestSharp.Extensions;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Xml;

    public class JsonNetDeserializer : IDeserializer
    {
        public JsonNetDeserializer() { this.Culture = CultureInfo.InvariantCulture; }

        private IDictionary BuildDictionary(Type type, object parent)
        {
            IDictionary dictionary = (IDictionary) Activator.CreateInstance(type);
            Type conversionType = type.GetGenericArguments()[0];
            Type type3 = type.GetGenericArguments()[1];
            foreach (KeyValuePair<string, object> pair in (IDictionary<string, object>) parent) {
                object obj3;
                object key = (conversionType != typeof (string))
                                 ? Convert.ChangeType(pair.Key, conversionType, CultureInfo.InvariantCulture)
                                 : pair.Key;
                if (type3.IsGenericType && (type3.GetGenericTypeDefinition() == typeof (List<>))) {
                    obj3 = this.BuildList(type3, pair.Value);
                } else {
                    obj3 = this.ConvertValue(type3, pair.Value);
                }
                dictionary.Add(key, obj3);
            }
            return dictionary;
        }

        private IList BuildList(Type type, object parent)
        {
            IList list = (IList) Activator.CreateInstance(type);
            Type type3 =
                type.GetInterfaces()
                    .First<Type>(x => (x.IsGenericType && (x.GetGenericTypeDefinition() == typeof (IList<>))))
                    .GetGenericArguments()[0];
            if (parent is IList) {
                foreach (object obj2 in (IList) parent) {
                    if (type3.IsPrimitive) {
                        object obj3 = this.ConvertValue(type3, obj2);
                        list.Add(obj3);
                    } else if (type3 == typeof (string)) {
                        if (obj2 == null) {
                            list.Add(null);
                        } else {
                            list.Add(obj2.ToString());
                        }
                    } else if (obj2 == null) {
                        list.Add(null);
                    } else {
                        object obj4 = this.ConvertValue(type3, obj2);
                        list.Add(obj4);
                    }
                }
                return list;
            }
            list.Add(this.ConvertValue(type3, parent));
            return list;
        }

        private object ConvertValue(Type type, object value)
        {
            string str = Convert.ToString(value, this.Culture);
            if (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof (Nullable<>))) {
                if (string.IsNullOrEmpty(str)) {
                    return null;
                }
                type = type.GetGenericArguments()[0];
            }
            if (type == typeof (object)) {
                if (value == null) {
                    return null;
                }
                type = value.GetType();
            }
            if (type.IsPrimitive) {
                return value.ChangeType(type, this.Culture);
            }
            if (type.IsEnum) {
                return type.FindEnumValue(str, this.Culture);
            }
            if (type == typeof (Uri)) {
                return new Uri(str, UriKind.RelativeOrAbsolute);
            }
            if (type == typeof (string)) {
                return str;
            }
            if ((type == typeof (DateTime)) || (type == typeof (DateTimeOffset))) {
                DateTime time;
                if (this.DateFormat.HasValue()) {
                    time = DateTime.ParseExact(str, this.DateFormat, this.Culture,
                                               DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
                } else {
                    time = str.ParseJsonDate(this.Culture);
                }
                if (type == typeof (DateTime)) {
                    return time;
                }
                if (!(type == typeof (DateTimeOffset))) {
                    return null;
                }
                return (DateTimeOffset) time;
            }
            if (type == typeof (decimal)) {
                if (value is double) {
                    return (decimal) ((double) value);
                }
                if (str.Contains("e")) {
                    return decimal.Parse(str, NumberStyles.Float, this.Culture);
                }
                return decimal.Parse(str, this.Culture);
            }
            if (type == typeof (Guid)) {
                return (string.IsNullOrEmpty(str) ? Guid.Empty : new Guid(str));
            }
            if (type == typeof (TimeSpan)) {
                TimeSpan span;
                if (TimeSpan.TryParse(str, out span)) {
                    return span;
                }
                return XmlConvert.ToTimeSpan(str);
            }
            if (type.IsGenericType) {
                Type genericTypeDefinition = type.GetGenericTypeDefinition();
                if (genericTypeDefinition == typeof (List<>)) {
                    return this.BuildList(type, value);
                }
                if (genericTypeDefinition == typeof (Dictionary<,>)) {
                    return this.BuildDictionary(type, value);
                }
                return this.CreateAndMap(type, value);
            }
            if (type.IsSubclassOfRawGeneric(typeof (List<>))) {
                return this.BuildList(type, value);
            }
            if (type == typeof (JsonObject)) {
                return this.BuildDictionary(typeof (Dictionary<string, object>), value);
            }
            return this.CreateAndMap(type, value);
        }

        private object CreateAndMap(Type type, object element)
        {
            object target = Activator.CreateInstance(type);
            this.Map(target, (IDictionary<string, object>) element);
            return target;
        }

        public T Deserialize<T>(IRestResponse response)
            =>
                this.RootElement.HasValue()
                    ? Deserialize<T>(FindRoot(response.Content))
                    : Deserialize<T>(response.Content);

        private T Deserialize<T>(string json)
            => JsonConvert.DeserializeObject<T>(json);

        private T Deserialize<T>(JToken rootElement)
            => Deserialize<T>(rootElement.ToString());

        private JToken FindRoot(string content)
        {
            JToken current = JObject.Parse(content);
            foreach (var key in this.RootElement.Split('.')) {
                try {
                    current = current[key];
                } catch (Exception) {
                    throw new KeyNotFoundException();
                }
            }
            return current;
        }

        private object Map(object target, IDictionary<string, object> data)
        {
            foreach (PropertyInfo info in (from p in target.GetType().GetProperties()
                                           where p.CanWrite
                                           select p).ToList<PropertyInfo>()) {
                string name;
                Type propertyType = info.PropertyType;
                object[] customAttributes = info.GetCustomAttributes(typeof (DeserializeAsAttribute), false);
                if (customAttributes.Length > 0) {
                    DeserializeAsAttribute attribute = (DeserializeAsAttribute) customAttributes[0];
                    name = attribute.Name;
                } else {
                    name = info.Name;
                }
                string[] strArray = name.Split(new char[] {'.'});
                IDictionary<string, object> dictionary = data;
                object obj2 = null;
                for (int i = 0; i < strArray.Length; i++) {
                    string str2 =
                        strArray[i].GetNameVariants(this.Culture)
                                   .FirstOrDefault<string>(new Func<string, bool>(dictionary.ContainsKey));
                    if (str2 == null) {
                        break;
                    }
                    if (i == (strArray.Length - 1)) {
                        obj2 = dictionary[str2];
                    } else {
                        dictionary = (IDictionary<string, object>) dictionary[str2];
                    }
                }
                if (obj2 != null) {
                    info.SetValue(target, this.ConvertValue(propertyType, obj2), null);
                }
            }
            return target;
        }

        public CultureInfo Culture { get; set; }

        public string DateFormat { get; set; }

        public string Namespace { get; set; }

        public string RootElement { get; set; }
    }
}
