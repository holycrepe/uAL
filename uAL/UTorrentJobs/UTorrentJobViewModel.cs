using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Telerik.Windows.Controls;
using Torrent;
using Torrent.Enums;
using Torrent.Extensions;
using Torrent.Helpers.Utils;
using Torrent.Helpers.Utils.IO;
using Torrent.Infrastructure;
using Torrent.Infrastructure.Enums;
using Torrent.Infrastructure.Reflection;
using Torrent.Properties.Settings;
using Torrent.Queue;
using uAL.Infrastructure.UI;
using uAL.Properties.Settings.LibSettings;

namespace uAL.UTorrentJobs
{
    [NotifyPropertyChanged]
    public class UTorrentJobViewModel : ViewModel<UTorrentJob, object>
    {
        #region Overrides of ViewModel
        [DebuggerNonUserCode]
        protected override object GetValueFromItem(UTorrentJob item)
            => item.Caption;
        [DebuggerNonUserCode]
        protected override void SetSelection(object value)
            => this.SetSelectedItem(value);
        #endregion
        public UTorrentJobViewModel() 
        {
            if (MainApp.DesignMode || true)
            {
                var rnd = new Random();
                var dir = LibSettings.LibSetting.Directories.ADDED;
                dir = string.IsNullOrEmpty(dir) ? @"E:\$\TORRENTS\ADDED" : dir;
                LogUtils.Writers.Design("Directory", dir);
                File.AppendAllText(Path.Combine(@"D:\Git\uAL\wUAL\bin\Debug\logs", "test.log"), $"[{DateUtils.Timestamp}] Directory: {dir}\n");
                Debug.WriteLine($"Directory: {dir}");
                var addedTorrents = FileSystemUtils.EnumerateFullNames(dir, "*.torrent", SearchOption.AllDirectories).GetEnumerator();
                for (var i = 0; i < 25; i++)
                {
                    var info = new TorrentInfo(addedTorrents.Pop())
                    {
                        RootDirectory = dir,
                        PercentComplete = rnd.Pick(rnd.NextDouble()*100),
                        Status = rnd.Pick(QueueStatus.Inactive, QueueStatus.Inactive, QueueStatus.Inactive, QueueStatus.Queued, QueueStatus.Active, QueueStatus.Pending),
                    };
                    if (info.Status.IsActive)
                        info.Progress = rnd.NextDouble()*100;
                    Items.Add(new UTorrentJob(info));
                }

                this.Items.AddRange(new []
                {
                    new UTorrentJob(new TorrentInfo(addedTorrents.Pop())
                        {
                            RootDirectory = dir,
                            Status = QueueStatus.Queued
                        })
                });
            }
            else
            {
                // this.SetEnum(ProcessQueueMethod.Parallel);
            }
        }
        #region Logging
        [DebuggerNonUserCode]        
        public override void Log(string prefix = "+", object status = null, object title = null, object text = null, object info = null, PadDirection textPadDirection = PadDirection.Default,
                       string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default,
                       string titleSuffix = null, int random = 0)
        {
#if DEBUG || TRACE_EXT
            base.Log(prefix, status, 
                title ?? this.Items.Count, text ?? Value, 
                info ?? (IsMultiple ? $"{SelectedItem?.ToString().Suffix(": ")}[{SelectedItems?.GetDebuggerDisplaySimple()}]" : SelectedItem?.ToString()), 
                textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);
#endif
        }

        #endregion
    }
}