using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using Torrent.Helpers.Utils;

namespace Torrent.Helpers.Utils
{
    using Enums;

    public static class FileSystemSearchUtils
	{
		#region Helper Classes
		class GetFileNamesHelper
		{
			readonly string SearchPattern;
			readonly List<string> _files = new List<string>();
			readonly object lockThis = new object();

			public GetFileNamesHelper(string searchPattern) {
				SearchPattern = searchPattern;
			}
			
			public List<string> GetFileList(string rootFolderPath)
			{				
				AddFileList(rootFolderPath);
				return _files;
			}

			void AddFileList(string directory)
			{
				var dInfo = new DirectoryInfo(directory);
				var files = Directory.GetFiles(directory, SearchPattern);
				lock (lockThis) {
					_files.AddRange(files);
				}

				var directories = Directory.GetDirectories(directory);
				Parallel.ForEach(directories, AddFileList);
			}
		}
		
		class GetFilesHelper
		{
			readonly string SearchPattern;
			readonly List<FileInfo> _files = new List<FileInfo>();
			readonly object lockThis = new object();

			public GetFilesHelper(string searchPattern) {
				SearchPattern = searchPattern;
			}
			
			public List<FileInfo> GetFileList(string rootFolderPath)
			{				
				AddFileList(rootFolderPath);
				return _files;
			}

			void AddFileList(string directory)
			{
				var dInfo = new DirectoryInfo(directory);
				var files = dInfo.GetFiles(SearchPattern);
				lock (lockThis) {
					_files.AddRange(files);
				}

				var directories = Directory.GetDirectories(directory);
				Parallel.ForEach(directories, AddFileList);
			}
		}
		
		class GetFileNamesEnumeratorHelper
		{
			readonly string SearchPattern;
			IEnumerable<string> _files;
			readonly object lockThis = new object();
			
			public GetFileNamesEnumeratorHelper(string searchPattern) {
				SearchPattern = searchPattern;
			}

			public IEnumerable<string> GetFileList(string rootFolderPath)
			{				
				AddFileList(rootFolderPath);
				return _files;
			}
			void AddFileList(string directory)
			{
				var files = Directory.EnumerateFiles(directory, SearchPattern, SearchOption.TopDirectoryOnly);
				lock (lockThis) {
					_files = _files == null ? files : _files.Concat(files);
				}

				var directories = Directory.EnumerateDirectories(directory);
				Parallel.ForEach(directories, AddFileList);
			}
		}
		
		class GetFilesEnumeratorHelper
		{
			readonly string SearchPattern;
			IEnumerable<FileInfo> _files;
			readonly object lockThis = new object();
			
			public GetFilesEnumeratorHelper(string searchPattern) {
				SearchPattern = searchPattern;
			}

			public IEnumerable<FileInfo> GetFileList(string rootFolderPath)
			{				
				AddFileList(rootFolderPath);
				return _files;
			}
			void AddFileList(string folderPath) { AddFileList(new DirectoryInfo(folderPath)); }
			void AddFileList(DirectoryInfo directory)
			{
				var files = directory.EnumerateFiles(SearchPattern, SearchOption.TopDirectoryOnly);
				lock (lockThis) {
					_files = _files == null ? files : _files.Concat(files);
				}

				var directories = directory.EnumerateDirectories();
				Parallel.ForEach(directories, AddFileList);
			}
		}
		
		class GetFileNamesEnumeratorLinqHelper
		{
			readonly string SearchPattern;
			readonly object lockThis = new object();
			
			public GetFileNamesEnumeratorLinqHelper(string searchPattern) {
				SearchPattern = searchPattern;
			}

			public IEnumerable<string> GetFileList(string rootFolderPath)
			{				
				var directory = new DirectoryInfo(rootFolderPath);
				return Directory.EnumerateFiles(rootFolderPath, SearchPattern)
								.Union(Directory.EnumerateDirectories(rootFolderPath, "*", SearchOption.AllDirectories).AsParallel()
					      		//.WithDegreeOfParallelism(Environment.ProcessorCount * 2)
					      		.SelectMany(d => Directory.EnumerateFiles(d, SearchPattern)));				
			}
		}
		
		class GetFilesEnumeratorLinqHelper
		{
			readonly string SearchPattern;
			readonly object lockThis = new object();
			
			public GetFilesEnumeratorLinqHelper(string searchPattern) {
				SearchPattern = searchPattern;
			}

			public IEnumerable<FileInfo> GetFileList(string rootFolderPath)
			{				
				var directory = new DirectoryInfo(rootFolderPath);
				return directory.EnumerateFiles(SearchPattern)
								.Union(directory.EnumerateDirectories("*", SearchOption.AllDirectories).AsParallel()
					      		//.WithDegreeOfParallelism(Environment.ProcessorCount * 2)
					      		.SelectMany(di => di.EnumerateFiles(SearchPattern)));				
			}
		}
		#endregion
		#region Get Files List
		public static IEnumerable<string> GetFileNames(string rootDirectory, string searchPattern, GetFileMethod method = GetFileMethod.Default) {
			switch (method.Value()) {
				case GetFileMethod.Plain:
					return GetFileNamesPlain(rootDirectory, searchPattern);
				case GetFileMethod.Enumerator:	
					return GetFileNamesEnumerator(rootDirectory, searchPattern);
				default:
					return GetFileNamesEnumeratorLinq(rootDirectory, searchPattern);
			}
		}
		public static IEnumerable<FileInfo> GetFiles(string rootDirectory, string searchPattern, GetFileMethod method = GetFileMethod.Default) {
			switch (method.Value()) {
				case GetFileMethod.Plain:
					return GetFilesPlain(rootDirectory, searchPattern);
				case GetFileMethod.Enumerator:	
					return GetFilesEnumerator(rootDirectory, searchPattern);
				default:
					return GetFilesEnumeratorLinq(rootDirectory, searchPattern);
			}
		}
		public static List<string> GetFileNamesPlain(string rootDirectory, string searchPattern) {
			var getFilesHelper = new GetFileNamesHelper(searchPattern);
			return getFilesHelper.GetFileList(rootDirectory);
		}
		
