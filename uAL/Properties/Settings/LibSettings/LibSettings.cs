namespace uAL.Properties.Settings
{
    #region Usings

    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using JetBrains.Annotations;
    using ToggleSettings;
    using Torrent;
    using Torrent.Enums;
    using Torrent.Extensions;
    using Torrent.Helpers.StringHelpers;
    using Torrent.Helpers.Utils;
    using Torrent.Properties.Settings;
    using static ToggleSettings.Toggles;
    using static Torrent.Infrastructure.NotifyPropertyChangedBase;

    #endregion

    // This class allows you to handle specific events on the settings class:
    //  The SettingChanging event is raised before a setting's value is changed.
    //  The PropertyChanged event is raised after a setting's value is changed.
    //  The SettingsLoaded event is raised after the setting values are loaded.
    //  The SettingsSaving event is raised before the setting values are saved.

    public sealed partial class LibSettings
    {
        public delegate bool __isConnectedLambda();

        #region Fields and Properties

        #region Fields

        #region Fields: Public

        public static string AppPath = Assembly.GetExecutingAssembly().CodeBase;

        public static string[] LabelFilterDeterminingSettings = {"labelsToInclude", "labelsToExclude"};

        public static readonly Dictionary<string, bool> LabelFilterResults = new Dictionary<string, bool>();

        public static List<string> Labels;

        public static string[] ValidityDeterminingSettings = {"UserName", "Password", "Host", "Port", "ImportDirectory"};

        #endregion

        #region Fields: Private

        private static __isConnectedLambda _isConnectedLambda = () => false;

        private static List<string> arguments;

        #endregion

        #endregion

        #region  Properties

        #region Properties: Public

        public static string ActiveAddedDirectory
            => LibSetting.AddedTorrentDirectory == ""
                   ? Path.Combine(Path.GetDirectoryName(ActiveDirectory), "ADDED")
                   : LibSetting.AddedTorrentDirectory;

        public static string ActiveDownloadDirectory
            => LibSetting.DownloadsDirectory == ""
                   ? Path.GetPathRoot(ActiveDirectory)
                   : LibSetting.DownloadsDirectory;

        public static string ActiveHost
            => LibSetting.Host == "" || LibSetting.Host == "localhost" ? "127.0.0.1" : LibSetting.Host;

        public static bool IS_CONNECTED => IsConnectedLambda();

        public static bool IS_VALID
            =>
                (ActiveHost == "127.0.0.1" || ActiveDirectory != "")
                && !(LibSetting.UserName == "" || LibSetting.Password == "" || LibSetting.Host == ""
                     || LibSetting.Port == 0);

        public static bool IS_VALID_DIRECTORY => Directory.Exists(ActiveDirectory);

        public static string[] LabelsToDupeCheck
        {
            get
            {
                var words = LibSetting.labelsToDupeCheck;
                return words == null || words.Count == 0 ? Labels.ToArray() : words.Cast<string>().ToArray();
            }
        }

        public static IEnumerable<string> LabelsWordsToStrip
        {
            get
            {
                IEnumerable<string> defaultWords =
                    @"(?:HD)?\d{3,}[dpk]|\d{3,}x\d{3,}|\d+(?:\.\d+)? ?[MG]B|DVD ?Rip|x264|MP4|WMV|MKV|AVC|REQ|SD|HD|New|REQ|Request(?:ed)?|WEB\-DL|included|^ \-+ | \-+ $"
                        .Split('|');
                var words = LibSetting.labelsWordsToStrip;
                defaultWords = defaultWords.Concat(TorrentNameWordsToStrip);
                return words == null ? defaultWords : defaultWords.Concat(words.Cast<string>());
            }
        }

        public static IEnumerable<string> LabelsWordsToStripExtended
        {
            get
            {
                var defaultWords = @"Scenes?".Split('|');
                var words = LibSetting.labelsWordsToStripExtended;
                return words == null ? defaultWords : defaultWords.Concat(words.Cast<string>());
            }
        }

        public static LibSettings LibSetting { get; } = (LibSettings) Synchronized(new LibSettings());

        public static bool MayQueueAllMetalinks => !LibSetting.HaveQueuedAllMetalinks || !Monitor.METALINKS;

        public static bool MayQueueAllTorrents => !LibSetting.HaveQueuedAllTorrents || !Monitor.TORRENTS;

        public static string PreviewModeCharacter
        {
            get
            {
                switch (PreviewMode.Status) {
                    case QueueToggleStatus.All:
                        return "[P]";
                    case QueueToggleStatus.Torrent:
                        return "[TP]";
                    case QueueToggleStatus.Metalink:
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
                switch (PreviewMode.Status) {
                    case QueueToggleStatus.Disabled:
                        return "";
                    case QueueToggleStatus.All:
                        return " in Preview Mode";
                    default:
                        return
                            $" in {Enum.GetName(typeof (QueueToggleStatus), PreviewMode.Status)} Preview Mode";
                }
            }
        }

        public static string StatusDescription
            => " " + (IS_CONNECTED ? "Connected" : "Unconnected") + PreviewModeDescription;

        public static IEnumerable<string> TorrentNameWordsToStrip
        {
            get
            {
                var defaultWords = @"BootyTape(?:\.com)|Empornium".Split('|');
                var words = LibSetting.torrentFileNameWordsToStrip;
                return words == null ? defaultWords : defaultWords.Concat(words.Cast<string>());
            }
        }

        [NotNull]
        public static string ActiveDirectory
        {
            get
            {
                return LibSetting.ImportDirectory == ""
                           ? LibSetting.uTorrentImportDirectory
                           : LibSetting.ImportDirectory;
                //return (PREVIEW_MODE ? previewDirectory : Default.ImportDirectory);
            }
            set
            {
                LibSetting.uTorrentImportDirectory = value;
                LibSetting.OnPropertyChanged(nameof(ActiveDirectory));
            }
        }

        public static List<string> Arguments
        {
            get { return arguments; }
            set
            {
                arguments = value;
                ToggleSettings.ToggleSettings.ToggleSetting.PreviewMode.Arguments.Set(
                                                                                      HasArgument("--preview", "-p"),
                                                                                      HasArgument("--noimport", "-n",
                                                                                                  "--torrent-preview"),
                                                                                      HasArgument("--nometalinkimport",
                                                                                                  "-m",
                                                                                                  "--metalink_preview")
                    );
                LibSetting.OnPropertyChanged(nameof(Arguments));
            }
        }

        public static __isConnectedLambda IsConnectedLambda
        {
            get { return _isConnectedLambda; }
            set
            {
                _isConnectedLambda = value;
                LibSetting.OnPropertyChanged(nameof(IS_CONNECTED));
            }
        }

        #endregion

        #endregion

        #endregion

        #region Constructors

        public LibSettings()
        {
            // // To add event handlers for saving and changing settings, uncomment the lines below:
            //
            // this.SettingChanging += this.SettingChangingEventHandler;
            //
            // this.SettingsSaving += this.SettingsSavingEventHandler;
            //
            RegexUtils.GetLabelsWordsToStrip = GetLabelsWordsToStrip;
            TorrentItem.getWordsToStrip = e => TorrentNameWordsToStrip;
            ToggleSettings.ToggleSettings.ToggleSetting.Filters.PropertyChanged +=
                (object sender, PropertyChangedEventArgs e) => LabelFilterResults.Clear();
        }

        #endregion

        public static bool HasArgument(params string[] args)
        {
            if (Arguments == null) {
                Debugger.Break();
                return false;
            }
            return Arguments.ContainsAny(args);
        }

        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE")]
        private static void LogLabelFilter(string title, string text = null, string item = null, PadDirection textPadDirection = PadDirection.Default, string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            => LogUtils.Log("Label.Filters", title, text, item, textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);

        public static IEnumerable<string> GetLabelsWordsToStrip(bool extended)
            => extended ? LabelsWordsToStrip.Concat(LabelsWordsToStripExtended) : LabelsWordsToStrip;

        public static bool FilterLabel(string label, Toggle TOGGLES)
        {
            var debug = LogUtils.DEBUG.LABEL_FILTERS;
            var doFilterIncludes = TOGGLES.FILTERS.INCLUDE && LibSetting.labelsToInclude.Count > 0;
            var doFilterExcludes = TOGGLES.FILTERS.EXCLUDE && LibSetting.labelsToExclude.Count > 0;
            if (!TOGGLES.FILTERS.GLOBAL || (!doFilterIncludes && !doFilterExcludes)) {
                return true;
            }
            if (LabelFilterResults.ContainsKey(label)) {
                return LabelFilterResults[label];
            }

            if (doFilterIncludes) {
                var matches = LibSetting.labelsToInclude.Regex(label);
                if (matches.Count == 0) {
                    if (debug) {
                        LogLabelFilter("Include", "  Failed", label);
                    }
                    return LabelFilterResults[label] = false;
                }
                if (debug) {
                    LogLabelFilter("Include", "+ " + matches[0].Value, label);
                }
            }

            if (doFilterExcludes) {
                var matches = LibSetting.labelsToExclude.Regex(label);
                if (matches.Count > 0) {
                    if (debug) {
                        LogLabelFilter("Exclude", "- " + matches[0].Value);
                    }
                    return LabelFilterResults[label] = false;
                }
            }
            return LabelFilterResults[label] = true;
        }

        private void OnPropertyChanged(params string[] propertyNames)
        {
            if (LabelFilterDeterminingSettings.ContainsAny(propertyNames)) {
                LabelFilterResults.Clear();
            }

            DoOnPropertyChanged(this, OnPropertyChanged, propertyNames);
        }

        private void SettingChangingEventHandler(object sender, SettingChangingEventArgs e)
        {
            // Add code to handle the SettingChangingEvent event here.
            if (ValidityDeterminingSettings.Contains(e.SettingName)) {}
        }

        private void SettingsSavingEventHandler(object sender, CancelEventArgs e)
        {
            // Add code to handle the SettingsSaving event here.
        }
    }
}
