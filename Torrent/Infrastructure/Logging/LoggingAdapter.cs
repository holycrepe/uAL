using System.ComponentModel;
using JetBrains.Annotations;
using NLog;
using System;

namespace Torrent.Infrastructure
{



    public static class LoggingAdapter
    {
        public static readonly Logger logger = LogManager.GetLogger("General");

        static LoggingAdapter()
        {

        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE")]
        public static void Log(LogEventInfo logEvent)
        {
            logger.Log(logEvent);
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE")]
    	[StringFormatMethod("message")]
        public static void Log(LogLevel level, string message, bool writeToConsole = false, params object[] args)
        {
        	logger.Log(level, message, args);
        	if (writeToConsole) {
        		if (Console.Out != null) {
        			Console.WriteLine(message);
        		}        		
        	}
        }
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE")]
        public static void Log(LogLevel level, string message)
        {
        	logger.Log(level, message);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE")]
        public static void Debug(string message)
        {
            logger.Log(LogLevel.Debug, message);
        }

        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE")]
        [EditorBrowsable(EditorBrowsableState.Never)]
    	[StringFormatMethod("message")]
        public static void Info(string message, bool writeToConsole = false, params object[] args)
        {
            logger.Log(LogLevel.Info, message, writeToConsole, args);
        }

        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void Info(string message, params object[] args)
        {
            logger.Log(LogLevel.Info, message, args);
        }
        [System.Diagnostics.Conditional("TRACE")]
        [EditorBrowsable(EditorBrowsableState.Never)]
    	[StringFormatMethod("message")]
        public static void Trace(string message, bool writeToConsole = false, params object[] args)
        {
            logger.Log(LogLevel.Trace, message, writeToConsole, args);
        }

        [System.Diagnostics.Conditional("TRACE")]
        [EditorBrowsable(EditorBrowsableState.Never)]
    	[StringFormatMethod("message")]
        public static void Trace(string message, params object[] args)
        {
            logger.Log(LogLevel.Trace, message, args);
        }
        
    	[StringFormatMethod("message")]
        public static void Warn(string message, bool writeToConsole = false, params object[] args)
        {
            logger.Log(LogLevel.Warn, message, writeToConsole, args);
        }

    	[StringFormatMethod("message")]
        public static void Warn(string message, params object[] args)
        {
            logger.Log(LogLevel.Warn, message, args);
        }

    	[StringFormatMethod("message")]
        public static void Error(string message,  params object[] args)
        {
            logger.Log(LogLevel.Error, message, args);
        }
        
    	[StringFormatMethod("message")]
        public static void Error(Exception ex, string message, params object[] args)
        {
            logger.Error(ex, message, args);
        }
    }
}
