using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.IO;
using Torrent.Helpers.Utils;

namespace Torrent.Properties.Settings
{
    public static class SettingsManager
    {
        static SettingsManager()
        {
            Settings = GetSettings();
        }

        private static ApplicationSettingsBase GetSettings()
        {
            var asm = Assembly.GetEntryAssembly();
            if (asm == null) return null;
            AssemblyName = asm.GetName();
            AppName = AssemblyName.Name;
            AppPath = PathUtils.ConvertFromUri(asm.CodeBase);
            var settingsType = (from t in asm.GetTypes()
                where t.IsSubclassOf(typeof(ApplicationSettingsBase))
                select t).FirstOrDefault();
            if (settingsType == null) return null;
            var pi = settingsType.GetProperty("Default", BindingFlags.Public | BindingFlags.Static);
            return pi != null ? pi.GetValue(null, null) as ApplicationSettingsBase : Activator.CreateInstance(settingsType) as ApplicationSettingsBase;
        }
      
        public static string AppPath { get; private set; }
        public static AssemblyName AssemblyName { get; private set; }
        public static string AppName { get; private set; }
        // ReSharper disable once AssignNullToNotNullAttribute
        public static string MakeRelativePath(string fileName) => Path.Combine(Path.GetDirectoryName(AppPath), Path.GetFileNameWithoutExtension(AppPath) + "." + fileName);

        public static ApplicationSettingsBase Settings
        {
        	get; set;
        }

    }
    
}
