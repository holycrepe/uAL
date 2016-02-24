using System;
using System.Windows;
using System.Windows.Markup;
using AddGenericConstraint;

namespace wUAL.WPF.Selectors.Models.Base
{
    public abstract partial class SelectorModelsBase<TItem, TModel, TState, TElement, TTemplate>
    {
        public class SelectorModelCollection
            : SelectorCollection<TModel, TState, TTemplate>
        { }
        public class SelectorStringCollection
            : SelectorCollection<TState, string>
        { }
        public class SelectorStyleCollection
            : SelectorCollection<SelectorStyle, TElement, Style>
        { }
        public class SelectorTemplateCollection
            : SelectorCollection<SelectorTemplate, TTemplate, DataTemplate>
        { }
    }
}