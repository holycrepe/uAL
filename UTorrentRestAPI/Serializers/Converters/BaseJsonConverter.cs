namespace UTorrentRestAPI.Serializers.Converters
{
    using System;
    using Newtonsoft.Json;
    using DataModels;

    public abstract class JsonLoadableConverter<T> : JsonConverter where T : IJsonLoadable, new()
    {
        public override bool CanConvert(Type objectType) { return (objectType == typeof (T)); }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                                        JsonSerializer serializer)
        {
            return LoadFromJson(reader);
        }

        public abstract object LoadFromJson(JsonReader reader);

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
