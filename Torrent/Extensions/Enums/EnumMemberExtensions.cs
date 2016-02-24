using Torrent.Helpers.Utils;
using Torrent.Helpers.Utils.IO;

namespace Torrent.Extensions
{
    public static partial class EnumExtensions
    {
        public static bool IsDupe(this MoveFileResultStatus enumMember)
            => (enumMember == MoveFileResultStatus.Dupe
                    || enumMember == MoveFileResultStatus.DupeError);

        public static bool IsError(this MoveFileResultStatus enumMember)
            => (enumMember == MoveFileResultStatus.Error
                    || enumMember == MoveFileResultStatus.DupeError);
        public static bool IsError(this UTorrentJobMoveStatus enumMember)
            => enumMember.ToString().Contains("Error");
        public static bool IsUnneeded(this UTorrentJobMoveStatus enumMember)
            => enumMember == UTorrentJobMoveStatus.Unneeded || enumMember == UTorrentJobMoveStatus.AlreadyMoved;
        public static bool IsSuccess(this UTorrentJobMoveStatus enumMember)
            => enumMember == UTorrentJobMoveStatus.Success;
        public static bool IsSuccessful(this UTorrentJobMoveStatus enumMember)
            => enumMember.IsSuccess() || enumMember.IsUnneeded();
    }
}
