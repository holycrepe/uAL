// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.LibSettings
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using ToggleSettings;
    using Torrent.Extensions;
    using static ToggleSettings.ToggleSettings;
    using static ToggleSettings.MonitorTypes;
    #endregion

    public sealed partial class LibSettings
    {
        #region Fields and Properties

        #region Fields

        #region Fields: Public

        public static string AppPath = Assembly.GetExecutingAssembly().CodeBase;
        //public static readonly string[] ValidityDeterminingSettings = {nameof(LibSetting.CONNECTION), nameof(LibSetting.DIRECTORY) };

        #endregion

        #region Fields: Private

        private static List<string> arguments;

        #endregion

        #endregion

        #region  Properties

        #region Properties: Public

        public bool IS_VALID
            =>
                (this.Connection.HOST == "127.0.0.1" || !string.IsNullOrEmpty(this.Directories.ACTIVE))
                && !(string.IsNullOrEmpty(this.Connection.USER_NAME) || string.IsNullOrEmpty(this.Connection.PASSWORD)
                     || string.IsNullOrEmpty(this.Connection.HOST)
                     || this.Connection.PORT == 0);

        //public static LibSettings LibSetting { get; } = (LibSettings) Synchronized(new LibSettings());

        #region Preview Mode
        public static MonitorTypes PreviewMode
        {
            get { return Toggles.Processing.PreviewMode; }
            set { if (PreviewMode != value) { Toggles.Processing.PreviewMode = value; } }
        }
        public static string PreviewModeCharacter
        {
            get
            {
                switch (PreviewMode) {
                    case All:
                    case AllMonitors:
                        return "[P]";
                    case MonitorTypes.Torrent:
                        return "[TP]";
                    case Metalink:
                        return "[MP]";
                    default:
                        return "";
                }
            }
        }

        public static string PreviewModeDescription
        {
            get
            {                
                switch (PreviewMode) {
                    case Disabled:
                        return "";
                    case All:
                    case AllMonitors:
                        return " in Preview Mode";
                    default:
                        return
                            $" in {PreviewMode.GetDescription()} Preview Mode";
                }
            }
        }

        public static string StatusDescription
            => " " + (LibSetting.Connection.IS_CONNECTED ? "Connected" : "Unconnected") + PreviewModeDescription;

        #endregion

        public static List<string> Arguments
        {
            get { return arguments; }
            set
            {
                arguments = value;
                if (HasArgument("--preview", "-p"))
                {
                    PreviewMode = All;
                }
                if (HasArgument("--noimport", "-n", "--torrent-preview"))
                {
                    PreviewMode &= MonitorTypes.Torrent;
                }
                if (HasArgument("--nometalinkimport", "-m", "--metalink_preview"))
                {
                    PreviewMode &= Metalink;
                }
                if (HasArgument("--nojobimport", "-j", "--job_preview", "--utorrent_preview"))
                {
                    PreviewMode &= Job;
                }
            }
        }

        #endregion

        #endregion

        #endregion

        public static bool HasArgument(params string[] args)
        {
            if (Arguments == null) {
                Debugger.Break();
                return false;
            }
            return Arguments.ContainsAny(args);
        }
    }
}
