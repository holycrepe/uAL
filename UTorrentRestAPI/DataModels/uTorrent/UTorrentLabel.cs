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
    [JsonConverter(typeof (JsonLoadableArrayConverter<UTorrentLabel>))]
    public class UTorrentLabel : IJsonLoadable, IDebuggerDisplay
    {
        public UTorrentLabel() { }
        public UTorrentLabel(RestList json) { LoadFromJson(json); }

        public void LoadFromJson(RestList json)
        {
            var i = 0;
            Name = json[i++];
            Count = json[i++];
        }

        #region Public Properties

        public string Name { get; private set; }
        public int Count { get; private set; }

        public string QualifiedName
            => $"{nameof(UTorrentLabel)}.{Name}";

        #endregion

        #region Debugger Display

        public string DebuggerDisplayValue(int level = 1)
            => $"{Name}: {Count}";

        public string DebuggerDisplayValueSimple(int level = 1)
            => this.DebuggerDisplayValue();

        public string DebuggerDisplay(int level = 1)
            =>
                $"{QualifiedName}: <{Count}>";

        public string DebuggerDisplaySimple(int level = 1)
            => $"{Name} [{Count}]";

        #endregion

        #region Operators

        public static implicit operator UTorrentLabel(RestList value)
            => new UTorrentLabel(value);

        public static implicit operator UTorrentLabel(JArray value)
            => new UTorrentLabel(value);

        #endregion
    }
}
