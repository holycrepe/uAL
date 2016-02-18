// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.LibSettings
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using JetBrains.Annotations;
    using PostSharp.Patterns.Model;
    using Torrent.Helpers.Utils;
    using Torrent.Properties.Settings;
    using System.ComponentModel;
    using Torrent.Enums;
    using static Torrent.Helpers.Utils.DebugUtils;
    using System.Runtime.Serialization;
    using System.Diagnostics;
    using Serialization;
    public sealed partial class LibSettings
    {
        [DataContract(Namespace = Namespaces.Default)]
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        [NotifyPropertyChanged]
        public class LibDirectorySettings : BaseSubSettings, INotifyPropertyChanged
        {
            #region Primary
            [DataMember(EmitDefaultValue = false)]
            public string Added { get; set; }
            [DataMember(EmitDefaultValue = false)]
            public string Download { get; set; }
            [DataMember(EmitDefaultValue = false)]
            public string Import { get; set; }
            [DataMember(EmitDefaultValue = false)]
            public string uTorrent { get; set; }
            [DataMember]
            public string AppData { get; set; } 
                = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "uTorrent");
            [DataMember(EmitDefaultValue = false)]
            public string ResumeDat { get; set; }

            public LibDirectorySettings() : this($"Initializing.{nameof(LibDirectorySettings)}") { }
            public LibDirectorySettings(string name) : base(name) {
                this.ResumeDat = Path.Combine(this.AppData, "resume.dat");
            }

            #endregion

            #region Private

            const string DEFAULT_DIRECTORY_ROOT = "$";
            const string DEFAULT_DIRECTORY_NAME = "TORRENTS";
            const string DEFAULT_ADDED_SUBDIRECTORY = "ADDED";
            const string DEFAULT_SETTINGS_SUBDIRECTORY = "SETTINGS";

            #endregion

            #region Derived
            protected override object[] DebuggerDisplayProperties => new object[]
            {
                nameof(Added), Added,
                nameof(Download), Download,
                nameof(Import), Import,
                nameof(uTorrent), uTorrent,
                nameof(IS_VALID), IS_VALID,
                nameof(ACTIVE), ACTIVE,
                nameof(ADDED), ADDED,
                nameof(DOWNLOAD), DOWNLOAD,
                nameof(SETTINGS), SETTINGS
            };
            public static readonly string[] DirectoryValidityDeterminingSettings = {nameof(Import), nameof(uTorrent)};
            public bool IS_VALID => Directory.Exists(this.ACTIVE);

            [NotNull]
            public string ACTIVE => string.IsNullOrEmpty(this.Import)
                                        ? this.uTorrent
                                        : this.Import;


            [SafeForDependencyAnalysis]
            public string ADDED
                // ReSharper disable once AssignNullToNotNullAttribute
                => string.IsNullOrEmpty(this.Added)
                       ? string.IsNullOrEmpty(this.ACTIVE)
                             ? ""
                             : Path.Combine(Path.GetDirectoryName(this.ACTIVE), DEFAULT_ADDED_SUBDIRECTORY)
                       : PathUtils.MakeAbsolute(this.Added, Path.GetDirectoryName(this.ACTIVE));

            [SafeForDependencyAnalysis]
            public string SETTINGS
                // ReSharper disable once AssignNullToNotNullAttribute
                => Path.Combine(string.IsNullOrEmpty(this.ACTIVE)
                                    ? Path.Combine(Path.GetPathRoot(MainApp.Path), DEFAULT_DIRECTORY_ROOT,
                                                   DEFAULT_DIRECTORY_NAME)
                                    : Path.GetDirectoryName(this.ACTIVE), DEFAULT_SETTINGS_SUBDIRECTORY);

            public string DOWNLOAD
                => string.IsNullOrEmpty(this.Download)
                       ? string.IsNullOrEmpty(this.ACTIVE)
                             ? ""
                             : Path.GetPathRoot(this.ACTIVE)
                       : this.Download;

            #endregion

            //#region Logging
            //[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
            //void Log(string title, string text = null, string item = null, PadDirection textPadDirection = PadDirection.Default, string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            //=> LogUtils.Log("DIRECTORY", title, text, item, textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);
            ////public event PropertyChangedEventHandler PropertyChanged;
            //protected override void OnPropertyChanged(string propertyName)
            //{
            //    Log(nameof(OnPropertyChanged), propertyName, ClassUtils.GetPropertyValue(this, propertyName).ToString());
            //    base.OnPropertyChanged(propertyName);
            //}
            //#endregion
        }
    }
}
