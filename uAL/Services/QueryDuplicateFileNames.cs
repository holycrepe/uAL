using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using Torrent.Helpers.Utils;
using Torrent.Extensions;
using Torrent.Infrastructure.FileSystem;
using System.Linq;

namespace uAL.Services
{
    using Properties.Settings.LibSettings;
    using static Properties.Settings.LibSettings.LibSettings;
    public struct QueryDuplicateFileNamesResult
    {
        public bool Matched { get; }
        public FileInfo Dupe { get; }
        public QueryDuplicateFileNamesResult(bool matched=false, FileInfo dupe=null)
        {
            Matched = matched;
            Dupe = dupe;
        }
        public QueryDuplicateFileNamesResult(FileInfo dupe) : this(true, dupe) { }
        public static implicit operator bool(QueryDuplicateFileNamesResult value)
            => value.Matched;
    }
    public class QueryDuplicateFileNames
    {
        public FileInfo Match = null;
        public bool Result;
        public string StartFolder;
        public string SearchPattern;
        public string RootLabel;
        public DirectoryInfo SearchDirectory;
        public RootDirectoryInfo Roots;
        public List<string> SearchDirectories = new List<string>();
        static Logger logger = LogManager.GetLogger(nameof(QueryDuplicateFileNames));
        public string MatchBase;

        public QueryDuplicateFileNames(string startFolder)
        {
            this.StartFolder = startFolder;
            UpdateRoots();
            LibSetting.Labels.PropertyChanged += (s, e) => UpdateRoots();
        }

        protected void UpdateRoots() { Roots = LibSetting.Labels.GetRootDirectoryInfo(this.StartFolder); }

        string isCheckableLabel(string label)
        {
            if (label == null || Roots.Labels == null) {
                return "";
            }
            var labelParts = new List<string>(label.Split('\\'));
            for (var i = labelParts.Count - 1; i >= 0; i--) {
                var parentLabel = string.Join("\\", labelParts.GetRange(0, i));
                if (Roots.Labels.Contains(parentLabel)) {
                    return parentLabel;
                }
            }
            string matchingLabel = null;
            foreach (string rootLabel in Roots.Labels) {
                if (label.StartsWith(rootLabel) && (matchingLabel == null || rootLabel.Length > matchingLabel.Length)) {
                    matchingLabel = rootLabel;
                }
            }
            return matchingLabel;
        }

        public QueryDuplicateFileNamesResult ReportMatch(string fileName)
            => (string.IsNullOrEmpty(fileName) ? new QueryDuplicateFileNamesResult() : ReportMatch(FileUtils.GetInfo(fileName)));

        public QueryDuplicateFileNamesResult ReportMatch(FileInfo fi)
        {
            if (fi == null) {
                return new QueryDuplicateFileNamesResult();
            }
            Match = fi;
            MatchBase = Match.FullName.Replace(SearchDirectory.FullName, "");
            logger.WARN("      >>>>>>    ******** Dupe Found At: {0}", MatchBase);
            Result = true;
            return new QueryDuplicateFileNamesResult(fi);
        }

        public QueryDuplicateFileNamesResult QueryDuplicates(string fileName, long length = 0, string label = null)
        {
            Match = null;
            MatchBase = null;
            RootLabel = isCheckableLabel(label);
            if (RootLabel == null) {
                return new QueryDuplicateFileNamesResult();
            }
            string extension = Path.GetExtension(fileName);
            SearchDirectory = (RootLabel == ""
                                   ? Roots.Folder
                                   : new DirectoryInfo(Path.Combine(Roots.Folder.FullName, RootLabel)));
            SearchDirectories.Clear();
            foreach (var directory in Roots.Directories) {
                if (directory.StartsWith(SearchDirectory.FullName)) {
                    SearchDirectories.Add(directory);
                }
            }
            SearchPattern = PathUtils.GetFileName(fileName);
#if TRACE_EXT
			logger.TRACE("Searching {0,-25} For `{1}`", (RootLabel == "" ? "Root Directory" : "Label `" + RootLabel + "`"), this.Source);
#endif
            return (length == 0
                ? ReportMatch(QueryDuplicateFileNamesCache.FindFileName(Roots, SearchPattern))
                : ReportMatch(QueryDuplicateFileNamesCache.FindFile(Roots, SearchPattern, fi => fi.Length == length))
                );
        }
    }
}
