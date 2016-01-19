namespace Torrent.Infrastructure.InfoReporters
{
    using System;
    using Extensions;
    using NLog;
    using Helpers.Utils;

    public abstract class TextInfoReporter : InfoReporter
    {
        protected virtual int DefaultWidth => 120;

        protected override void reportError(string title, string text)
            => WriteError(title, text);


        protected override void reportBanner(string title, int? width)
            => WriteInfoBanner(title.PadCenter((int) (width ?? DefaultWidth)));

        protected override void reportText(string text)
            => WriteLine(text);


        public string GetBanner(int width = -1, char chr = '=')
            => new string(chr, width < 1 ? DefaultWidth - 2 : width) + "\n";
        public string GetBannerText(string text, int width = -1, char chr = '=')
            => string.Format("{0}{1}\n{0}", GetBanner(width, chr), text);

        protected abstract void DoWrite(string text);

        protected virtual void DoWriteLine(string text)
            => DoWrite(text + "\n");

        public virtual void Write(string text, string suffix = "", string prefix = "")
            => DoWrite(prefix + text + suffix);

        public virtual void WriteLine(string text="", string suffix = "", string prefix = "")
            => DoWriteLine(prefix + text + suffix);

        public virtual void WriteErrorLine(string text, string suffix = "", string prefix = "")
            => WriteLine(text, suffix, prefix);
        public virtual void WriteErrorTitle(string text, string suffix = "", string prefix = "")
            => WriteLine(text, suffix, prefix);

        public virtual void WriteError(string title, string text, string sep = "", string suffix = "", string prefix = "")
        {
            WriteErrorTitle(title, sep, prefix);
            WriteErrorLine(text, suffix);
        }
        protected void DoWriteException(Exception ex)
        {
            var exceptionStrings = ex.ToString().Replace("\n", "\n#           ").Split(new string[] { "\r\n" }, 2, StringSplitOptions.None);
            var exceptionTitle = exceptionStrings[0];
            var exceptionDetails = exceptionStrings[1];

            WriteLine("########");            
            WriteErrorTitle("Message:", suffix: "  ", prefix: "# ");
            WriteErrorLine(ex.Message);
            WriteErrorTitle("Source: ", suffix: "  ", prefix: "# ");
            WriteErrorLine(ex.Source);
            Write("#-------\n# ");
            WriteErrorTitle("Details: ", suffix: "\n");
            Write("#-------\n#           ");
            WriteErrorLine(exceptionTitle);
            Write("#-------\n# ");
            WriteErrorTitle("Trace: ", suffix: "\n");
            WriteLine("#-------");
            WriteLine(exceptionDetails);
            WriteLine("########");
            WriteLine();
        }
        public virtual void WriteInfoLine(string text, string suffix = "", string prefix = "")
            => WriteLine(text, suffix, prefix);
        public virtual void WriteInfoTitle(string text, string suffix = "", string prefix = "")
            => WriteLine(text, suffix, prefix);
        public void WriteInfoBanner(string text, int width = -1, char chr = '=')
        {
            var banner = GetBanner(width, chr);
            WriteInfoTitle(banner);
            WriteInfoLine(text);
            WriteInfoTitle(banner);
        }
        
    }
}