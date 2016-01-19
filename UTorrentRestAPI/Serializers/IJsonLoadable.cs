using Newtonsoft.Json.Linq;

namespace UTorrentRestAPI.DataModels
{
    public interface IJsonLoadable
    {
        void LoadFromJson(RestList json);
    }
    public interface IJsonLoadable<JType> where JType : JToken
    {
        void LoadFromJson(JType json);
    }
}
