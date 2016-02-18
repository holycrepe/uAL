namespace Torrent.Extensions
{
    using System.Diagnostics.CodeAnalysis;
    using Helpers.Utils;

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public static partial class Extensions
    {
        public static bool IsForced(this Flag value) { return value == Flag.ForceOn || value == Flag.ForceOff; }
        public static bool Value(this Flag value) { return value == Flag.ForceOn || value == Flag.On; }

        public static bool Value(this Flag value, params Flag[] flags)
        {
            var all = true;
            foreach (var flag in flags) {
                if (flag.IsForced()) {
                    return flag.Value();
                }
                all &= flag.Value();
            }
            if (value.IsForced()) {
                return value.Value();
            }
            return all && value.Value();
        }
    }
}
