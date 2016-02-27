namespace wUAL.WPF.Models.Base
{
    using System.Windows;
    using Torrent.Infrastructure.Enums;

    public abstract partial class SelectorModelsBase<TItem, TModel, TState, TElement, TTemplate>
    {
        public class SelectorTemplate : EnumKeyValuePair<TTemplate,DataTemplate>
        {
            
        }
        public class SelectorStyle: EnumKeyValuePair<TElement, Style>
        {

        }
    }
}