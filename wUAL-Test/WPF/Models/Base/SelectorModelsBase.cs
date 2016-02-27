namespace wUAL.WPF.Models.Base
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Windows;
    using System.Windows.Markup;
    using AddGenericConstraint;
    using Torrent.Extensions;
    using Torrent.Extensions.UI;
    using Torrent.Helpers.Utils;
    using Torrent.Infrastructure;

    [ContentProperty(nameof(Models))]
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public abstract partial class SelectorModelsBase<TItem, TModel, [AddGenericConstraint(typeof(Enum))] TState, [AddGenericConstraint(typeof(Enum))] TElement, [AddGenericConstraint(typeof(Enum))] TTemplate>
        : IDebuggerDisplay, ISelectorModels<TItem, TModel, TState, TElement, TTemplate> 
        where TItem: class
        where TModel :  class, ISelectorModel<TState, TTemplate>
        where TState : struct
        where TElement : struct
        where TTemplate : struct
    {
        private Dictionary<TState, Dictionary<TElement, Style>> _generatedStyles = null;
        public SelectorModelsBase()
        {
            
        }

        public virtual Dictionary<TState, Dictionary<TElement, Style>> GetGeneratedStyles()
        {
            if (this._generatedStyles != null)
                return this._generatedStyles;

            this._generatedStyles = new Dictionary<TState, Dictionary<TElement, Style>>();
            foreach (var state in this.States)
                MakeStyle(state, this.Generated[state] = new Dictionary<TElement, Style>());
            return this._generatedStyles;
        }
        public virtual void MakeStyle(TState state, Dictionary<TElement, Style> styles)
        {
            
        }

        #region Public Properties
        #region Public Properties: Enums
        [DebuggerNonUserCode]
        protected TElement[] Elements { get; }
            = EnumUtils.GetValuesArray<TElement>();
        [DebuggerNonUserCode]
        protected TState[] States { get; }
            = EnumUtils.GetValuesArray<TState>();
        #endregion
        #region Public Properties: Collections
        public virtual SelectorModelCollection Models { get; set; }
            = new SelectorModelCollection();
        [DebuggerNonUserCode]
        public virtual SelectorStyleCollection Styles { get; set; }
            = new SelectorStyleCollection();
        [DebuggerNonUserCode]
        public virtual SelectorTemplateCollection Templates { get; set; }
            = new SelectorTemplateCollection();

        [DebuggerNonUserCode]
        public Dictionary<TState, Dictionary<TElement, Style>> Generated
            => GetGeneratedStyles();
        #endregion        
        #endregion
        #region Get Matching Models / States
        [DebuggerNonUserCode]
        public IEnumerable<TState> GetMatchingStates(object item)
            => GetMatchingStates(item as TItem);
        public abstract IEnumerable<TState> GetMatchingStates(TItem item);
        //[DebuggerNonUserCode]
        //public IEnumerable<TModel> GetMatchingModels(object item)
        //    => GetMatchingModels(item as TItem);

        //[DebuggerNonUserCode]
        //public IEnumerable<TModel> GetMatchingModels(TItem item)
        //=> GetMatchingStates(item)
        //    .Where(Models.Contains)
        //    .Select(Models.Get);

        [DebuggerNonUserCode]
        public IEnumerable<TModel> GetMatchingModels(object item)
        => GetMatchingStates(item)
            .Where(this.Models.Contains)
            .Select(this.Models.Get);
        #endregion
        #region Get
        #region Get Item
        #region Get Item: Model
        //[DebuggerNonUserCode]
        //public TModel Get<T>(T item)
        //    => Get(item as TItem);
        //[DebuggerNonUserCode]
        //public TModel Get(TItem item)
        //    => GetMatchingModels(item)
        //    .FirstOrDefault(value => value != null);
        [DebuggerNonUserCode]
        public TModel Get(object item)
           => GetMatchingModels(item as TItem)
           .FirstOrDefault(value => value != null);
        #endregion
        #region Get Item: Style
        //[DebuggerNonUserCode]
        //public Style GetStyle(object item, TElement element = default(TElement))
        //=> GetStyle(item as TItem, element);
        //[DebuggerNonUserCode]
        //public Style GetStyle(TItem item, TElement element = default(TElement))
        //    => GetMatchingStates(item)
        //    .Select(this.Generated.Get)
        //    .Select(styles => styles.Get(element))
        //    .FirstOrDefault(value => value != null);
        [DebuggerNonUserCode]
        public Style GetStyle(object item, TElement element = default(TElement))
            => GetMatchingStates(item)
            .Select(this.Generated.Get)
            .Select(styles => styles.Get(element))
            .FirstOrDefault(value => value != null);
        #endregion
        #region Get Item: Template
        //[DebuggerNonUserCode]
        //public DataTemplate GetTemplate(object item)
        //=> GetTemplate(item as TItem);
        //[DebuggerNonUserCode]
        //public DataTemplate GetTemplate(TItem item)
        //    => GetMatchingStates(item)
        //    .Where(Models.Contains)
        //    .Select(Models.GetValue)
        //    .Select(GetTemplate)
        //    .FirstOrDefault(value => value != null);
        [DebuggerNonUserCode]
        public DataTemplate GetTemplate(object item)
            => Enumerable.Select<TTemplate, DataTemplate>(GetMatchingStates(item)
                                                 .Where(this.Models.Contains)
                                                 .Select(this.Models.GetValue), GetTemplate)
            .FirstOrDefault(value => value != null);
        [DebuggerNonUserCode]
        public DataTemplate GetTemplate(TTemplate template)
        => this.Templates.GetValue(template);
        #endregion
        #endregion
        #region Get Attached Model
        #region Get Attached Model: Style
        //[DebuggerNonUserCode]
        //public TModel GetStyleModel(object item, TElement element = default(TElement))
        //    => GetStyleModel(item as TItem, element);
        //[DebuggerNonUserCode]
        //public TModel GetStyleModel(TItem item, TElement element = default(TElement))
        //    => GetMatchingStates(item)
        //    .Where(state => Generated.Get(state)?.Get(element) != null)
        //    .Select(Models.Get)
        //    .FirstOrDefault(value => value != null);
        [DebuggerNonUserCode]
        public TModel GetStyleModel(object item, TElement element = default(TElement))
            => Enumerable.FirstOrDefault<TModel>(GetMatchingStates(item)
                                        .Where(state => this.Generated.Get(state)?.Get(element) != null)
                                        .Select(this.Models.Get), value => value != null);

        #endregion
        #region Get Attached Model: Template
        //[DebuggerNonUserCode]
        //public TModel GetTemplateModel(object item, TElement element = default(TElement))
        //    => GetTemplateModel(item as TItem, element);
        //[DebuggerNonUserCode]
        //public TModel GetTemplateModel(TItem item, TElement element = default(TElement))
        //    => GetMatchingModels(item)
        //    .Where(Templates.Contains)
        //    .FirstOrDefault();
        [DebuggerNonUserCode]
        public TModel GetTemplateModel(object item, TElement element = default(TElement))
            => Enumerable.FirstOrDefault<TModel>(GetMatchingModels(item)
                                        .Where(this.Templates.Contains));
        #endregion
        #endregion
        #endregion
        [DebuggerNonUserCode]
        public bool IsItem<T>()
            => typeof(T) == typeof(TItem);
        [DebuggerNonUserCode]
        public Style GetBaseStyle(TElement element=default(TElement))
        => this.Styles.GetValue(element);

        [DebuggerNonUserCode]
        public Style CreateStyle(TElement element = default(TElement))
        => GetBaseStyle(element).New();


        [DebuggerNonUserCode]
        protected static T Load<T>()
            where T : class
            => ResourceUtils.Get<T>();
        #region Interfaces
        #region Interfaces: IDebuggerDisplay
        [DebuggerNonUserCode]
        public override string ToString()
        => DebuggerDisplaySimple();
        public string DebuggerDisplay(int level = 1)
            => $"<{this.GetType().GetFriendlyName()}> {DebuggerDisplaySimple(level)}";
        public string DebuggerDisplaySimple(int level = 1)
        => $"{this.States.Join(", ")}";
        #endregion
        #endregion
    }

}