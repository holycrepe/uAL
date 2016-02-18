using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Torrent.Helpers.Utils;

namespace Torrent.Infrastructure
{
    using DictExtensions = global::Torrent.Extensions.DictExtensions;

    public class LogEventInfoCloneable : LogEventInfo
    {
        public LogEventInfoCloneable(LogLevel level, string loggerName, [Localizable(false)] string message,
                                     string group = null, string subject = null,
                                     Dictionary<string, object> eventInfo = null) : base(level, loggerName, message)
        {
            if (group != null) {
                Properties["Group"] = group;
            }
            if (subject != null) {
                Properties["Subject"] = subject;
            }
            if (eventInfo != null) {
                Properties["Info"] = eventInfo;
            }
        }

        public LogEventInfoCloneable SetError(Exception e, bool setGroup = true)
        {
            Exception = e;
            return SetError(setGroup);
        }

        public LogEventInfoCloneable SetError(bool setGroup = true)
        {
            Level = LogLevel.Error;
            if (setGroup) {
                Properties["Group"] = "Error";
            }
            return this;
        }

        public LogEventInfoCloneable Clone(Exception ex, Dictionary<string, object> newEventDict)
        {
            return Clone(newEventDict: newEventDict, ex: ex);
        }

        public LogEventInfoCloneable Clone(Dictionary<string, object> newEventDict = null)
        {
            return Clone(null, newEventDict);
        }

        public LogEventInfoCloneable Clone(Exception ex, string newMessage = null,
                                           Dictionary<string, object> newEventDict = null, string[] keysToDelete = null,
                                           string group = null, string subject = null, string titleSuffix = null,
                                           string titleValue = null, LogLevel level = null)
        {
            return Clone(newMessage, newEventDict, keysToDelete, group, subject, titleSuffix, titleValue, level, ex);
        }

        public LogEventInfoCloneable Clone(string newMessage = null, Dictionary<string, object> newEventDict = null,
                                           string[] keysToDelete = null,
                                           string group = null, string subject = null, string titleSuffix = null,
                                           string titleValue = null,
                                           LogLevel level = null, Exception ex = null)
        {
            var newEvent = new LogEventInfoCloneable(Level, LoggerName, (newMessage ?? Message));
            DictExtensions.Concat(newEvent.Properties, Properties);
            var eventDict = new Dictionary<string, object>();
            if (newEvent.Properties.ContainsKey("Info")) {
                DictExtensions.Concat(eventDict, (Dictionary<string, object>) newEvent.Properties["Info"]);
            }

            if (level != null) {
                newEvent.Level = level;
            }
            if (group != null) {
                newEvent.Properties["Group"] = group;
            }
            if (subject != null) {
                newEvent.Properties["Subject"] = subject;
            }
            if (newEventDict == null && (titleSuffix != null || titleValue != null)) {
                newEventDict = new Dictionary<string, object>();
            }
            if (titleSuffix != null) {
                newEventDict["TitleSuffix"] = titleSuffix;
            }
            if (titleValue != null) {
                newEventDict["TitleValue"] = titleValue;
            }
            if (newEventDict != null) {
                DictExtensions.Concat(eventDict, newEventDict);
                newEvent.Properties["Info"] = eventDict;
            }

            if (keysToDelete != null) {
                foreach (var key in keysToDelete) {
                    if (eventDict.ContainsKey(key)) {
                        eventDict.Remove(key);
                    }
                }
                newEvent.Properties["Info"] = eventDict;
            }

            if (ex != null) {
                newEvent.Exception = ex;
            }

            return newEvent;
        }
    }
}
