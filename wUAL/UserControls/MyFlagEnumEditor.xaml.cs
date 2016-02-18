#define LOG_PROPERTY_CHANGED

using System;
using System.Windows;
using Telerik.Windows.Controls.Data.PropertyGrid;

namespace wUAL.UserControls
{
    using PostSharp.Patterns.Model;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows.Controls;
    using Telerik.Windows.Controls;
    using Torrent.Converters;
    using Torrent.Enums;
    using Torrent.Extensions;
    using Torrent.Helpers.Utils;
    using Torrent.Infrastructure;
    using Torrent.Infrastructure.Enums;
    using static Torrent.Helpers.Utils.DebugUtils;
    [NotifyPropertyChanged]
    public partial class MyFlagEnumEditor : UserControl, INotifyPropertyChanged
    {
        bool _useCombinedFormat = true;
        
        public MyFlagEnumEditor() : base()
        {
            this.PropertyChanged += OnPropertyChanged;
            InitializeComponent();
            //this.DataContext = ViewModel;
            (this.Content as FrameworkElement).DataContext = this.ViewModel;
        }
        #region Public Properties
        #region Public Properties: Accessors
        public EnumMemberViewModel ViewModel { get; set; }
            = new EnumMemberViewModel();
        public object Test { get; set; }
        public bool UseCombinedFormat {
            get { return this._useCombinedFormat; }
            set { this.ViewModel.UseCombinedFormat = this._useCombinedFormat = value; }
        }
        #endregion
        #endregion

        #region Dependency Properties
        #region Dependency Properties: Set Value
        protected void SetValueDp(DependencyProperty property, object value, [CallerMemberName] string propertyName=null)
        {
            this.SetValue(property, value);
            OnPropertyChanged(propertyName, property.Name == propertyName ? string.Empty : property.Name);
        }
        #endregion
        #region Dependency Properties: Enum
        #region Dependency Properties: Enum: Accessors and Definition
        /// <summary>
        /// Gets or sets the `Enum` Dependency Property
        /// </summary>
        public object Enum
        {
            get { return (object)this.GetValue(EnumProperty); }
            set { this.SetValueDp(EnumProperty, value); DEBUG.Break(); }
        }
        
        /// <summary>
        /// Identifies the `Enum` Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnumProperty =
            DependencyProperty.Register(nameof(Enum), typeof(object),
              typeof(MyFlagEnumEditor), 
              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnEnumChanged, OnEnumCoerced));
        #endregion
        #region Dependency Properties: Enum: Events
        protected static void OnEnumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
            var myFlagEnumEditor = d as MyFlagEnumEditor;
            var value = e.NewValue;
            // myFlagEnumEditor.OnDependencyPropertyChanged(e);
            myFlagEnumEditor.ViewModel.SetEnum(value);
        }
        protected static object OnEnumCoerced(DependencyObject d, object baseValue)
        {
            return baseValue;
        }
        #endregion
        #endregion
        #endregion

        #region Events
        #region Events: Property Changed
        protected virtual void OnPropertyChangedLocal(string propertyName)
        {
            ViewModel.LogPropertyChanged(propertyName);
            if (IsInitialized)
            {
                DEBUG.Noop();
            }
        }
        #endregion
        #region Components
        #region Components: RadComboBoxFlagEnumGrid
        private void RadComboBoxFlagEnumGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as RadComboBox).IsDropDownOpen = false;
        }
        #endregion
        #endregion
        #endregion
        #region Interfaces
        #region Interfaces: IPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnDependencyPropertyChanged(DependencyPropertyChangedEventArgs e)
            => OnPropertyChanged(e.Property.Name);
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            => OnPropertyChanged(e.PropertyName);
        public virtual void OnPropertyChanged(string propertyNames)
            => OnPropertyChanged(propertyNames.Split(';'));
        public virtual void OnPropertyChanged(params string[] propertyNames)
            => NotifyPropertyChangedBase.DoOnPropertyChanged(this, PropertyChanged, OnPropertyChangedLocal, propertyNames);
        #endregion
        #endregion
    }
}