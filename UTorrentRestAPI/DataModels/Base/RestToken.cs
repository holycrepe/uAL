namespace UTorrentRestAPI
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using RestSharp;
    using Torrent.Infrastructure;
    using Extensions;
    using Newtonsoft.Json.Linq;
	public interface IRestJsonItem<JType> where JType:JToken { }
    /// <summary>
    /// Struct That Automatically Casts A String To/From A Primitive Value
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay()}")]
    public abstract partial class RestToken : JToken, IRestJsonItem<JToken>, IDebuggerDisplay
    {

        public abstract string DebuggerDisplayValue(int level = 1);

        public virtual string DebuggerDisplayValueSimple(int level = 1)
            => DebuggerDisplayValue(level);

        public abstract string QualifiedName { get; }
        public virtual string DebuggerDisplaySimple(int level = 1)
        	=> DebuggerDisplayValueSimple(level);
        public virtual string DebuggerDisplay(int level = 1)
            => $"{QualifiedName}: {DebuggerDisplayValue(level)}";
    }
}