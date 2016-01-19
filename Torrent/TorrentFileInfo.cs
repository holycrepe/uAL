using System;
using System.Collections.Generic;

namespace Torrent
{
	public struct TorrentFileInfo {
			public readonly long Size;
			public readonly List<string> Paths;
			public readonly string FullName;
			public readonly string Path;
			public readonly string Name;

            public TorrentFileInfo(string[] paths, long size) : this(new List<string>(paths), size) {				
			}			
			
			public TorrentFileInfo(string fileName, long size) : this(new List<string>(fileName.Split('\\')), size) {				
			}

			public TorrentFileInfo(List<string> paths, long size) {
				this.Size = size;
				this.Paths= paths;
				this.FullName = string.Join(";", paths);
				this.Path = string.Join(";", paths.GetRange(0, paths.Count - 1));
				this.Name = paths[paths.Count-1];

            }			
		}
}
