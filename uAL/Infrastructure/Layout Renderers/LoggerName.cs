using NLog;
using NLog.LayoutRenderers;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace uAL.Infrastructure
{


    public abstract class LoggerNameBase: LayoutRenderer
    {

        public LoggerNameBase()
        {
            Delimiter = ".";
            Offset = 0;
            Count = 0;
            ReverseGroup = false;
            ShortName = false;
            Label = null;
        }

        internal string MyLogger() {
            var name = Regex.Replace(eventInfo.LoggerName, @"^(Default|Plain|Simple)\.(.+)", "$2");

            if (Label != null)
            {
                name += "." + Label;
            }

            string[] keys = new [] { "Group", "Subject" };
        
            if (ReverseGroup)
            {
                keys = keys.Reverse().ToArray();
            }
            foreach (var key in keys)
            {
                if (eventInfo.Properties.ContainsKey(key))
                {
                    name += "." + eventInfo.Properties[key];
                }
            }
            return name;
        }
        
        public int Offset { get; set;  }
        public int Count { get; set; }
        public string Delimiter { get; set; }
        public string Label{ get; set; }
        public bool ReverseGroup { get; set; }
        public bool ShortName { get; set; }

        internal string[] names()
        {
            return MyLogger().Split('.');
        }
        internal string shortName()
        {
            var names = this.names(); return names[names.Length - 1];
        }
        internal string baseName()
        {
            
                var myLogger = MyLogger();
                var shortName = this.shortName();
                if (myLogger == shortName)
                {
                    return myLogger;
                }
                return myLogger.Substring(0, myLogger.Length - shortName.Length - 1);
            
        }

        internal void AppendLevel(StringBuilder builder, LogEventInfo logEvent, int level)
        {
            eventInfo = logEvent;
            var names = this.names();
            if (names.Length > level)
            {
                builder.Append(names[level - 1]);
            }
        }

        protected virtual LogEventInfo eventInfo{ get; set; }
    }

    [LayoutRenderer("mylogger")]
    public class MyLogger : LoggerNameBase
    {

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            eventInfo = logEvent;
            var names = base.names().ToList();

            if (ShortName)
            {
                names = names.GetRange(names.Count - 1, 1);
            }
            else if (Offset != 0 || Count != 0)
            {
                var count = (Count > 0 ? Count : names.Count + Count);
                var offset = (Offset < 0 ? Offset + names.Count : Offset);
                if (count < 0)
                {
                    count += names.Count;
                }
                offset = Math.Min(Math.Max(offset, 0), names.Count - 1);
                count = Math.Min(Math.Max(count, 0), names.Count - offset);
                names = names.GetRange(offset, count);
            }
            builder.Append(string.Join(Delimiter, names));
        }
    }

    [LayoutRenderer("myloggerbase")]
    public class MyLoggerBaseName : LoggerNameBase
    {

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            eventInfo = logEvent;
            builder.Append(baseName());
        }
    }



    [LayoutRenderer("myloggerclass")]
    public class MyLoggerClassName : LoggerNameBase
    {

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            AppendLevel(builder, logEvent, 1);
        }
    }



    [LayoutRenderer("myloggersection")]
    public class MyLoggerSectionName : LoggerNameBase
    {

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            AppendLevel(builder, logEvent, 2);
        }
    }



    [LayoutRenderer("myloggergroup")]
    public class MyLoggerGroupName : LoggerNameBase
    {

        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            AppendLevel(builder, logEvent, 3);
        }
    }
     
    
}
