using System.Collections.Generic;
using System.IO;

namespace Torrent.Infrastructure.FileSystem
{
    public class FileSearchResult<T>
    {
        public readonly RootDirectoryInfo Roots;
        public readonly T[] Files;
        public FileSearchResult(RootDirectoryInfo roots, T[] files)
        {
            Roots = roots;
            Files = files;
        }
    }

    public class FileSearchOptions 
    {
        public readonly RootDirectoryInfo Roots;
        public readonly string SearchPattern;

        public FileSearchOptions(RootDirectoryInfo roots, string searchPattern)
        {
            Roots = roots;
            SearchPattern = searchPattern.ToLower();
        }
        public override bool Equals(object obj)
        {
            var cast = obj as FileSearchOptions;
            if (cast?.GetHashCode() == this.GetHashCode())
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
            => Roots.GetHashCode() ^ SearchPattern.ToLowerInvariant().GetHashCode();
    }
}
