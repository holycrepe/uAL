using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Torrent.Infrastructure.Enums;

namespace Torrent.Extensions
{
    public static class EnumMembersExtensions
    {
        public static void SetDisplayFormat(this IEnumerable<EnumMember> list, EnumMemberDisplayFormat value = EnumMemberDisplayFormat.Combined)
            => list.ForEach(e => e.DisplayFormat = value);

        public static EnumMember GetAllMember(this IEnumerable<EnumMember> list)
            => list.FirstOrDefault(x => x.IsAll);
        public static EnumMember GetDisabledMember(this IEnumerable<EnumMember> list)
            => list.FirstOrDefault(x => x.IsDisabled);

        public static bool HasAllMember(this IEnumerable<EnumMember> list)
            => list.Any(x => x.IsAll);
        public static bool HasDisabledMember(this IEnumerable<EnumMember> list)
            => list.Any(x => x.IsDisabled);
        public static bool HasEnabledMember(this IEnumerable<EnumMember> list)
            => list.Any(x => !x.IsDisabled);
        public static bool HasMainMember(this IEnumerable<EnumMember> list)
            => list.Any(x => x.IsMain);
        public static bool HasMainMembersOnly(this IEnumerable<EnumMember> list)
            => list.All(x => x.IsMain);
    }
}
