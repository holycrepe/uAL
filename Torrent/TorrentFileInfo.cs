using System;
using System.Collections.Generic;

namespace Torrent
{
    using IO = System.IO;
    public struct TorrentFileInfo
    {
        public readonly long Length;
        public readonly long Offset;
        public readonly int FirstPiece;
        public readonly int LastPiece;
        public readonly List<string> Paths;
        public readonly string FullName;
        public readonly string Path;
        public readonly string Name;

        public TorrentFileInfo(string[] paths, long length, long currentLength, long pieceLength) : this(new List<string>(paths), length, currentLength, pieceLength) { }

        public TorrentFileInfo(string fileName, long length, long currentLength, long pieceLength) : this(new List<string>(fileName.Split('\\')), length, currentLength, pieceLength) { }

        public TorrentFileInfo(List<string> paths, long length, long currentLength, long pieceLength)
        {
            Length = length;
            Offset = currentLength;
            FirstPiece = (int) Math.Floor((double)Offset / pieceLength);
            LastPiece = (int)Math.Ceiling((double)(Offset+length) / pieceLength);
            Paths = paths;
            FullName = string.Join(IO.Path.DirectorySeparatorChar.ToString(), paths);
            Path = string.Join(IO.Path.DirectorySeparatorChar.ToString(), paths.GetRange(0, paths.Count - 1));
            Name = paths[paths.Count - 1];
        }
    }
}
