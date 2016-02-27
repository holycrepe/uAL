using System;
using System.IO;
using Torrent.Enums;

namespace Torrent.Helpers.Utils
{
    using Extensions;
    using FileHelpers;
    using Properties.Settings;
    using System.Linq;

    public static class PathUtils
    {
        private const int MAX_PATH_LENGTH = 260;
        public static string GetFinalPath(string path)
            => new ReparsePoint(path).Normalized;
        //public static string GetFinalPath(string path)
        //    => NativeMethods.GetFinalPathName(path).TrimStart(@"\\?\");
        [System.Diagnostics.Contracts.Pure]
        public static string Shorten(params string[] fileNames)
        {            
            if (fileNames.Length == 0) {
                throw new ArgumentNullException(nameof(fileNames));
            }
            var fileName = Path.Combine(fileNames);
            if (fileName.Length < MAX_PATH_LENGTH) {
                return fileName;
            }
            var extension = Path.GetExtension(fileName);
            var baseFileName = fileName.Substring(0, fileName.Length - extension.Length);
            var newFileName = (baseFileName.Substring(0, MAX_PATH_LENGTH - 1 - extension.Length - 1)) + "." + extension;
            Log(nameof(Shorten), fileName, " ==> " + Path.GetFileNameWithoutExtension(newFileName));
            return newFileName;
        }
        public static string MakeLocal(string path)
            => path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);        
        public static string GetFullNameWithoutExtension(string path)
            => Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
        public static string GetFullNameWithSuffix(string path, string suffix)
            => GetFullNameWithoutExtension(path) + suffix + Path.GetExtension(path);
        public static string MakeRelative(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) {
                throw new ArgumentNullException(nameof(fromPath));
            }
            if (string.IsNullOrEmpty(toPath)) {
                throw new ArgumentNullException(nameof(toPath));
            }

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) {
                return toPath;
            } // path can't be made relative.

            var relativeUri = fromUri.MakeRelativeUri(toUri);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.ToUpperInvariant() == "FILE") {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        [System.Diagnostics.Contracts.Pure]
        public static string MakeAbsolute(string path, string basePath)
        {
            if (string.IsNullOrEmpty(path) && string.IsNullOrEmpty(basePath))
            {
                throw new ArgumentNullException(nameof(path));
            }
            return Path.IsPathRooted(path) ? path : Path.Combine(basePath, path);
        }        

        public static string GetFileName(string path)
        {
            var i = Math.Max(path.IndexOf('\\'), path.IndexOf('/'));
            return (i == -1 ? path : path.Substring(i));
        }

        public static string GetFileNameWithoutExtension(string path)
            => GetFileName(Path.GetFileNameWithoutExtension(path));

        public static string ConvertFromUri(string uriPath)
        {
            if (string.IsNullOrEmpty(uriPath))
                return null;
            Uri uri;
            return Uri.TryCreate(uriPath, UriKind.RelativeOrAbsolute, out uri) ? uri.LocalPath : uriPath;
        }

        public static string MakeSafe(string path)
            => "\"<>|\t\r\n:*?".Aggregate(path, (current, c) => current.Replace(c.ToString(), ""));

        static string[] GetValidPaths(params string[] paths)
        {
            var strPaths = paths?.Where(p => !string.IsNullOrEmpty(p)).ToArray();
            if (strPaths == null || strPaths.Length == 0)
            {
                throw new ArgumentNullException(nameof(paths));
            }
            return strPaths;
        }
        public static string Combine(params string[] paths)
            => Path.Combine(GetValidPaths(paths));
        public static string CombineSafe(string basePath, params string[] paths)
            => GetValidPaths(paths).Aggregate(MakeSafe(basePath), 
                (current, path) => Path.Combine(current, MakeSafe(path)));

        #region Logging

        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        static void Log(string title, string text = null, string item = null,
                        PadDirection textPadDirection = PadDirection.Default, string textSuffix = null,
                        PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            =>
                LogUtils.Log("PathUtils", title, text, item, textPadDirection, textSuffix, titlePadDirection,
                             titleSuffix, random);

        #endregion
    }
}
