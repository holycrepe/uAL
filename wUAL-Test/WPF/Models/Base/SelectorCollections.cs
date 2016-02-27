namespace wUAL.WPF.Models.Base
{
    using System.Windows;

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