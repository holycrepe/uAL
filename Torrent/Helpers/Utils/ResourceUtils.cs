
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Torrent.Properties.Settings;

namespace Torrent.Helpers.Utils
{

    public static class ResourceUtils
    {
        static Dictionary<string, string[]> Keys = new Dictionary<string, string[]>();
        public static Uri GetApplicationUri(string path)
            => new Uri("pack://application:,,,/" + path);
        public static Uri GetImageUriFromFileName(string path)
            => GetApplicationUri($"Assets/{(path.EndsWith(".ico") ? "Icons" : "Images")}/{path}");
        public static Uri GetImageUri(string name)
        {
            var uri = GetImageUriFromFileName($"{name}.ico");
            if (CanCreateImageFrom(uri))
            {
                return uri;
            }
            uri = GetImageUriFromFileName($"{name}.png");
            if (CanCreateImageFrom(uri))
            {
                return uri;
            }
            return null;
        }
        public static StackPanel GetColumnGroupHeader(string iconPath)
            => Application.Current.TryFindResource(iconPath + "Image") as StackPanel;
        public static ImageSource GetIconFromPath(string iconPath)
        {
            if (string.IsNullOrEmpty(iconPath) || iconPath == "None")
            {
                return null;
            }
            iconPath = iconPath.Replace(" ", "");
            var iconResource = Application.Current.TryFindResource(iconPath + "Icon");
            if (iconResource != null)
            {
                return (ImageSource)iconResource;
            }
            var uri = GetImageUri(iconPath);
            if (uri != null)
            {
                return new BitmapImage(uri);
            }
            iconResource = Application.Current.TryFindResource(iconPath);
            return (ImageSource)iconResource;
        }
        /// <summary>(2) Verify the resource is located where I think it is.</summary>
        public static bool ResourceExists(Uri resourceUri)
            => ResourceExists(resourceUri.LocalPath);
        public static bool ResourceExists(string resourcePath, Assembly assembly=null)
            => GetResourcePaths(assembly).Contains(resourcePath.ToLowerInvariant().Trim(Path.AltDirectorySeparatorChar));
        public static string[] GetResourcePaths(Assembly assembly=null)
            => GetResourcePaths(assembly, CultureInfo.CurrentCulture);
        public static string[] GetResourcePaths(Assembly assembly, CultureInfo culture)
        {
            if (assembly == null)
            {
                assembly = MainApp.Assembly;
            }
            var assemblyName = assembly.GetName();
            var assemblyFullName = assemblyName.FullName;
            if (!Keys.ContainsKey(assemblyFullName))
            {
                var resourceName = assemblyName.Name + ".g";
                var resourceManager = new ResourceManager(resourceName, assembly);
                var keys = new List<string>();
                try
                {
                    var resourceSet = resourceManager.GetResourceSet(culture, true, true);
                    foreach (DictionaryEntry resource in resourceSet)
                    {
                        var key = resource.Key.ToString();
                        if (!key.EndsWith(".baml"))
                        {
                            keys.Add(key);
                        }
                    }
                    keys.Sort();
                    Keys[assemblyFullName] = keys.ToArray();
                }
                finally
                {
                    resourceManager.ReleaseAllResources();
                }
            }
            if (Keys.ContainsKey(assemblyFullName))
            {
                return Keys[assemblyFullName];
            }
            return new string[0];
        }

        /// <summary>(3) Verify the uri can construct an image.</summary>
        public static bool CanCreateImageFrom(Uri uri)
        {
            if (!ResourceExists(uri))
            {
                return false;
            }
            try
            {
                var bm = new BitmapImage(uri);
                return bm.UriSource == uri;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return false;
            }
        }
    }
}
