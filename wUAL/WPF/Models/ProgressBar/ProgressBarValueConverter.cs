namespace wUAL.WPF.ValueConverters
{
    using Models.ProgressBar;
    using Torrent.Enums;
    using Torrent.Extensions;
    using Torrent.Helpers.Utils;
    using static Torrent.Helpers.Utils.DebugUtils;

    public abstract class ProgressBarValueConverter : BaseConverter
    {
        string _resultType = null;
        protected string ResultType
            => this._resultType ?? (this._resultType = this.GetType().Name.TrimStart("Progress").TrimEnd("Converter"));
        protected object LogResult(object value, ProgressBarModel model, object result, object parameter)
        {
            var debug = parameter?.ToString().Contains("Debug") == true;
            var log = parameter?.ToString().Contains("Log") == true || debug;
            if (log)
                Log(value, model, result);
            if (debug)
                DEBUG.Noop();
            return result;
        }
        #region Logging
        [System.Diagnostics.Conditional("LOG_PROGRESS_MODEL"), System.Diagnostics.Conditional("TRACE_EXT")]
        protected void Log(object value, ProgressBarModel model, object result, PadDirection textPadDirection = PadDirection.Default,
                           string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default,
                           string titleSuffix = null, int random = 0)
        {
#if DEBUG || TRACE_EXT
            //var item = value as UTorrentJob;
            if (value == null)
                return;

            var strValue = DebugUtils.GetDebuggerDisplay(value);

            LogUtils.Log(this.ResultType,
                         model.DebuggerDisplaySimple(),
                         "   =>   " + (strValue.Contains("\n") ? "\n" : "") + strValue,
                         "   -->  " + DebugUtils.GetDebuggerDisplay(result), 
                         textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);
            
#endif
        }

        #endregion
    }
}