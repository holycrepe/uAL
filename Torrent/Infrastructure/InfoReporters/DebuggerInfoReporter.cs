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
        [DebuggerNonUserCode]
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
        [Conditional("DEBUG")]
        public void Noop() { }
        public object Noop(params object[] args)
            => null;
        public T Noop<T>(params T[] args)
            => default(T);
        #endregion
    }
}
