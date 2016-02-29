namespace Torrent.Infrastructure.Reflection
{
    using System;
    using System.ComponentModel;
    using Extensions;

    [Flags]
    public enum MemberTypes
    {
        [Browsable(false)]
        None = 0,
        [Description("Properties & Fields")]
        All = -1,
        [Description("Properties")]
        Property = 1,
        [Description("Fields")]
        Field = 1 << 1,
    }
    public static class MemberTypeExtensions
    {
        public static bool IncludesProperties(this MemberTypes value)
            => value.Has(MemberTypes.Property);
        public static bool IncludesFields(this MemberTypes value)
            => value.Has(MemberTypes.Field);
    }
}