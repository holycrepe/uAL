// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.LibSettings
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using PostSharp.Patterns.Model;
    using Torrent.Properties.Settings;
    using Torrent.Enums;
    using Torrent.Helpers.Utils;
    using System.IO;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using Serialization;
    public sealed partial class LibSettings
    {
        [DataContract(Namespace = Namespaces.Default)]
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        [NotifyPropertyChanged]
        public class LibConnectionSettings : BaseSubSettings
        {
            #region Primary Members
            [DataMember]
            public string UserName { get; set; } = "admin";
            [DataMember]
            public string Password { get; set; }
            [DataMember]
            public string Host { get; set; } = "localhost";
            [DataMember]
            public int Port { get; set; } = 65255;
            [DataMember]
            public bool ConnectOnStartup { get; set; } = true;
            public LibConnectionSettings() : this($"Initializing.{nameof(LibConnectionSettings)}") { }
            public LibConnectionSettings(string name) : base(name) { }

            #endregion

            #region Loaded From Settings Directory

            private static string _pw;

            #endregion

            #region Constructor

            static LibConnectionSettings()
            {
                var pwFile = MainApp.MakeRelativePath($"{nameof(Password)}.txt");
                if (File.Exists(pwFile)) {
                    _pw = File.ReadAllText(pwFile);
                }
            }

            #endregion

            #region Derived Members
            protected override object[] DebuggerDisplayProperties => new object[]
            {
                nameof(UserName), UserName,
                nameof(Password), Password,
                nameof(Host), Host,
                nameof(Port), Port
            };

            public static string[] ConnectionValidityDeterminingSettings =
            {
                nameof(UserName),
                nameof(Password),
                nameof(Host),
                nameof(Port)
            };

            //public delegate bool IsConnectedDelegate();            
            //public IsConnectedDelegate IsConnectedFunc { get; set; }

            [SafeForDependencyAnalysis]
            public string PASSWORD
                => this.Password ?? (this.Password = _pw);

            public string HOST
                =>
                    this.Host == "" || this.Host == "localhost"
                        ? "127.0.0.1"
                        : this.Host;

            public int PORT => this.Port;
            public string USER_NAME => this.UserName;

            [XmlIgnore]
            public bool IS_CONNECTED { get; set; } = false;

            #endregion

            #region Logging

            //[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
            //void Log(string title, string text = null, string item = null, PadDirection textPadDirection = PadDirection.Default, string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            //=> LogUtils.Log("CONNECTION", title, text, item, textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);
            ////public event PropertyChangedEventHandler PropertyChanged;
            //protected override void OnPropertyChanged(string propertyName)
            //{
            //    Log(nameof(OnPropertyChanged), propertyName, ClassUtils.GetPropertyValue(this, propertyName).ToString());
            //    base.OnPropertyChanged(propertyName);
            //}

            #endregion
        }
    }
}
