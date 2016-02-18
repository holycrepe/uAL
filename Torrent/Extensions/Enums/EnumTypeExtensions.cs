namespace Torrent.Extensions
{
    using Infrastructure.Enums;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;
    using static Helpers.Utils.EnumUtils;
    using Exceptions;
    public static class EnumTypeExtensions
    {
        public static IEnumerable<EnumMember> GetEnumMembers(Type enumType)
            => EnumMember.GetEnumMembers(enumType);
        public static EnumMember[] GetEnumMemberArray(Type enumType)
            => EnumMember.GetEnumMemberArray(enumType);
    }
}
