using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Telerik.Windows.Controls;
using NLog;

namespace wUAL.Infrastructure
{
    using Torrent.Extensions;
    using Torrent.Infrastructure.InfoReporters;

    public class ListBoxInfoReporter : InfoReporter
    {

        readonly RadListBox radListStatus;  
        object lastInfoTitle;

        public ListBoxInfoReporter(RadListBox radListBox)
        {
        	radListStatus = radListBox;
        }        
        
        public override InfoReporter SetLogger(Logger logger) {
    		this.logger = logger;
    		return this;
    	}

        protected override void reportError(Exception ex, string title, string text)
        {
            var exceptionStrings = ex.ToString().Replace("\n", "\n#           ").Split(new string[] { "\r\n" }, 2, StringSplitOptions.None);
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

        protected override void reportError(string title, string text) {
        	if (title != null) {
				ReportErrorTitle(title);
			}
			if (text != null) {
				ReportErrorText(text);
			}
        }
        
        protected  override void reportBanner(string title, int? width)
        {            
            var size = width ?? 10;
            ReportInfoBanner(title.PadCenter(size * 10), size);
        }

        protected  override void reportText(string text)
        {
            ReportStatus(text);
        }
        public Action<string> doLog { get; set; }
        public void ReportStatus(object status = null, bool writeToLog = false, string BackgroundHex = null, string group=null, Action<string> log = null)
		{
			
			Brush Background = (BackgroundHex == null ? null : new SolidColorBrush((Color)ColorConverter.ConvertFromString(BackgroundHex)));
			var statusItem = new RadListBoxItem();
			if (Background != null) {
				statusItem.Background = Background;
			}
			if (status != null) {
				statusItem.Content = status;
			}           
			
//			Collection.Add(statusEntry);
			radListStatus.Items.Add(statusItem);
			if (writeToLog) {
				Logger.INFO(status.ToString());
			}
			if (log != null) {
				log(status.ToString());
			}
			
			if (VisualTreeHelper.GetChildrenCount(radListStatus) > 0)
			{
				var grid = VisualTreeHelper.GetChild(radListStatus, 0) as Grid;
			    var scrollViewer = VisualTreeHelper.GetChild(grid, 0) as ScrollViewer;
			    scrollViewer.ScrollToBottom();
			}
		}
        public void ReportInfo(object title = null, object text = null, bool writeToLog = false, bool repeatTitles = false) {        	
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
        	ReportStatus(BackgroundHex: "#004080", status: status, writeToLog: writeToLog);
		}
		public void ReportInfoText(object status = null, bool writeToLog = false)
		{
			ReportStatus(BackgroundHex: "#0060BF", status: status, writeToLog: writeToLog);
		}
		public void ReportInfoBanner(object status = null, int size = 10, bool writeToLog = false)
		{

			ReportInfoText(GetLine(height: size));
			ReportInfoTitle(status);
			ReportInfoText(GetLine(height: size));

			if (writeToLog) {
				Logger.INFO(new string('-', size * 10) + "\n" + status.ToString() + "\n" + new string('-', size * 10) + "\n");
			}
		}
		public void ReportErrorTitle(object status = null, bool writeToLog = false)
		{
			ReportStatus(BackgroundHex: "#741111", status: status, writeToLog: writeToLog);
		}

		public void ReportErrorText(object status = null, bool writeToLog = false)
		{
			ReportStatus(BackgroundHex: "#420505", status: status, writeToLog: writeToLog);
		}

		Line GetLine(int height = 3)
		{
			var line = new Line();
			line.Height = height;
			return line;
		}

		Line GetSep()
		{
			return GetLine(10);
		}
    }
    
}
