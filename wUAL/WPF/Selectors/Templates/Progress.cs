using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Torrent;
using wUAL.WPF.Selectors;

namespace wUAL.WPF.Templates
{
    using Torrent;
    using static Selectors.Selectors;
    using static Selectors.Selectors.Progress;
    public class ProgressTemplateSelector : TemplateSelectorBase<UTorrentJob>
    {
        public ProgressTemplateSelector()
        {
            MakeTemplates(Names);
        }
        protected override SelectorBase<UTorrentJob> Selectors { get; }
            = Selector;
        protected override string BaseKey { get; }
            = Key;
    }
}
