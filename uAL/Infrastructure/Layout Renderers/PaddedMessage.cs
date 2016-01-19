using NLog;
using NLog.LayoutRenderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace uAL.Infrastructure
{
    using Torrent.Extensions;
    using Torrent.Helpers.Utils;


    [LayoutRenderer("paddedmessage")]
    public class PaddedMessage : LayoutRenderer
    {

    	StringBuilder Builder = null;
    	int titlePadding;
        
    	public char Char { get; set; }

        public PaddedMessage()
        {
            Char = ' ';
        }
        
        string GetLine(char c = '-', int width = 80)
		{
        	return new string(c, width);
		}

		string GetSep()
		{
			return GetLine('=');
		}
		void AppendLine(string text, string prefix="  ", int pad=0) {
			var logText = "\n" + text;
			if (text != GetLine() && text != GetSep()) {
				logText = logText.Replace("\n", "\n" + new string(' ', titlePadding+pad) + prefix);
			}
			Builder.Append(logText);
		}
		void ReportError(string title, string text, string prefix = "  ", int pad=12) {
			AppendLine((title + ": ").PadRight(pad + 2) + text, prefix, 0);
		}
		void ReportErrorTitle(string title="", string prefix = "  ", int pad = 0) {
			AppendLine(title, prefix, pad);
		}
		
		void ReportErrorText(string text="", string prefix = "  ", int pad = 10) {
			AppendLine(text, prefix, pad);
		}				

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
        	Builder = builder;
        	titlePadding = builder.Length;
            
            string formattedMessage;

            Dictionary<string, object> eventAttributes = null;
            if (logEvent.Properties.ContainsKey("Info"))
            {
                eventAttributes = (Dictionary<string, object>)logEvent.Properties["Info"];
            }
            else if (logEvent.Parameters != null && logEvent.Parameters.Count() == 1 && logEvent.Parameters[0].GetType().Equals(new Dictionary<string, object>()))
            {
                eventAttributes = (Dictionary<string, object>)logEvent.Parameters[0];
            }

            string title = logEvent.Message;
            var maxKeyLength = (logEvent.Properties.ContainsKey("DisableTitleValuePadding") ? 0 : title.Length);
            var maxKeyLengthInitial = maxKeyLength;
            var logger = logEvent.LoggerName;
//            if (logger.Contains("FSU.Mover")) {
//            	Debug.WriteLine(logEvent);
//            }
            if (eventAttributes == null || logEvent.Message != logEvent.FormattedMessage)
            {
                formattedMessage = logEvent.FormattedMessage;
            }
            else {                
                formattedMessage = title;                
                string titleValue = null;                
                string titleKey = (eventAttributes.ContainsKey("TitleValue") ? "TitleValue" : eventAttributes.ContainsKey("TitleSuffix") ? "TitleSuffix" : null);
                string titleSuffix = (titleKey == "TitleSuffix" ? " ": ":");

                if (titleKey != null)
                {
                    titleValue = (string)eventAttributes[titleKey];
                    eventAttributes.Remove(titleKey);
                }

                foreach (string key in eventAttributes.Keys)
                {
                    maxKeyLength = Math.Max(maxKeyLength, key.Length);
                }

                if (titleValue != null)
                {
                    formattedMessage = title.PadKeyValuePair(titleValue, maxKeyLength, titleSuffix);
                }
                
                foreach (var attribute in eventAttributes)
                {
                    formattedMessage += "\n" + attribute.PadKeyValuePair(maxKeyLength);
                }
            }
            

            string message = Regex.Replace(formattedMessage, @"([\r?\n])", "$1" + new string(' ', titlePadding));
            builder.Append(message);
            var ex = logEvent.Exception;
            if (ex != null) {
            	var exceptionStrings = ex.ToString().Split(new string[] { "\r\n" }, 2, StringSplitOptions.None);
				string exceptionTitle = exceptionStrings[0];
				string exceptionDetails = exceptionStrings[1];		
				ReportErrorTitle();
				ReportErrorTitle(GetSep());
				ReportError("Message", ex.Message, "# ");
				ReportError("Source", ex.Source, "# ");
				ReportErrorTitle(GetLine());
				ReportErrorTitle("Details", "# ");
				ReportErrorTitle(GetLine());
				ReportErrorText(exceptionTitle);
				ReportErrorTitle(GetLine());
				ReportErrorTitle("Trace", "# ");
				ReportErrorTitle(GetLine());
				ReportErrorText(exceptionDetails);
				ReportErrorTitle(GetSep());
				ReportErrorTitle();
            }
            Builder = null;
        }
    }
}
