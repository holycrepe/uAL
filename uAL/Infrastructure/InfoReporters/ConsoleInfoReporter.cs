namespace uAL.Infrastructure
{
    using System;
    using NLog;
    using Torrent.Extensions;
    using Torrent.Helpers.Utils;
    using Torrent.Infrastructure.InfoReporters;

    public class ConsoleInfoReporter : InfoReporter
    { 

        public override InfoReporter SetLogger(Logger logger) {
            this.logger = logger;
            return this;
        }
    	
        protected  override void reportError(string title, string text)
        {
            ColoredConsole.WriteError(title, text);
        }

        protected  override void reportBanner(string title, int? width)
        {
            if (!width.HasValue)
            {
                width = Console.WindowWidth - 2;
            }
            ColoredConsole.WriteInfoBanner(title.PadCenter((int)width));
        }

        protected override void reportText(string text)
        {
            throw new NotImplementedException();
        }
    }
}