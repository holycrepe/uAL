using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Torrent.Queue;
using uAL.Extensions;
using wUAL.WPF.Selectors;

namespace wUAL.WPF.Templates
{
    using Puchalapalli.Extensions.Primitives;
    using Torrent.Extensions;
    using Torrent.Helpers.Utils;
    using static Selectors.Selectors;
    public abstract class TemplateSelectorBase<T> : DataTemplateSelector
        where T : class
    {
        string _keyBase = null;
        public TemplateSelectorBase()
        {

        }

        protected void MakeTemplates(string[] names)
        {
            foreach (var name in names)
                Templates[name] = GetTemplate(name);
        }
                
        protected abstract SelectorBase<T> Selectors { get; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
            => GetTemplate(item as T);
        protected virtual DataTemplate GetTemplate(T item)
            => Selectors.GetSelections(item, Templates);
        protected DataTemplate GetTemplate(string key)
            => ResourceUtils.Get<DataTemplate>(this.KeyBase + "Template" 
                + key?.Prefix("_", "Default"));
        protected abstract string BaseKey { get; }
        protected virtual string KeyBase
            => this._keyBase ?? (this._keyBase 
            = this.GetType().Name.TrimEnd("DataTemplateSelector", "TemplateSelector"));
            
        protected DataTemplate GetTemplate(object item)
            => GetTemplate(item as T);

        protected virtual Dictionary<string, DataTemplate> Templates { get; }
            = new Dictionary<string, DataTemplate>();
    }
}
