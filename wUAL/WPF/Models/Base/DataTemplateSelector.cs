namespace wUAL.WPF.Models.Base
{
    using System.Windows;
    using System.Windows.Controls;

    public abstract partial class SelectorModelsBase<TItem, TModel, TState, TElement, TTemplate>
    {
        public abstract class DataTemplateSelector<T> : DataTemplateSelector
        where T : ISelectorModels<TItem, TModel, TState, TElement, TTemplate>
        {
            protected abstract T SelectorModels { get; }
            protected virtual TTemplate Template
                => default(TTemplate);
            public override DataTemplate SelectTemplate(object item, DependencyObject container)
                => this.SelectorModels?.GetTemplate(item) 
                ?? base.SelectTemplate(item, container);
        }
    }
}
