using System;
using System.IO;

namespace Torrent.Helpers.Utils
{
    public static class PathUtils
    {        
    	
    	public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.ToUpperInvariant() == "FILE")
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

    	public static string GetFileName(string path) {
    		var i = Math.Max(path.IndexOf('\\'), path.IndexOf('/'));
			return (i == -1 ? path : path.Substring(i));
    	}
    	
        public static string ConvertFromUri(string uriPath)
        {
            Uri uri;
            if (Uri.TryCreate(uriPath, UriKind.RelativeOrAbsolute, out uri))
            {
                return uri.LocalPath;
            }

            return uriPath;
        }
        
        public static string MakeSafe(string path)
        {
            foreach (var c in "\"<>|\t\r\n:*?")
            {
                path = path.Replace(c.ToString(), "");
            }
            return path;
        }

        public static string CombineSafe(string basePath, params string[] paths)
        {
            var combinedPath = MakeSafe(basePath);
            foreach (var path in paths)
            {
                combinedPath = Path.Combine(combinedPath, MakeSafe(path));
            }
            return combinedPath;
        }
    }
}
