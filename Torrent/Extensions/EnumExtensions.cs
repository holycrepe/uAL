using Torrent.Helpers.Utils;
namespace Torrent.Extensions
{
    public static partial class EnumExtensions
    {
        public static bool IsDupe(this FileSystemUtils.MoveFileResultStatus enumMember)
        {
        	return (enumMember == FileSystemUtils.MoveFileResultStatus.Dupe || enumMember == FileSystemUtils.MoveFileResultStatus.DupeError);
        }
        public static bool IsError(this FileSystemUtils.MoveFileResultStatus enumMember)
        {
        	return (enumMember == FileSystemUtils.MoveFileResultStatus.Error || enumMember == FileSystemUtils.MoveFileResultStatus.DupeError);
        }
    }
}
