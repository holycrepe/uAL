using System;
using System.Windows.Markup;
using System.Windows.Media;
using wUAL.UserControls;

namespace wUAL
{
    public abstract class BaseControl<TControl> : MarkupExtension
    {

        public abstract TControl GetControl();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return GetControl();
        }
    }

    public class QueueGridHeaderExtension : BaseControl<QueueGridHeaderControl>
    {
        public QueueGridHeaderExtension() { }
        public QueueGridHeaderExtension(string label) : this(label, null) { }
        public QueueGridHeaderExtension(string label, string iconName) : this(label, iconName, null) { }
        public QueueGridHeaderExtension(string label, string iconName, ImageSource icon)
        {
            Label = label;
            IconName = iconName;
            Icon = icon;
        }
        [ConstructorArgument("label")]
        public string Label { get; set; }
        [ConstructorArgument("iconName")]
        public string IconName { get; set; }
        [ConstructorArgument("icon")]
        public ImageSource Icon { get; set; }
        public override QueueGridHeaderControl GetControl()
        {
            return new QueueGridHeaderControl(Label, IconName, Icon);
        }
    }
}
