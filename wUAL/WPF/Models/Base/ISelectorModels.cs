namespace wUAL.WPF.Models.Base
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using AddGenericConstraint;
    using Torrent.Infrastructure;

    public interface ISelectorModels<TItem, TModel, [AddGenericConstraint(typeof(Enum))]
    TState, [AddGenericConstraint(typeof(Enum))]
    TElement, [AddGenericConstraint(typeof(Enum))]
    TTemplate> : IDebuggerDisplay
        where TItem : class
        where TModel : class, ISelectorModel<TState, TTemplate>
        where TState : struct
        where TElement : struct
        where TTemplate : struct
    {
        Dictionary<TState, Dictionary<TElement, Style>> Generated { get; }
        WPF.Models.Base.SelectorModelsBase<TItem, TModel, TState, TElement, TTemplate>.SelectorModelCollection Models { get; set; }
        WPF.Models.Base.SelectorModelsBase<TItem, TModel, TState, TElement, TTemplate>.SelectorStyleCollection Styles { get; set; }
        WPF.Models.Base.SelectorModelsBase<TItem, TModel, TState, TElement, TTemplate>.SelectorTemplateCollection Templates { get; set; }

        Style CreateStyle(TElement element = default(TElement));
        TModel Get(object item);
        Style GetBaseStyle(TElement element = default(TElement));
        Dictionary<TState, Dictionary<TElement, Style>> GetGeneratedStyles();
        IEnumerable<TModel> GetMatchingModels(object item);
        IEnumerable<TState> GetMatchingStates(TItem item);
        IEnumerable<TState> GetMatchingStates(object item);
        Style GetStyle(object item, TElement element = default(TElement));
        TModel GetStyleModel(object item, TElement element = default(TElement));
        DataTemplate GetTemplate(TTemplate template);
        DataTemplate GetTemplate(object item);
        TModel GetTemplateModel(object item, TElement element = default(TElement));
        bool IsItem<T>();
        void MakeStyle(TState state, Dictionary<TElement, Style> styles);
    }
}