using System;
using NLog;
using Torrent.Extensions;

namespace Torrent.Infrastructure.InfoReporters
{
    public abstract class InfoReporter
    {
        protected Logger logger;
        public Logger Logger => (logger ?? LoggingAdapter.logger);
        protected abstract void reportBanner(string title, int? width);
        protected abstract void reportText(string text);
        protected abstract void reportError(string title, string text);

        protected virtual void reportError(Exception ex, string title, string text)
        {
            reportError(title, getErrorText(ex, title, text));
        }

        public virtual string getErrorText(Exception ex, string title, string text) => (text == null ? "" : text + ": \n") + ex.GetSummaryText();

        public void ReportAndLogBanner(string title, int? width = null, bool log = true)
        {
            ReportBanner(title, width, (log ? LogLevel.Info : null));
        }

        public void ReportBanner(string title, int? width = null, LogLevel log = null)
        {
            reportBanner(title, width);
            LogText(title, log: log);
        }

        public void ReportAndLogText(string text = null, bool log = true)
        {
            ReportText(text, (log ? LogLevel.Info : null));
        }

        public void ReportText(string text = null, LogLevel log = null)
        {
            reportText(text);
            LogText(text, log: log);
        }

        public void ReportAndLogError(string title, string text = null, bool log = true)
        {
            ReportError(title, text, (log ? LogLevel.Error : null));
        }

        public void ReportError(string title, string text = null, LogLevel log = null)
        {
            reportError(title, text);
            LogText(title, text, log);
        }

        public void ReportAndLogError(Exception ex, string title, string text = null, bool log = true)
        {
            ReportError(ex, title, text, (log ? LogLevel.Error : null));
        }

        public void ReportError(Exception ex, string title, string text = null, LogLevel log = null)
        {
            reportError(ex, title, text);
            LogError(ex, title, text, log);
        }

        public virtual void ReportInfoBanner(object status = null, int size = 10, bool writeToLog = false)
        {
            ReportBanner(status?.ToString(), size, writeToLog ? LogLevel.Info : null);
        }

        public void LogText(string title, string text = null, LogLevel log = null)
        {
            if (log != null) {
                this.Logger.Log(log, title + (text == null ? "" : ": " + text));
            }
        }

        public void LogError(Exception ex, string title, string text = null, LogLevel log = null)
        {
            if (log != null) {
                this.Logger.Log(log, ex, title + getErrorText(ex, title, text));
            }
        }

        public abstract InfoReporter SetLogger(Logger logger);
    }
}
