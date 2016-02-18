using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.IO;
using Torrent.Helpers.Utils;

namespace Torrent.Properties.Settings
{
    using JetBrains.Annotations;
    using System.ComponentModel;
    using IO = System.IO;
    using static Helpers.Utils.DebugUtils;

    public static class MainApp
    {
        static MainApp() { Settings = GetSettings(); }

        private static ApplicationSettingsBase GetSettings()
        {
            Assembly = Assembly.GetEntryAssembly();
            if (Assembly == null) {
                return null;
            }
            var assemblyName = Assembly.GetName();
            Name = assemblyName.Name;
            Path = PathUtils.ConvertFromUri(Assembly.CodeBase);
            Type settingsType=null;
            try {
                settingsType = (from t in Assembly.GetTypes()
                                    where t.IsSubclassOf(typeof(ApplicationSettingsBase))
                                    select t).FirstOrDefault();
            }
            catch (ReflectionTypeLoadException ex)
            {
                DEBUG.LogError(ex, "Error creating MainApp Settings: Could not load types");
            }
            if (settingsType == null) {
                return null;
            }
            var pi = settingsType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
            if (pi != null)
            {
                return pi.GetValue(null, null) as ApplicationSettingsBase;
            }
            if (settingsType.GetConstructor(Type.EmptyTypes) == null)
            {
                return null;
            }
            try {
                return Activator.CreateInstance(settingsType) as ApplicationSettingsBase;
            }
            catch (Exception ex)
            {
                DEBUG.LogError(ex, "Error creating MainApp Settings");
            }
            return null;
        }
        static bool? _designMode = null;
        public static bool DesignMode2
            => _designMode.HasValue ? _designMode.Value 
            : (_designMode = LicenseManager.UsageMode == LicenseUsageMode.Designtime).Value;
        public static bool DesignMode
            => (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
        [NotNull]
        public static string Path { get; private set; }
        public static Assembly Assembly { get; private set; }
        //public static AssemblyName AssemblyName { get; private set; }
        public static string Name { get; private set; }
            = "N/A";
        public static string FileName
            => IO.Path.GetFileNameWithoutExtension(Path);
        // ReSharper disable once AssignNullToNotNullAttribute
        public static string MakeRelativePath(string fileName)
            => IO.Path.Combine(IO.Path.GetDirectoryName(Path), FileName + "." + fileName);
        public static ApplicationSettingsBase Settings { get; set; }
    }
}
