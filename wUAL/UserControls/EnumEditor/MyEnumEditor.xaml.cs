using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls.Data.PropertyGrid;
using AddGenericConstraint;

namespace wUAL.UserControls
{
    using Torrent.Extensions;
    using Torrent.Helpers.Utils;
    using static Torrent.Helpers.Utils.DebugUtils;

    public partial class MyEnumEditor : UserControl
    {
        Type _type;
        public MyEnumEditor() : base()
        {
            InitializeComponent();
            SetupComponent();
        }
        public Type Type {
            get { return _type; }
            set
            {
                _type = value;
            }
        }


        #region Enum Dependency Property

        /// <summary>
        /// Gets or sets the `Enum` Dependency Property
        /// </summary>
        public object Enum
        {
            get { return (object)GetValue(EnumProperty); }
            set {
                SetValue(EnumProperty, value);
                SetEnumValue();
            }
        }

        /// <summary>
        /// Identifies the `Enum` Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnumProperty =
            DependencyProperty.Register(nameof(Enum), typeof(object),
              typeof(MyEnumEditor), new PropertyMetadata(null));

        #endregion
        public void SetupComponent()
        {
            SetEnumValue(false);
        }
        public void SetEnumValue(bool requireValue=true)
        {
            if (Enum == null)
            {
                if (requireValue)
                {
                    throw new ArgumentNullException(nameof(Enum));
                }
                return;
            }
            var enumType = Enum.GetType();
            enumType = Nullable.GetUnderlyingType(enumType) ?? enumType;
            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"Specified Type {enumType.GetFriendlyFullName()} is not an Enum.");
            }
            Type = enumType;
            var values = System.Enum.GetValues(enumType);
            DEBUG.Break();
        }
    }
}
