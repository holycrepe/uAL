using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using PostSharp.Patterns.Model;
using Torrent.Infrastructure;

namespace wUAL.UserControls
{
    /// <summary>
    /// Interaction logic for QueueToggleSettingControl.xaml
    /// </summary>
    [NotifyPropertyChanged]
    public partial class QueueToggleSettingControl : UserControl, INotifyPropertyChanged
    {
        public QueueToggleSettingControl() { InitializeComponent(); }
        public QueueToggleSettingControl(bool enableMetalinks) : this() {
            EnableMetalinks = enableMetalinks;
        }
        #region Dependency Properties
        #region Dependency Properties: Set Value
        protected void SetValueDp(DependencyProperty property, object value, [CallerMemberName] string propertyName = null)
        {
            this.SetValue(property, value);
            OnPropertiesChanged(propertyName, property.Name == propertyName ? string.Empty : property.Name);
        }
        #endregion
        #region Dependency Properties: Enum
        #region Dependency Properties: Enum: Accessors and Definition
        /// <summary>
        /// Gets or sets the `Enum` Dependency Property
        /// </summary>
        [SafeForDependencyAnalysis]
        public object Enum
        //{ get; set; }
        {
            get { return (object)this.GetValue(EnumProperty); }
            set { this.SetValueDp(EnumProperty, value); }
        }

        /// <summary>
        /// Identifies the `Enum` Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnumProperty =
            DependencyProperty.Register(nameof(Enum), typeof(object),
              typeof(QueueToggleSettingControl),
              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnEnumChanged, OnEnumCoerced));
        #endregion
        #region Dependency Properties: Enum: Events
        protected static void OnEnumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as QueueToggleSettingControl;
            var value = e.NewValue;
            // myFlagEnumEditor.OnDependencyPropertyChanged(e);
            //if (control == null ||
            //    control?.Value?.ToString() == value?.ToString())
            //    return;
            ////control.Enum = value;
            //control.ViewModel.SetEnum(value);
        }
        protected static object OnEnumCoerced(DependencyObject d, object baseValue)
        {
            return baseValue;
        }
        #endregion
        #endregion
        #region EnableMetalinks Dependency Property

        /// <summary>
        /// Gets or sets the `EnableMetalinks` Dependency Property
        /// </summary>
        [SafeForDependencyAnalysis]
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
        #endregion


        #region Events
        #region Events: Property Changed
        protected virtual void OnPropertyChangedLocal(string propertyName)
        {

        }
        #endregion
        #endregion
        #region Interfaces
        #region Interfaces: IPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnDependencyPropertyChanged(DependencyPropertyChangedEventArgs e)
            => OnPropertiesChanged(e.Property.Name);
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => OnPropertiesChanged(propertyName);
        public virtual void OnPropertiesChanged(params string[] propertyNames)
            => NotifyPropertyChangedBase.DoOnPropertyChanged(this, PropertyChanged, OnPropertyChangedLocal, propertyNames);
        #endregion

        #endregion

    }
}
