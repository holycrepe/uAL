using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Torrent.Extensions
{
    using System.Diagnostics.CodeAnalysis;
    using Helpers.StringHelpers;
    using PostSharp.Patterns.Model;
    public static class ByteExtensions
    {
        public static string ToHex(this byte[] bytes)
            => BitConverter.ToString(bytes).Replace("-", string.Empty);
    }
}
