
#define TRACE_LABEL_FILTERSX

// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.LibSettings
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;
    using PostSharp.Patterns.Model;
    using Torrent.Properties.Settings;
    using Torrent.Infrastructure.FileSystem;
    using System.IO;
    using System.Linq;
    using ToggleSettings;
    using Torrent.Helpers.Utils;
    using Torrent.Extensions;
    using Torrent.Enums;
    using RunProcessAsTask;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.Collections.ObjectModel;
    using Torrent;
    using System.Collections.Concurrent;
    using System.Runtime.Serialization;
    using Serialization;
    public sealed partial class LibSettings
    {
        [DataContract(Namespace = Namespaces.Default)]
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        [NotifyPropertyChanged]
        public class LibLabelSettings : BaseSubSettings
        {
            #region Primary

            private LibLabelDefaultSettings _defaults = null;

            [XmlIgnore]
            internal LibLabelDefaultSettings Defaults
            {
                get { return this._defaults ?? (this._defaults = new LibLabelDefaultSettings()); }
                set { this._defaults = value; }
            }

            [DataMember]
            public string[] RootLabels
            {
                get { return this._rootLabels; }
                set { UpdateRootLabels(value); }
            }
            [DataMember]
            public ObservableCollection<string> Collection { get; set; } 
                = new ObservableCollection<string>();
            [DataMember]
            public string[] Generated {
                get { return this._generated; }
                set { this._generated = value; Update(); }
            }
            [DataMember]
            public List<string> Queue { get; set; } = new List<string>();
            [DataMember]
            public string[] Include { get; set; } = new string[0];
            [DataMember]
            public string[] Exclude { get; set; } = new string[0];
            [DataMember]
            public string[] WordsToStrip { get; set; } = new string[0];
            [DataMember]
            public string[] WordsToStripExtended { get; set; } = new string[0];
            [DataMember]
            public string[] TorrentNameWordsToStrip { get; set; } = new string[0];
            [DataMember]
            public bool ExtendByDefault { get; set; } = true;
            #endregion
            #region Protected: Generator Properties
            [DataMember]
            protected string GeneratorPrefix { get; set; }
                = @"$\";
            [DataMember]
            protected string GeneratorFolder { get; set; }
                = @"D:\Scripts\Python\torrent";
            [XmlIgnore]
            protected string GeneratorFileName
                => FileUtils.Resolve(this.GeneratorFolder, this._generatorFileName);
            [XmlIgnore]
            protected string GeneratorOutput
                => FileUtils.Resolve(this.GeneratorFolder, this._generatorOutput);

            [DataMember(Name=nameof(GeneratorFileName))]
            protected string _generatorFileName { get; set; }
                = "labels.bat";
            [DataMember(Name = nameof(GeneratorOutput))]
            protected string _generatorOutput { get; set; }
                = "labels.txt";
            #endregion
            #region Private
            string[] _rootLabels = new string[0];
            string[] _generated = new string[0];
            #endregion

            #region Constructor            
            public LibLabelSettings() : this($"Initializing.{nameof(LibLabelSettings)}") { }
            public LibLabelSettings(string name) : base(name)
            {
                this.Defaults = LoadDataContract<LibLabelDefaultSettings>();
                //Defaults.GetLists();
                //Defaults.Save();
                if (this.Defaults == null)
                {
                    Debugger.Break();
                }
                UpdateRootLabels();
                PropertyChanged += (s, e) => {
                    if (FilterDeterminingSettings.Contains(e.PropertyName))
                    {
                        this.FilterResults.Clear();
                    }
                };
            }
            static LibLabelSettings()
            {
                
            }

            #endregion            
            #region Derived
            protected override object[] DebuggerDisplayProperties => new object[]
            {                
                nameof(this.Include), this.Include,
                nameof(this.Exclude), this.Exclude,
                nameof(this.RootLabels), this.RootLabels,
                nameof(this.Collection), this.Collection == null ? "Empty" : $"{this.Collection.Count} Labels",
                nameof(this.Queue), this.Queue == null ? "Empty" : $"{this.Queue.Count} Labels",
                nameof(this.Generated), this.Generated == null ? "Empty" : $"{this.Generated.Count()} Labels"
            };

            private List<string> _labels = null;
            public static string[] FilterDeterminingSettings = {nameof(Include), nameof(Exclude)};
            public readonly ConcurrentDictionary<string, bool> FilterResults = new ConcurrentDictionary<string, bool>();

            public List<string> Labels
            {
                get { return this._labels ?? (this._labels = new List<string>()); }
                set { this._labels = value; }
            }

            [SafeForDependencyAnalysis]
            public string[] WORDS_TO_STRIP
            {
                get
                {
                    var defaultWords =
                        @"(?:HD)?\d{3,}[dpk]|\d{3,}x\d{3,}|\d+(?:\.\d+)? ?[MG]B|DVD ?Rip|x264|MP4|WMV|MKV|AVC|REQ|SD|HD|New|REQ|Request(?:ed)?|WEB\-DL|included|^ \-+ | \-+ $"
                            .Split('|');
                    if (this.Defaults?.LabelsWordsToStrip?.Length > 0) {
                        defaultWords = defaultWords.Union(this.Defaults.LabelsWordsToStrip).ToArray();
                    }
                    var words = this.WordsToStrip;
                    defaultWords = defaultWords.Union(this.TORRENT_NAME_WORDS_TO_STRIP).ToArray();

                    return words == null ? defaultWords : defaultWords.Union(words).ToArray();
                }
            }

            [SafeForDependencyAnalysis]
            public string[] WORDS_TO_STRIP_EXTENDED
            {
                get
                {
                    var defaultWords = @"Scenes?".Split('|');
                    if (this.Defaults?.LabelsWordsToStripExtended?.Length > 0) {
                        defaultWords = defaultWords.Union(this.Defaults.LabelsWordsToStripExtended).ToArray();
                    }
                    var words = this.WordsToStripExtended;
                    return words == null ? defaultWords : defaultWords.Union(words).ToArray();
                }
            }

            [SafeForDependencyAnalysis]
            public string[] TORRENT_NAME_WORDS_TO_STRIP
            {
                get
                {
                    var defaultWords = new string[] {}; //@"".Split('|');
                    if (this.Defaults?.TorrentWordsToStrip?.Length > 0) {
                        defaultWords = defaultWords.Union(this.Defaults.TorrentWordsToStrip).ToArray();
                    }
                    var words = this.TorrentNameWordsToStrip;
                    return words == null ? defaultWords : defaultWords.Union(words).ToArray();
                }
            }

            #endregion
            #region Methods            
            public string[] GetLabelsWordsToStrip(bool extended)
                => extended ? this.WordsToStrip.Union(this.WORDS_TO_STRIP_EXTENDED).ToArray() : this.WORDS_TO_STRIP;
            public void UpdateRootLabels(string[] values = null)
            {
                this._rootLabels = values ?? new string[0];
                if (!this._rootLabels.Any() && this.Labels != null && this.Labels.Any())
                {
                    this._rootLabels = this._rootLabels.Union(this.Labels.Select(s => $"*{s}")).ToArray();
                }
                if (this.Defaults?.RootLabels?.Length > 0)
                {
                    this._rootLabels = this._rootLabels.Union(this.Defaults.RootLabels).ToArray();
                }
                if (this.Defaults?.RootLabelsExcluded?.Length > 0)
                {
                    this._rootLabels = this._rootLabels.Union(
                        this.Labels.SelectMany(label =>
                        Directory.EnumerateDirectories(Path.Combine(LibSetting.Directories.DOWNLOAD, label))
                                .Select(subDirectory => subDirectory.Replace(LibSetting.Directories.DOWNLOAD, "").Trim('\\'))
                                .Where(s => !this.Defaults.RootLabelsExcluded.Contains(s))))
                                .ToArray();
                }
            }



            public RootDirectoryInfo GetRootDirectoryInfo(string startFolder)
                => new RootDirectoryInfo(startFolder, this.RootLabels);
            public bool Filter(string label, Toggle TOGGLES)
            {
                var debug = LogUtils.DEBUGS.LABEL_FILTERS;
                var doFilterIncludes = TOGGLES.Filters.Include && this.Include.Any();
                var doFilterExcludes = TOGGLES.Filters.Exclude && this.Exclude.Any();
                if (!TOGGLES.Filters.Global || (!doFilterIncludes && !doFilterExcludes)) {
                    return true;
                }
                if (this.FilterResults.ContainsKey(label)) {
                    return this.FilterResults[label];
                }

                if (doFilterIncludes) {
                    var matches = this.Include.Regex(label);
                    if (matches.Count == 0) {
                        if (debug) {
                            LogLabelFilter(nameof(this.Include), "  Failed", label);
                        }
                        return this.FilterResults[label] = false;
                    }
                    if (debug) {
                        LogLabelFilter(nameof(this.Include), "+ " + matches[0].Value, label);
                    }
                }

                if (doFilterExcludes) {
                    var matches = this.Exclude.Regex(label);
                    if (matches.Count > 0) {
                        if (debug) {
                            LogLabelFilter(nameof(this.Exclude), "- " + matches[0].Value);
                        }
                        return this.FilterResults[label] = false;
                    }
                }
                return this.FilterResults[label] = true;
            }

            public void Update(string label = null)
            {
                if (!string.IsNullOrEmpty(label))
                {
                    Add(label);
                }
                IEnumerable<string> labels = this.Generated;
                if (this.Queue != null)
                {
                    labels = labels.Union(this.Queue);
                }
                this.Collection = new ObservableCollection<string>(labels.OrderBy(s => s));
            }
            public async Task GenerateLabels(Action<int, TimeSpan> OnComplete = null)
            {
                var stopwatch = Stopwatch.StartNew();
                await ProcessEx.RunAsync(new ProcessStartInfo
                {
                    FileName = this.GeneratorFileName,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                });
                this.Generated = File.ReadAllLines(this.GeneratorOutput).Select(l => this.GeneratorPrefix + l).ToArray();
                Update();
                OnComplete?.Invoke(this.Generated.Length, stopwatch.Elapsed);
            }

            public bool Add(TorrentLabel label)
                => Add(label.Base);
            public bool Add(string label)
            {
                if (!this.Queue.Contains(label) && !this.Generated.Contains(label))
                {
                    this.Queue.Add(label);
                    return true;
                }
                return false;
            }
            #endregion

            #region Logging 

            [Conditional("TRACE_LABEL_FILTERS")]
            private static void LogLabelFilter(string title, string text = null, string item = null,
                                               PadDirection textPadDirection = PadDirection.Default,
                                               string textSuffix = null,
                                               PadDirection titlePadDirection = PadDirection.Default,
                                               string titleSuffix = null, int random = 0)
                =>
                    LogUtils.Log("Label.Filters", title, text, item, textPadDirection, textSuffix, titlePadDirection,
                                 titleSuffix, random);

            //[System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
            //void Log(string title, string text = null, string item = null, PadDirection textPadDirection = PadDirection.Default, string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default, string titleSuffix = null, int random = 0)
            //=> LogUtils.Log("LABEL", title, text, item, textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);
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
