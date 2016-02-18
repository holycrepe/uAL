using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using NLog;

namespace wUAL.Infrastructure
{
    using System.Collections.Generic;
    using System.Windows;
    using Torrent.Extensions;
    using Torrent.Infrastructure.InfoReporters;
    using uAL.Helpers.Utils;
    using uAL.Infrastructure;

    public class ListBoxInfoReporter : InfoReporter
    {
        static readonly Style RadListBoxItemStyle = (Style) Application.Current.FindResource("RadListBoxItemStyle");
        readonly RadListBox radListStatus;
        object lastInfoTitle;

        public ListBoxInfoReporter(RadListBox radListBox) { radListStatus = radListBox; }

        public override InfoReporter SetLogger(Logger logger)
        {
            this.logger = logger;
            return this;
        }
        public void ReportTiming(string text, bool writeToLog = false, string group = null, Action<string> log = null)
            => ReportStatus(text, writeToLog, group, log, foreground: "#66ffb2"); // #00ff7e
        protected override void reportError(Exception ex, string title, string text)
        {
            var exceptionStrings = ex.ToString()
                                     .Replace("\n", "\n#           ")
                                     .Split(new string[] {"\r\n"}, 2, StringSplitOptions.None);
            string exceptionTitle = exceptionStrings[0];
            string exceptionDetails = exceptionStrings[1];


            ReportErrorTitle(GetSep());
            ReportErrorTitle("# Message");
            ReportErrorText(ex.Message);
            ReportErrorTitle("# Source");
            ReportErrorText(ex.Source);
            ReportErrorText(GetLine());
            ReportErrorTitle("# Details");
            ReportErrorText(GetLine());
            ReportErrorText(exceptionTitle);
            ReportErrorText(GetLine());
            ReportErrorTitle("Trace: ");
            ReportErrorText(GetLine());
            ReportErrorText(exceptionDetails);
            ReportErrorTitle(GetSep());
            ReportErrorTitle();
        }

        protected override void reportError(string title, string text)
        {
            if (title != null) {
                ReportErrorTitle(title);
            }
            if (text != null) {
                ReportErrorText(text);
            }
        }

        protected override void reportBanner(string title, int? width)
        {
            var size = width ?? 10;
            ReportInfoBanner(title.PadCenter(size*10), size);
        }

        protected override void reportText(string text) { ReportStatus(text); }
        public Action<string> doLog { get; set; }
        public void ReportStatus(object status = null, bool writeToLog = false, string group = null, Action<string> log = null, string background = null, string foreground = null,
                                 IEnumerable<SetterBase> setters = null, IEnumerable<TriggerBase> triggers = null)
        {
            //Brush Foreground = (ForegroundHex == null ? null : new SolidColorBrush((Color)ColorConverter.ConvertFromString(ForegroundHex)));
            //Brush Background = (BackgroundHex == null ? null : new SolidColorBrush((Color) ColorConverter.ConvertFromString(BackgroundHex)));
            var allSetters = new List<SetterBase>();
            if (setters != null)
            {
                allSetters.AddRange(setters);
            }
            if (foreground != null)
            {
                allSetters.Add(new Setter(Control.ForegroundProperty, ColorUtils.HexToBrush(foreground)));
            }
            if (background != null)
            {
                allSetters.Add(new Setter(Control.BackgroundProperty, ColorUtils.HexToBrush(background)));
            }

            var allTriggers = new List<TriggerBase>();
            if (triggers != null)
            {
                allTriggers.AddRange(triggers);
            }
            if (allSetters.Count == 0 && allTriggers.Count == 0)
            {
                ReportStatus(status, null, writeToLog, group, log);
            }
            else {
                var style = new Style(typeof(RadListBoxItem), RadListBoxItemStyle);
                style.Setters.AddRange(allSetters);
                style.Triggers.AddRange(allTriggers);
                ReportStatus(status, style, writeToLog, group, log);
            }
        }
        public void ReportStatus(object status, Style style, bool writeToLog = false, string group = null, Action<string> log = null)
        { 
            var statusItem = new RadListBoxItem();
            if (style != null)
            {
                statusItem.Style = style;
            }

            if (status != null) {
                //var statusTextBlock = new TextBlock();
                //statusTextBlock.TextWrapping = TextWrapping.Wrap;
                //statusTextBlock.Text = status.ToString();
                statusItem.Content = status;
                statusItem.ToolTip = DateTime.Now.FormatFriendly();
            }
            
            radListStatus.Items.Add(statusItem);
            if (writeToLog) {
                Logger.INFO(status.ToString());
            }
            log?.Invoke(status.ToString());

            if (VisualTreeHelper.GetChildrenCount(radListStatus) > 0) {
                var grid = VisualTreeHelper.GetChild(radListStatus, 0) as Grid;
                var scrollViewer = VisualTreeHelper.GetChild(grid, 0) as ScrollViewer;
                scrollViewer.ScrollToBottom();
            }
        }

        public void ReportInfo(object title = null, object text = null, bool writeToLog = false,
                               bool repeatTitles = false)
        {
            ReportInfoTitle(title, writeToLog, repeatTitles);
            ReportInfoText(text, writeToLog);
        }

        public void ReportInfoTitle(object status = null, bool writeToLog = false, bool repeatTitles = false)
        {
            if (!repeatTitles) {
                if (lastInfoTitle == status) {
                    return;
                }
                lastInfoTitle = status;
            }
            ReportStatus(background: "#004080", status: status, writeToLog: writeToLog);
        }

        public void ReportInfoText(object status = null, bool writeToLog = false)
        {
            ReportStatus(background: "#0060BF", status: status, writeToLog: writeToLog);
        }

        public override void ReportInfoBanner(object status = null, int size = 10, bool writeToLog = false)
        {
            ReportInfoText(GetLine(height: size));
            ReportInfoTitle(status);
            ReportInfoText(GetLine(height: size));

            if (writeToLog) {
                Logger.INFO(new string('-', size*10) + "\n" + status.ToString() + "\n" + new string('-', size*10) + "\n");
            }
        }

        public void ReportErrorTitle(object status = null, bool writeToLog = false)
        {
            ReportStatus(background: "#741111", status: status, writeToLog: writeToLog);
        }

        public void ReportErrorText(object status = null, bool writeToLog = false)
        {
            ReportStatus(background: "#420505", status: status, writeToLog: writeToLog);
        }

        Line GetLine(int height = 3)
        {
            var line = new Line();
            line.Height = height;
            return line;
        }

        Line GetSep() { return GetLine(10); }
    }
}
