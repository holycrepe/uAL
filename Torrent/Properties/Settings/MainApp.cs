using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Resources;
using Torrent.Extensions.Reflection;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure.Exceptions;

namespace Torrent.Properties.Settings
{
    using JetBrains.Annotations;
    using System.ComponentModel;
    using IO = System.IO;
    using static Helpers.Utils.DebugUtils;
    using Extensions;
    using System.Windows;
    public static class MainApp
    {
        static MainApp()
        {

        }

        public static Assembly[] GetMainAssemblies()
        {
            if (!DesignMode)
                return new [] { Assembly.GetEntryAssembly() };
            var asms = new List<Assembly>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.EntryPoint == null)
                    continue;
                var types = asm.GetTypesSafe();
                var app = types?.FirstOrDefault(t => t?.IsSubclassOf(typeof (System.Windows.Application)) == true);
                if (app == null)
                    continue;                
                asms.Add(asm);
            }
            return asms.ToArray();
        }
        /// <summary>
        /// This method tries to find the assembly of resources and the strongly 
        /// typed Resource class in a project so the designer works properly.
        /// 
        /// It's recommended your application ALWAYS explicitly initializes the
        /// DefaultResourceAssembly and DefaultResourceManager in LocalizationSettings.
        /// 
        /// When running in the designer this code tries to find the System.Windows.Application 
        /// </summary>
        /// <returns></returns>
        //public static ResourceManager GetResourceManager()
        //{
        //    var a = Assembly.GetManifestResourceNames();
            
        //    // Assume the Document we're called is in the same assembly as resources
        //    var _asm = Assembly ?? Assembly.GetExecutingAssembly();

        //    // Search for Properties.Resources in the Exported Types (has to be public!)
        //    var resType = _asm.GetExportedTypes()
        //        .FirstOrDefault(type => type.FullName.Contains(".Properties.Resources"));

        //    return resType?.GetProperty("ResourceManager")
        //                   .GetValue(resType, null) as ResourceManager;
        //}
        public static Assembly GetMainAssembly()
        {
            var asms = GetMainAssemblies();
            if (!asms.Any())
                return Assembly.GetExecutingAssembly();
            return asms.LastOrDefault();
        }
        private static ApplicationSettingsBase GetSettings()
        {
            if (Assembly == null)
                return null;
            
            var types = Assembly.GetTypesSafe("Error creating MainApp Settings: Could not load types");
            _loadedSettings = true;

            var settingsType = types?.FirstOrDefault();
            if (settingsType == null)
                return null;

            var pi = settingsType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
            if (pi != null)
                return pi.GetValue(null, null) as ApplicationSettingsBase;

            if (settingsType.GetConstructor(Type.EmptyTypes) == null)
                return null;

            try
            {
                return Activator.CreateInstance(settingsType) as ApplicationSettingsBase;
            }
            catch (Exception ex)
            {
                DEBUG.LogError(ex, "Error creating MainApp Settings");
            }
            return null;
        }
        private static Assembly _assembly = null;
        private static ApplicationSettingsBase _settings = null;
        private static bool _loadedSettings = false;
        private static string _path = null;
        private static string _name = null;
        public static bool DesignMode
            => (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
        [NotNull]
        public static string Path
            => _path ?? (_path = PathUtils.ConvertFromUri(Assembly.CodeBase));
        public static Assembly Assembly
            => _assembly ?? (_assembly = GetMainAssembly());
        //public static ResourceManager Resources
        //    => _resourceManager ?? (_resourceManager = GetResourceManager());
        //public static AssemblyName AssemblyName { get; private set; }
        [NotNull]
        public static string Name            
            => _name ?? (_name = Assembly.GetName().Name) ?? "N/A";
            //=> _name ?? (_name = PathUtils.ConvertFromUri(Assembly?.CodeBase)) ?? "N/A";
        public static string FileName
            => IO.Path.GetFileNameWithoutExtension(Path);
        // ReSharper disable once AssignNullToNotNullAttribute
        public static string MakeRelativePath(string fileName)
            => IO.Path.Combine(IO.Path.GetDirectoryName(Path), FileName + "." + fileName);
        public static ApplicationSettingsBase Settings
            => _settings ?? (_settings = _loadedSettings ? null : GetSettings());
    }
}
