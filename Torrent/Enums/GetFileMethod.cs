using System;
using System.ComponentModel;

namespace Torrent.Enums
{
    using Converters;
    [Serializable]
    //[TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum GetFileMethod
    {
        [Description("Use Default Method")]
        Default,
        [Description("Use Array Methods")]
        Plain,
        [Description("Use Enumerate()")]
        Enumerator,
        [Description("Use PLINQ")]
        Linq
    }
}