		public static List<FileInfo> GetFilesPlain(string rootDirectory, string searchPattern) {
			var getFilesHelper = new GetFilesHelper(searchPattern);
			return getFilesHelper.GetFileList(rootDirectory);
		}
		
		public static IEnumerable<string> GetFileNamesEnumerator(string rootDirectory, string searchPattern) {
			var getFilesHelper = new GetFileNamesEnumeratorHelper(searchPattern);
			return getFilesHelper.GetFileList(rootDirectory);
		}
		
		public static IEnumerable<FileInfo> GetFilesEnumerator(string rootDirectory, string searchPattern) {
			var getFilesHelper = new GetFilesEnumeratorHelper(searchPattern);
			return getFilesHelper.GetFileList(rootDirectory);
		}
		
		public static IEnumerable<string> GetFileNamesEnumeratorLinq(string rootDirectory, string searchPattern) {
			var getFilesHelper = new GetFileNamesEnumeratorLinqHelper(searchPattern);
			return getFilesHelper.GetFileList(rootDirectory);
		}
		
		public static IEnumerable<FileInfo> GetFilesEnumeratorLinq(string rootDirectory, string searchPattern) {
			var getFilesHelper = new GetFilesEnumeratorLinqHelper(searchPattern);
			return getFilesHelper.GetFileList(rootDirectory);
		}
		#endregion
		#region Find File Base Methods
		static T findFileName<T>(IEnumerable<T> files) {
			return files.FirstOrDefault();
		}
		
		static FileInfo findFile(IEnumerable<FileInfo> files, Func<FileInfo, bool> predicate = null) {
			if (predicate == null) {
				return findFileName(files);
			}
			FileInfo retval = null;
			Parallel.ForEach(files, (fi, loopState) => {
			                 	if (predicate(fi)) {
			                 		retval = fi;
			                 		loopState.Stop();
			                 	}
			                 });	 
			return retval;			
		}
		#endregion
		#region Find File Implementations
		public static string FindFileName(DirectoryInfo rootDirectory, string searchPattern, GetFileMethod method = GetFileMethod.Default) {
			return FindFileName(rootDirectory.FullName, searchPattern, method);
		}
		public static string FindFileName(string rootDirectory, string searchPattern, GetFileMethod method = GetFileMethod.Default) {
			switch (method.Value()) {
				case GetFileMethod.Plain:
					return FindFileNamePlain(rootDirectory, searchPattern);
				case GetFileMethod.Enumerator:	
					return FindFileNameEnumerator(rootDirectory, searchPattern);
				default:
					return FindFileNameEnumeratorLinq(rootDirectory, searchPattern);
			}
		}
		public static FileInfo FindFile(DirectoryInfo rootDirectory, string searchPattern, Func<FileInfo, bool> predicate = null, GetFileMethod method = GetFileMethod.Default) {
			return FindFile(rootDirectory.FullName, searchPattern, predicate, method);
		}
		public static FileInfo FindFile(string rootDirectory, string searchPattern, Func<FileInfo, bool> predicate = null, GetFileMethod method = GetFileMethod.Default) {
			switch (method.Value()) {
				case GetFileMethod.Plain:
					return FindFilePlain(rootDirectory, searchPattern, predicate);
				case GetFileMethod.Enumerator:	
					return FindFileEnumerator(rootDirectory, searchPattern, predicate);
				default:
					return FindFileEnumeratorLinq(rootDirectory, searchPattern, predicate);
			}
		}
		public static string FindFileNamePlain(string rootDirectory, string searchPattern) {
			return findFileName(GetFileNamesPlain(rootDirectory, searchPattern));
		}
		
		public static FileInfo FindFilePlain(string rootDirectory, string searchPattern, Func<FileInfo, bool> predicate = null) {
			return findFile(GetFilesPlain(rootDirectory, searchPattern), predicate);
		}
		
		public static string FindFileNameEnumerator(string rootDirectory, string searchPattern) {
			return findFileName(GetFileNamesEnumerator(rootDirectory, searchPattern));
		}
		
		public static FileInfo FindFileEnumerator(string rootDirectory, string searchPattern, Func<FileInfo, bool> predicate = null) {
			return findFile(GetFilesEnumerator(rootDirectory, searchPattern), predicate);
		}
		
		public static string FindFileNameEnumeratorLinq(string rootDirectory, string searchPattern) {
			return findFileName(GetFileNamesEnumeratorLinq(rootDirectory, searchPattern));
		}
		
		public static FileInfo FindFileEnumeratorLinq(string rootDirectory, string searchPattern, Func<FileInfo, bool> predicate = null) {
			return findFile(GetFilesEnumeratorLinq(rootDirectory, searchPattern), predicate);
		}
		#endregion
	}
}
