//-----------------------------------------------------------------------
// <copyright file="FileCollection.cs" company="Mike Davis">
//     To the extent possible under law, Mike Davis has waived all copyright and related or neighboring rights to this work.  This work is published from: United States.  See copying.txt for details.  I would appreciate credit when incorporating this work into other works.  However, you are under no legal obligation to do so.
// </copyright>
//-----------------------------------------------------------------------

namespace UTorrentRestAPI
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using RestSharp;
    using Torrent.Infrastructure;
    using Newtonsoft.Json.Linq;
    using System.Xml.Serialization;
    using Newtonsoft.Json;
    using Serializers.Converters;
    using DataModels;
    #region Private Class: UTorrentLabelCollection 

    /// <summary>
    /// Holds a collection of uTorrent Settings
    /// </summary>
    [JsonConverter(typeof(JsonLoadableArrayConverter<UTorrentLabelCollection>))]
    public class UTorrentLabelCollection : MyKeyedCollection<string, UTorrentLabel>, IJsonLoadable
    {
        protected override Dictionary<Type, Func<object, string>> KeySelectors { get; }
            = new Dictionary<Type, Func<object, string>>()
              {
                  [typeof (UTorrentLabel)] = item => ((UTorrentLabel) item).Name
              };
        public void LoadFromJson(RestList json)
        {
            foreach (var label in json)
            {
                this.Add(new UTorrentLabel(label));
            }
        }
    }

    #endregion
}
