namespace Torrent.Extensions
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using JetBrains.Annotations;
    using NLog;


    public static class LoggerExtensions
    {
        [StringFormatMethod("message")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Conditional("TRACE_EXT")]
        public static void TRACE<TArgument1, TArgument2>(this Logger logger, [Localizable(false)] string message,
                                                         TArgument1 argument1, TArgument2 argument2)
            => logger.Trace(message, argument1, argument2);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Conditional("DEBUG"), Conditional("TRACE_EXT")]
        public static void LOG(this Logger logger, LogEventInfo info)
            => logger.Log(info);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Conditional("DEBUG"), Conditional("TRACE_EXT")]
        public static void INFO(this Logger logger, LogEventInfo info)
            => logger.Info(info);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Conditional("DEBUG"), Conditional("TRACE_EXT")]
        public static void INFO(this Logger logger, string message)
            => logger.Info(message);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Conditional("DEBUG"), Conditional("TRACE_EXT")]
        [StringFormatMethod("message")]
        public static void WARN(this Logger logger, string message, string argument)
            => logger.Warn(message, argument);


        [EditorBrowsable(EditorBrowsableState.Never)]
        [Conditional("DEBUG"), Conditional("TRACE_EXT")]
        public static void ERROR(this Logger logger, Exception ex, string message)
            => logger.Error(ex, message);

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Conditional("DEBUG"), Conditional("TRACE_EXT")]
        [StringFormatMethod("message")]
        public static void WARN<TArgument1, TArgument2, TArgument3>(this Logger logger,
                                                                    [Localizable(false)] string message,
                                                                    TArgument1 argument1, TArgument2 argument2,
                                                                    TArgument3 argument3)
            => logger.Warn(message, argument1, argument2, argument3);
    }
}
