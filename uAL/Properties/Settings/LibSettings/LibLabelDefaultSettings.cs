
#define TRACE_LABEL_FILTERSX

// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.LibSettings
{
    using System;
    using System.Xml.Serialization;
    using PostSharp.Patterns.Model;
    using Torrent.Properties.Settings;
    using System.IO;
    using Torrent.Extensions;
    using System.Runtime.Serialization;
    using static Torrent.Helpers.Utils.DebugUtils;
    public sealed partial class LibSettings
    {
        [Serializable]
        [XmlSerializerAssembly("uAL.XmlSerializers")]
        [DataContract(Name="DefaultLabels", Namespace="")]
        [NotifyPropertyChanged]
        internal class LibLabelDefaultSettings : BaseSettings
        {
            [DataMember(Name="Root")]
            string _strRootLabels {
                get { return this.RootLabels.ToIndentedList(); }
                set { this.RootLabels = value.SplitLinesTrimmed();  }
            }
            [DataMember(Name="RootExcluded")]
            string _strRootLabelsExcluded
            {
                get { return this.RootLabelsExcluded.ToIndentedList(); }
                set { this.RootLabelsExcluded = value.SplitLinesTrimmed(); }
            }
            [DataMember(Name="WordsToStrip")]
            string _strLabelsWordsToStrip
            {
                get { return this.LabelsWordsToStrip.ToIndentedList(); }
                set { this.LabelsWordsToStrip = value.SplitLinesTrimmed(); }
            }
            [DataMember(Name="WordsToStripExtended")]
            string _strLabelsWordsToStripExtended
            {
                get { return this.LabelsWordsToStripExtended.ToIndentedList(); }
                set { this.LabelsWordsToStripExtended = value.SplitLinesTrimmed(); }
            }
            [DataMember(Name="TorrentWordsToStrip")]
            string _strTorrentWordsToStrip
            {
                get { return this.TorrentWordsToStrip.ToIndentedList(); }
                set { this.TorrentWordsToStrip = value.SplitLinesTrimmed(); }
            }
            string[] _rootLabels, _rootLabelsExcluded, _labelsWordsToStrip, _labelsWordsToStripExtended, _torrentWordsToStrip;
            internal string[] RootLabels {
                get { return this._rootLabels ?? new string[0]; }
                private set { this._rootLabels = value; }
            }
            internal string[] RootLabelsExcluded
            {
                get { return this._rootLabelsExcluded ?? new string[0]; }
                private set { this._rootLabelsExcluded = value; }
            }
            internal string[] LabelsWordsToStrip
            {
                get { return this._labelsWordsToStrip ?? new string[0]; }
                private set { this._labelsWordsToStrip = value; }
            }
            internal string[] LabelsWordsToStripExtended
            {
                get { return this._labelsWordsToStripExtended ?? new string[0]; }
                private set { this._labelsWordsToStripExtended = value; }
            }
            internal string[] TorrentWordsToStrip
            {
                get { return this._torrentWordsToStrip ?? new string[0]; }
                private set { this._torrentWordsToStrip = value; }
            }

            [Pure]
            static string[] GetList(string file)
            {
                var fileName = MainApp.MakeRelativePath($"Settings.{file}.txt");
                if (File.Exists(fileName))
                {
                    return File.ReadAllLines(fileName);
                }
                return new string[0];
            }
            internal void GetLists()
            {
                RootLabels = GetList(nameof(RootLabels));
                RootLabelsExcluded = GetList(nameof(RootLabelsExcluded));
                LabelsWordsToStrip = GetList(nameof(LibLabelSettings.WordsToStrip));
                LabelsWordsToStripExtended = GetList(nameof(LibLabelSettings.WordsToStripExtended));
                TorrentWordsToStrip = GetList(nameof(LibLabelSettings.TorrentNameWordsToStrip));
            }

            public LibLabelDefaultSettings() : base() {
                DEBUG.Noop();
            }
        }
    }
}
