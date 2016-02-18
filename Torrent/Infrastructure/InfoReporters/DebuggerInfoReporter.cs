namespace Torrent.Infrastructure.InfoReporters
{
    using System;
    using System.Diagnostics;
    using NLog;

    public class DebuggerInfoReporter : TextInfoReporter
    {
        #region Overrides of TextInfoReporter

        public override InfoReporter SetLogger(Logger logger)
        {
            this.logger = logger;
            return this;
        }

        [Conditional("DEBUG")]
        [DebuggerHidden]
        [DebuggerNonUserCode]
        [DebuggerStepThrough]
        public void Break()
            => Debugger.Break();

        [Conditional("DEBUG")]
        protected void doWrite(string text)
            => Debug.Write(text);

        [Conditional("DEBUG")]
        protected void doWriteLine(string text = "", string suffix = "", string prefix = "")
            => base.WriteLine(text, suffix, prefix);

        protected override void DoWrite(string text)
            => doWrite(text);

        public override void WriteLine(string text = "", string suffix = "", string prefix = "")
            => doWriteLine(text, suffix, prefix);
        [Conditional("DEBUG_NEVER_EXECUTE")]
        public void Noop(params object[] args) { }
        #endregion
    }
}
