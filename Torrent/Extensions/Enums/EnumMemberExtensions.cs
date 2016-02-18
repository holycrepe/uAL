using Torrent.Helpers.Utils;

namespace Torrent.Extensions
{
    public static partial class EnumExtensions
    {
        public static bool IsDupe(this FileSystemUtils.MoveFileResultStatus enumMember)
            => (enumMember == FileSystemUtils.MoveFileResultStatus.Dupe
                    || enumMember == FileSystemUtils.MoveFileResultStatus.DupeError);

        public static bool IsError(this FileSystemUtils.MoveFileResultStatus enumMember)
            => (enumMember == FileSystemUtils.MoveFileResultStatus.Error
                    || enumMember == FileSystemUtils.MoveFileResultStatus.DupeError);
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
