using System.Windows;
using System.Windows.Controls;

namespace wUAL.UserControls
{
    /// <summary>
    /// Interaction logic for QueueToggleSettingControl.xaml
    /// </summary>
    public partial class QueueToggleSettingControlOld : UserControl
    {
        public QueueToggleSettingControlOld() { InitializeComponent(); }
        public QueueToggleSettingControlOld(bool enableMetalinks) : this() {
            EnableMetalinks = enableMetalinks;
        }

        #region EnableMetalinks Dependency Property

        /// <summary>
        /// Gets or sets the `EnableMetalinks` Dependency Property
        /// </summary>
        public bool EnableMetalinks
        {
            get { return (bool)GetValue(EnableMetalinksProperty); }
            set { SetValue(EnableMetalinksProperty, value); }
        }

        /// <summary>
        /// Identifies the `EnableMetalinks` Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnableMetalinksProperty =
            DependencyProperty.Register(nameof(EnableMetalinks), typeof(bool),
              typeof(QueueToggleSettingControl), new PropertyMetadata(true));

        #endregion        
    }
}
