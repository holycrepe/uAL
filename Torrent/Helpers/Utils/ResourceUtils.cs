
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
using Torrent.Extensions;
using Torrent.Extensions.Framework;
using Torrent.Properties.Settings;

namespace Torrent.Helpers.Utils
{
    using Infrastructure;
    public static class ResourceUtils
    {
        private static ResourceDictionary _designResources = null;

        private static ResourceDictionary DesignResources
            => _designResources ?? (_designResources = GetDesignResources());

        public static T Get<T>() where T : class
            => Get<T>(typeof (T).Name);
        public static T Get<T>(string key)
        where T : class
        {
            if (!MainApp.DesignMode)
                return (Application.Current.TryFindResource(key)
                        ?? UI.Window?.TryFindResource(key)) as T;
            var resource = DesignResources.TryFindResource<T>(key);
            if (resource == null)
                Debugger.Break();
            return resource;
        }

        static Dictionary<string, string[]> Keys 
            = new Dictionary<string, string[]>();
        public static Uri GetAppUri(string path, string assembly = "")
            => new Uri("/" + assembly.Suffix(";component/") + path, UriKind.Relative);
        public static Uri GetPackUri(string path, string assembly="")
            => new Uri("pack://application:,,,/" + assembly.Suffix(";component/") + path, UriKind.Absolute);
        public static Uri GetImageUriFromFileName(string path)
            => GetPackUri($"Assets/{(path.EndsWith(".ico") ? "Icons" : "Images")}/{path}");
        public static Uri GetImageUri(string name)
        {            
            var uri = GetImageUriFromFileName($"{name}.ico");
            if (CanCreateImageFrom(uri))
            {
                return uri;
            }
            uri = GetImageUriFromFileName($"{name}.png");
            return CanCreateImageFrom(uri) ? uri : null;
        }
        public static StackPanel GetColumnGroupHeader(string iconPath)
            => Application.Current.TryFindResource(iconPath + "Image") as StackPanel;
        static ResourceDictionary GetDesignResources()
            => Application.LoadComponent(GetAppUri("Assets/Design.xaml", MainApp.Name)) as ResourceDictionary;
        public static ImageSource GetIconFromPath(string iconPath)
        {
            if (string.IsNullOrEmpty(iconPath) || iconPath == "None")            
                return null;
            
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
                    keys.AddRange(resourceSet.Cast<DictionaryEntry>()
                        .Select(resource => resource.Key.ToString())
                        .Where(key => !key.EndsWith(".baml")));
                    keys.Sort();
                    Keys[assemblyFullName] = keys.ToArray();
                }
                finally
                {
                    resourceManager.ReleaseAllResources();
                }
            }
            return Keys.ContainsKey(assemblyFullName) 
                ? Keys[assemblyFullName] 
                : new string[0];
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
