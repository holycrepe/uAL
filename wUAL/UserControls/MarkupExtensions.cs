using System;
using System.Windows.Markup;
using System.Windows.Media;
using wUAL.UserControls;

namespace wUAL
{
    public abstract class BaseControl<TControl> : MarkupExtension
    {
        public abstract TControl GetControl();

        public override object ProvideValue(IServiceProvider serviceProvider) { return GetControl(); }
    }

    public class QueueGridHeaderExtension : BaseControl<QueueGridHeaderControl>
    {
        string _name = null, _label="";
        public QueueGridHeaderExtension() { }
        public QueueGridHeaderExtension(string label) : this(label, null) { }
        public QueueGridHeaderExtension(string label, string name) : this(label, name, null) { }

        public QueueGridHeaderExtension(string label, string name, ImageSource icon)
        {
            Label = label;
            Name = name;
            Icon = icon;
        }

        [ConstructorArgument("label")]
        public string Label {
            get { return this._label; }
            set { this._label = value; }
        }

        [ConstructorArgument("name")]
        public string Name {
            get { return this._name ?? this.Label; }
            set { this._name = value; }
        }

        [ConstructorArgument("icon")]
        public ImageSource Icon { get; set; }

        public override QueueGridHeaderControl GetControl()
        {
            return new QueueGridHeaderControl(Label, Name, Icon);
        }
    }
}
