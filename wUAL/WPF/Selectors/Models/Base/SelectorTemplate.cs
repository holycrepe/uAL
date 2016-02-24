using System;
using System.Windows;
using System.Windows.Markup;
using AddGenericConstraint;
using Torrent.Infrastructure.Enums;

namespace wUAL.WPF.Selectors.Models.Base
{
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