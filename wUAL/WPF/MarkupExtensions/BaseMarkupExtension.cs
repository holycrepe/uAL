using System;
using System.Windows.Markup;

namespace wUAL
{
    public abstract class BaseMarkupExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
            => this;
    }
}