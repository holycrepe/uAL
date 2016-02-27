using System;
using System.Windows.Markup;

namespace wUAL
{
    public abstract class UserControlExtensionBase<TControl> : MarkupExtension
    {
        public abstract TControl GetControl();

        public override object ProvideValue(IServiceProvider serviceProvider) => GetControl();
    }
}
