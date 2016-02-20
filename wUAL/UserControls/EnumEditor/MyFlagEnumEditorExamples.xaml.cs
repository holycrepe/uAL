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
    public partial class MyFlagEnumEditorExamples : UserControl, INotifyPropertyChanged
    {
        bool _useCombinedFormat = true;

        public MyFlagEnumEditorExamples() : base()
        {
            this.PropertyChanged += MyFlagEnumEditor_PropertyChanged;
            InitializeComponent();
            ViewModel.PropertyChanged += ViewModelOnPropertyChanged;
            //this.DataContext = ViewModel;
            //(this.Content as FrameworkElement).DataContext = this.ViewModel;
        }

        private void ViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.Value))
            {
                Enum = ViewModel.Value;
            }
        }

        #region Public Properties
        #region Public Properties: Accessors
        public FlagEnumMemberViewModel ViewModel { get; set; }
            = new FlagEnumMemberViewModel();
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
        {
            get
            {
                if (Depends.Guard)
                {
                    Depends.On(this.ViewModel, this.ViewModel.Value, this.ViewModel.SelectedItem);
                }
                return (object)this.GetValue(EnumProperty);
            }
            set { this.SetValueDp(EnumProperty, value); }
        }
        
        /// <summary>
        /// Identifies the `Enum` Dependency Property
        /// </summary>
        public static readonly DependencyProperty EnumProperty =
            DependencyProperty.Register(nameof(Enum), typeof(object),
              typeof(MyFlagEnumEditorExamples), 
              new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnEnumChanged, OnEnumCoerced));
        #endregion
        #region Dependency Properties: Enum: Events
        protected static void OnEnumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
            var control = d as MyFlagEnumEditorExamples;
            var value = e.NewValue;
            // myFlagEnumEditor.OnDependencyPropertyChanged(e);
            if (control == null || control.Enum == control.ViewModel.Value)
                return;

            control.ViewModel.SetEnum(value);
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
        protected void RadComboBoxFlagEnumGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var control = sender as RadComboBox;
            if (control != null)
            {
                control.IsDropDownOpen = false;
            }
        }
        #endregion
        #endregion
        #endregion
        #region Interfaces
        #region Interfaces: IPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnDependencyPropertyChanged(DependencyPropertyChangedEventArgs e)
            => OnPropertiesChanged(e.Property.Name);
        private void MyFlagEnumEditor_PropertyChanged(object sender, PropertyChangedEventArgs e)
            => DEBUG.Noop();
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => OnPropertiesChanged(propertyName);
        public virtual void OnPropertiesChanged(params string[] propertyNames)
            => NotifyPropertyChangedBase.DoOnPropertyChanged(this, PropertyChanged, OnPropertyChangedLocal, propertyNames);
        #endregion
        #endregion
    }
}