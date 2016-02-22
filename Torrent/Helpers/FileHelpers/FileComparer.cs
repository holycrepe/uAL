using System.IO;

namespace Torrent.Helpers.Utils
{
    public class FileComparer
    {
        public readonly FileInfo Info = null;
        readonly byte[] original = null;

        public FileComparer(byte[] contents)
        {
            Info = null;
            original = contents;
        }


        public FileComparer(FileInfo fileInfo) : this(File.ReadAllBytes(fileInfo.FullName)) { Info = fileInfo; }

        public FileComparer(string filename) : this(FileUtils.GetInfo(filename)) { }

        public bool FileEquals(byte[] compare)
        {
            if (original.Length == compare.Length) {
                for (var i = 0; i < original.Length; i++) {
                    if (original[i] != compare[i]) {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        public bool FileEquals(string path) => FileEquals(File.ReadAllBytes(path));
    }
}
