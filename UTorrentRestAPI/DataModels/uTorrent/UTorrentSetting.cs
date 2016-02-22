//-----------------------------------------------------------------------
// <copyright file="File.cs" company="Mike Davis">
//     To the extent possible under law, Mike Davis has waived all copyright and related or neighboring rights to this work.  This work is published from: United States.  See copying.txt for details.  I would appreciate credit when incorporating this work into other works.  However, you are under no legal obligation to do so.
// </copyright>
//-----------------------------------------------------------------------

namespace UTorrentRestAPI
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using Newtonsoft.Json;
    using RestSharp;
    using Torrent.Infrastructure;
    using Newtonsoft.Json.Linq;
    using DataModels;
    using Serializers.Converters;
    

    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    [JsonConverter(typeof (JsonLoadableArrayConverter<UTorrentSetting>))]
    public class UTorrentSetting : IJsonLoadable, IDebuggerDisplay
    {
        public UTorrentSetting() { }
        public UTorrentSetting(RestList json) { LoadFromJson(json); }

        public void LoadFromJson(RestList json)
        {
            var i = 0;
            Name = json[i++];
            Type = json[i++];
            Value = json[i++];
            if (Type == RestType.Bool) {
                Value.SetBool();
            }
            if (i + 1 >= json.Count) {
                return;
            }
            var extra = (JsonObject) json[i++];
            Extra = extra.ToDictionary(s => s.Key, s => new RestStruct(s.Value));
        }

        #region Public Properties

        public string Name { get; private set; }
        public RestType Type { get; private set; }
        public RestStruct Value { get; private set; }
        public Dictionary<string, RestStruct> Extra { get; private set; }
        public RestDict ExtraRD { get; }

        public string QualifiedName
            => $"{nameof(UTorrentSetting)}.{Name}";

        #endregion

        #region Debugger Display

        public string DebuggerDisplayValue(int level = 1)
            => Value.DebuggerDisplayValue(level);

        public string DebuggerDisplayValueSimple(int level = 1)
            => Value.DebuggerDisplayValueSimple(level);

        public string DebuggerDisplay(int level = 1)
            =>
                $"{QualifiedName}: <{Type}> {DebuggerDisplayValueSimple(level)}"
                + (Value.Original?.ToString() == Value.AsString ? "" : $" ({Value.Original})");

        public string DebuggerDisplaySimple(int level = 1)
            => $"{Name}: {DebuggerDisplayValueSimple(level)}";

        #endregion

        #region Operators

        public static implicit operator UTorrentSetting(RestList value)
            => new UTorrentSetting(value);

        public static implicit operator UTorrentSetting(JArray value)
            => new UTorrentSetting(value);

        #endregion
    }
}
