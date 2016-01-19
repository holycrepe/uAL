namespace UTorrentRestAPI.Extensions
{
    using System.IO;
    using RestSharp;

    public static class EnumExtensions
    {        
        public static string GetFormatString(this RestType type) 
        {
        	switch (type) {
        		case RestType.String:
        			return "\"{0}\"";        		
        		case RestType.Long:
        			return "L{0}";
        		case RestType.Bool:	
        			return "!{0}";
        		case RestType.Unknown:
        			return "? {0}";
        		default:
        			return "{0}";
        	}
        }
        public static bool IsEnumerable(this RestType type)
        {
            switch (type)
            {
                case RestType.List:
                case RestType.JArray:
                case RestType.JsonArray:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsDictionary(this RestType type)
            => type == RestType.Dict || type == RestType.JObject;
        public static bool IsBool(this RestType type)
            => type == RestType.Bool;
    }
}