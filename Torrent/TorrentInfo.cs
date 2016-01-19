using System;
using BencodeNET;
using System.Collections.Generic;
using BencodeNET.Objects;
using System.IO;

namespace Torrent
{
	/// <summary>
	/// Torrent Info.
	/// </summary>
	public class TorrentInfo : Torrent
	{
	    readonly List<TorrentFileInfo> files;
		public readonly TorrentFileInfo Largest;
		public readonly TorrentFile Torrent;
        public readonly bool HasFileList;
        public readonly bool success = false;
        public readonly Exception error = null;
        public readonly bool isBDecodeError = false;
        public readonly string Comment;
        readonly Uri commentLink;
        public Uri CommentLink => commentLink;
	    public readonly bool HasCommentLink;
		
		
		public TorrentInfo(string filename)
		{
			FileName = filename;
			Torrent = null;			
			
			try {
				Torrent = Bencode.DecodeTorrentFile(FileName);
			} catch (Exception ex) {
				error = ex;
				isBDecodeError = (ex.GetType().Name.StartsWith("BencodeDecodingException", StringComparison.CurrentCulture));
				return;
			}
			

			// Calculate info hash (e.g. "B415C913643E5FF49FE37D304BBB5E6E11AD5101")
			this.Hash = Torrent.CalculateInfoHash();	

			this.Comment = Torrent.Comment;
			
			this.HasCommentLink = Uri.TryCreate(Comment, UriKind.Absolute, out commentLink);
			
			// Get name and size of each file in 'files' list of 'info' dictionary ("multi-file mode")
			this.files = new List<TorrentFileInfo>();			
			
			this.Name = ((BString)Torrent.Info["name"]).ToString();
			if (Torrent.Info.ContainsKey("length")) {
				long size = (BNumber) Torrent.Info["length"];
				var tfi = new TorrentFileInfo(this.Name, size);
			    if (this.files.Count == 0 || tfi.Size > this.Largest.Size) {
			    	this.Largest = tfi;
			    }
				this.files.Add(tfi);
			}
						
			var bFiles = (BList)Torrent.Info["files"];
            HasFileList = (bFiles != null);
			if (HasFileList) {
				foreach (BDictionary file in bFiles)
				{
					
					long size = (BNumber) file["length"];			    
				    var bPaths = (BList) file["path"];
				    var paths = new List<string>();
				    
				    foreach (var bPath in bPaths) {
				    	paths.Add(((BString)bPath).ToString());
				    }			    
				    var tfi = new TorrentFileInfo(paths, size);			    			    
				    if (this.files.Count == 0 || tfi.Size > this.Largest.Size) {
				    	this.Largest = tfi;
				    }
				    this.files.Add(tfi);
				}
			}
			
			if (this.files.Count == 0) {
				            	            	
			}
            
            success = true;
		}

        public byte[] GetBytes()
        {
            Stream TorrentStream = File.OpenRead(FileName);
            var FileBytes = new byte[TorrentStream.Length];
            TorrentStream.Read(FileBytes, 0, FileBytes.Length);
            return FileBytes;
        }
	}
}
