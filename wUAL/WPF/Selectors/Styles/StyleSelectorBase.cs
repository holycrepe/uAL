using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Torrent.Extensions;
using Torrent.Extensions.Collection;
using Torrent.Queue;
using uAL.Extensions;
using wUAL.WPF.Selectors;

namespace wUAL.WPF.Styles
{
    using Puchalapalli.Extensions.Collections;
    using Puchalapalli.Extensions.Primitives;
    using Torrent.Helpers.Utils;
    using static Selectors.Selectors;
    public abstract class StyleSelectorBase<T> : StyleSelector
        where T : class
    {
        public StyleSelectorBase()
        {
            BaseStyle = ResourceUtils.Get<Style>(this.KeyBase + this.KeySuffix.Prefix("_"));
            if (this.BaseStyle == null)
                Debugger.Break();
            foreach (var name in Names)
                MakeStyles(name);
        }
        protected virtual void MakeStyles(string name) { }
        protected abstract SelectorBase<T> Selectors { get; }

        protected virtual string[] Names { get; }
            = new string[0];
        [DebuggerNonUserCode]
        public override Style SelectStyle(object item, DependencyObject container)
            => SelectStyle(item as T);
        [DebuggerNonUserCode]
        public virtual Style SelectStyle(T item)
            => Select(item, this.Styles);
        [DebuggerNonUserCode]
        public virtual object SelectContent(object item)
            => Select(item as T, this.Content);
        [DebuggerNonUserCode]
        public virtual TSelection Select<TSelection>(T item, Dictionary<string, TSelection> selections)
            => Selectors.GetSelections(item, selections);
        [DebuggerNonUserCode]
        protected abstract string KeyBase { get; }
        [DebuggerNonUserCode]
        protected virtual string KeySuffix { get; } = "";
        [DebuggerNonUserCode]
        public Style SelectStyle(object item)
            => SelectStyle(item as T);
        [DebuggerNonUserCode]
        protected Style GenerateStyle()
            => new Style
            {
                BasedOn = this.BaseStyle,
                TargetType = this.BaseStyle?.TargetType
            };
        [DebuggerNonUserCode]
        protected string MakeResourceKey(params string[] keys)
            => this.KeyBase + KeySuffix.Prefix("_") + keys.Select(s => s.Prefix("_")).Join("").TrimEnd("_Default");

        [DebuggerNonUserCode]
        public object GetResource(params string[] keys)
        => GetResource<object>(keys);
        [DebuggerNonUserCode]
        public TResource GetResource<TResource>(params string[] keys)
            where TResource : class
        => ResourceUtils.Get<TResource>(MakeResourceKey(keys));
        public TResource GetResource<TResource>(DependencyProperty property, params string[] keys)
            where TResource : class
        => GetResource<TResource>(keys.Before(property.Name));

        protected Style GenerateStyle(string selectorName, params DependencyProperty[] properties)
        {
            if (this.BaseStyle == null)
                Debugger.Break();
            var style = GenerateStyle();
            foreach (var property in properties)
                style.AddSetter(property, GetResource(property.Name, selectorName));
            return style;
        }
        protected Style GenerateColorStyle(string selectorName)
        => GenerateStyle(selectorName, Control.ForegroundProperty, Control.BackgroundProperty);
        protected Style GenerateForegroundStyle(string selectorName)
        => GenerateStyle(selectorName, Control.ForegroundProperty);
        protected Style GenerateBackgroundStyle(string selectorName)
        => GenerateStyle(selectorName, Control.BackgroundProperty);
        protected Style GenerateStyle(string background, string foreground = null)
        {
            if (this.BaseStyle == null)
                Debugger.Break();
            var style = GenerateStyle();
            style.AddForeground(foreground);
            style.AddBackground(background);
            return style;
        }

        [DebuggerNonUserCode]
        protected Style BaseStyle { get; }
        [DebuggerNonUserCode]
        protected virtual Dictionary<string, Style> Styles { get; }
            = new Dictionary<string, Style>();
        [DebuggerNonUserCode]
        protected virtual Dictionary<string, object> Content { get; }
            = new Dictionary<string, object>();
    }
}
