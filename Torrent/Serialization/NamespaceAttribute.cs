using System;
using System.Collections.Generic;
using System.Linq;

namespace Torrent.Serialization
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class NamespaceAttribute : Attribute
    {
        public const string Default = @"http://schemas.puchalapalli.com/2016/Torrent";
        public NamespaceAttribute()
        {
        }

        public NamespaceAttribute(string prefix, string uri)
        {
            Prefix = prefix;
            Uri = uri;
        }

        public string Prefix { get; set; }
        public string Uri { get; set; }
    }
}
