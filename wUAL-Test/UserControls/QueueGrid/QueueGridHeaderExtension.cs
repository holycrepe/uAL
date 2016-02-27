using System.Diagnostics;
using System.Windows.Markup;
using System.Windows.Media;
using wUAL.UserControls;

namespace wUAL
{
    public class QueueGridHeaderExtension : UserControlExtensionBase<QueueGridHeaderControl>
    {
        string _name = null, _label="";
        public QueueGridHeaderExtension() { }
        public QueueGridHeaderExtension(string label) : this(label, null) { }
        public QueueGridHeaderExtension(string label, string name) : this(label, name, null) { }

        public QueueGridHeaderExtension(string label, string name, ImageSource icon)
        {
            this.Label = label;
            this.Name = name;
            this.Icon = icon;
        }

        [ConstructorArgument("label")]
        [DebuggerNonUserCode]
        public string Label {
            get { return this._label; }
            set { this._label = value; }
        }

        [ConstructorArgument("name")]
        [DebuggerNonUserCode]
        public string Name {
            get { return this._name ?? this.Label; }
            set { this._name = value; }
        }

        [ConstructorArgument("icon")]
        [DebuggerNonUserCode]
        public ImageSource Icon { get; set; }

        public override QueueGridHeaderControl GetControl() 
            => new QueueGridHeaderControl(this.Label, this.Name, this.Icon);
    }
}