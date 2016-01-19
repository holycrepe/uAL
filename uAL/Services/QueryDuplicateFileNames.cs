using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using Torrent.Helpers.Utils;
using Torrent.Extensions;

namespace uAL.Services
{
    public class QueryDuplicateFileNames
	{
		DirectoryInfo startFolder;
		public FileInfo Match = null;
		public bool Result;
		public string Source;
		public string RootLabel;
		public DirectoryInfo SearchDirectory;
		readonly List<string> rootLabelsToCheck;
		static Logger logger = LogManager.GetLogger("QueryDuplicateFileNames");
		public string MatchBase;
		
		public QueryDuplicateFileNames(string startFolder, string[] rootLabelsToCheck = null)
		{
			this.startFolder = new DirectoryInfo(startFolder);
			this.rootLabelsToCheck = new List<string>(rootLabelsToCheck);
		}
		
		string isCheckableLabel(string label)
		{
			if (label == null || rootLabelsToCheck == null) {
				return "";
			}
			var labelParts = new List<string>(label.Split('\\'));
			for (var i = labelParts.Count - 1; i >= 0; i--) {
				var parentLabel = string.Join("\\", labelParts.GetRange(0, i));
				if (rootLabelsToCheck.Contains(parentLabel)) {
					return parentLabel;
				}
			}
			foreach (string rootLabel in rootLabelsToCheck) {
				if (label.StartsWith(rootLabel)) {
					return rootLabel;
				}
			}
			return null;
		}
		public bool ReportMatch(string fileName) {
			return fileName != null && ReportMatch(new FileInfo(fileName));
		}
		public bool ReportMatch(FileInfo fi) {
			if (fi == null) {
				return false;
			}
			Match = fi;
			MatchBase = Match.FullName.Replace(SearchDirectory.FullName, "");
			logger.WARN("      >>>>>>    ******** Dupe Found At: {0}\n", MatchBase);
			return Result = true;
		}
		public bool QueryDuplicates(string fileName, long length = 0, string label = null)
		{
			Match = null;
			MatchBase = null;  
			RootLabel = isCheckableLabel(label);
			if (RootLabel == null) {
				return false;
			}
			
			SearchDirectory = (RootLabel == "" ? startFolder : new DirectoryInfo(Path.Combine(startFolder.FullName, RootLabel)));
			Source = PathUtils.GetFileName(fileName);
			logger.TRACE("Searching {0,-25} For `{1}`", (RootLabel == "" ? "Root Directory" : "Label `" + RootLabel + "`"), this.Source);
			return length == 0 ? ReportMatch(FileSystemSearchUtils.FindFileName(SearchDirectory, Source)) : 
								 ReportMatch(FileSystemSearchUtils.FindFile(SearchDirectory, Source, fi => fi.Length == length));
		}
	}
}
