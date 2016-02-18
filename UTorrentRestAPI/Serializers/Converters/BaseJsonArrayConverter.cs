namespace UTorrentRestAPI.Serializers.Converters
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using DataModels;

    public class JsonLoadableArrayConverter<T> : JsonLoadableConverter<T> where T : IJsonLoadable, new()
    {
        public override object LoadFromJson(JsonReader reader)
        {
            JArray json = JArray.Load(reader);
            T newValue = new T();
            newValue.LoadFromJson(json);
            return newValue;
        }
    }
}
