using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using AddGenericConstraint;
using Torrent.Extensions;
using Torrent.Extensions.UI;
using Torrent.Helpers.Utils;
using uAL.Extensions;

namespace wUAL.WPF.Selectors.Models.Base
{    
    [ContentProperty(nameof(Models))]
    public abstract partial class SelectorModelsBase<TItem, TModel, [AddGenericConstraint(typeof(Enum))] TState, [AddGenericConstraint(typeof(Enum))] TElement, [AddGenericConstraint(typeof(Enum))] TTemplate>
        where TItem: class
        where TModel :  class, ISelectorModel<TState, TTemplate>
        where TState : struct
        where TElement : struct
        where TTemplate : struct
    {
        public SelectorModelsBase()
        {
            foreach (var state in this.States)
                MakeStyle(state, this.GeneratedStyles[state] = new Dictionary<TElement, Style>());
        }

        public virtual void MakeStyle(TState state, Dictionary<TElement, Style> styles)
        {
            
        }

        [DebuggerNonUserCode]
        protected TElement[] Elements { get; }
            = EnumUtils.GetValuesArray<TElement>();
        [DebuggerNonUserCode]
        protected TState[] States { get; }
            = EnumUtils.GetValuesArray<TState>();

        public abstract IEnumerable<TState> GetMatchingStates(TItem item);
        public virtual SelectorModelCollection Models { get; set; }
            = new SelectorModelCollection();
        [DebuggerNonUserCode]
        public virtual SelectorStyleCollection Styles { get; set; }
            = new SelectorStyleCollection();
        [DebuggerNonUserCode]
        public virtual SelectorTemplateCollection Templates { get; set; }
            = new SelectorTemplateCollection();
        [DebuggerNonUserCode]
        protected Dictionary<TState, Dictionary<TElement, Style>> GeneratedStyles { get; }
            = new Dictionary<TState, Dictionary<TElement, Style>>();

        [DebuggerNonUserCode]
        protected TElement DefaultElement { get; }
            = EnumUtils.GetDefault<TElement>();

        [DebuggerNonUserCode]
        protected TState DefaultState { get; }
            = EnumUtils.GetDefault<TState>();

        [DebuggerNonUserCode]
        protected TTemplate DefaultTemplate { get; }
            = EnumUtils.GetDefault<TTemplate>();
        protected IEnumerable<TModel> GetMatchingModels(TItem item)
        => GetMatchingStates(item)
            .Where(Models.Contains)
            .Select(Models.Get);

        public TModel Get(object item)
            => Get(item as TItem);
        public TModel Get(TItem item)
            => GetMatchingModels(item)
            .FirstOrDefault(value => value != null);

        [DebuggerNonUserCode]
        public Style GetStyle(object item)
        => GetStyle(item as TItem);
        [DebuggerNonUserCode]
        public Style GetStyle(object item, TElement element)
        => GetStyle(item as TItem, element);
        [DebuggerNonUserCode]
        public Style GetStyle(TItem item)
        => GetStyle(item, this.DefaultElement);

        public Style GetStyle(TItem item, TElement element)
            => GetMatchingStates(item)
            .Select(GeneratedStyles.Get)
            .Select(styles => styles.Get(element))
            .FirstOrDefault(value => value != null);
        public DataTemplate GetTemplate(TItem item)
            => GetMatchingStates(item)
            .Where(Models.Contains)
            .Select(Models.GetValue)
            .Select(GetTemplate)
            .FirstOrDefault(value => value != null);
        [DebuggerNonUserCode]
        public DataTemplate GetTemplate(TTemplate template)
        => this.Templates.GetValue(template);

        [DebuggerNonUserCode]
        public Style GetBaseStyle(TElement element)
        => this.Styles.GetValue(element);

        [DebuggerNonUserCode]
        public Style CreateStyle(TElement element)
        => GetBaseStyle(element).New();


        [DebuggerNonUserCode]
        protected static T Load<T>()
            where T : class
            => ResourceUtils.Get<T>();

    }
    
}