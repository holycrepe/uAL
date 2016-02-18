using System.Collections.Generic;
using System.IO;

namespace Torrent.Infrastructure.FileSystem
{
    public class RootDirectoryInfo
    {
        public readonly DirectoryInfo Folder;
        public readonly string[] Directories;
        public readonly string[] Labels;
        public readonly bool Valid;

        public RootDirectoryInfo(string startFolder, string[] rootLabels)
        {            
            if (string.IsNullOrEmpty(startFolder))
            {
                Folder = null;
                Directories = Labels = null;
                Valid = false;
                return;
            }
            Valid = true;
            Folder = new DirectoryInfo(startFolder);
            List<string> directories = new List<string>();
            List<string> labels = new List<string>();
            for (var i = 0; i < rootLabels.Length; i++) {
                var label = rootLabels[i];
                if (label.StartsWith("*")) {
                    labels.Add(label.Substring(1));
                } else {
                    directories.Add(Path.Combine(startFolder, label));
                }
            }
            directories.Sort();
            Directories = directories.ToArray();
            labels.Sort();
            Labels = labels.ToArray();
        }
        public override bool Equals(object obj)
        {
            var cast = obj as RootDirectoryInfo;
            if (cast?.GetHashCode() == this.GetHashCode())
            {
                return true;
            }
            return false;
        }
        public override int GetHashCode()
            => Folder.FullName.ToLowerInvariant().GetHashCode() ^ string.Join("|", Directories).ToLowerInvariant().GetHashCode() ^ string.Join("|", Labels).ToLowerInvariant().GetHashCode();
    }
}
