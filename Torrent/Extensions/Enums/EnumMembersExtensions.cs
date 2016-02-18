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
        public static void SetUseCombined(this IEnumerable<EnumMember> list, bool value)
            => list.ForEach(e => e.UseCombinedFormat = value);
    }
}
