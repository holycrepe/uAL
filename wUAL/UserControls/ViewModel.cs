#define LOG_PROPERTY_CHANGED

using PostSharp.Patterns.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Telerik.Windows.Controls;
using Torrent.Enums;
using Torrent.Extensions;
using Torrent.Extensions.Expressions;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;
using Torrent.Infrastructure.Enums;

namespace wUAL.UserControls
{
    [NotifyPropertyChanged]
    public class ViewModel<T, TValue> : ViewModelBase 
        where T : class 
        where TValue : class
    {
        public ObservableCollection<T> Items { get; set; }
            = new ObservableCollection<T>();
        public ViewModel()
        {
            
        }

        public TValue Value { get; set; }
        #region Selected Item
        private T selectedItem;

        public T SelectedItem
        {
            get
            {
                return this.selectedItem;
            }

            set
            {
                if (this.selectedItem != value)
                {
                    this.selectedItem = value;
                    this.OnPropertyChanged(() => this.SelectedItem);
                }
            }
        }
        #endregion
        #region Properties: Methods: Set Property
        protected bool SetProperty<TProperty>(ref TProperty backingField, TProperty newValue, [CallerMemberName] string propertyName = null)
        {
            if ((backingField == null && newValue == null) ||
                (backingField != null && backingField.Equals(newValue)))
            {
                return false;
            }
            backingField = newValue;
            this.OnPropertyChanged(propertyName);
            return true;
        }
        protected void SetProperty<TProperty>(ref TProperty backingField, TProperty newValue, Expression<Func<T>> propertyExpression)
            => SetProperty(ref backingField, newValue, propertyExpression.GetPropertyName());
        #endregion
        #region Events
        #region Events: Property Changed
        protected virtual void OnPropertyChangedLocal(string propertyName)
        {
            LogPropertyChanged(propertyName);
        }
        #endregion
        #endregion
        #region Interfaces
        #region Interfaces: IPropertyChanged
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
            => OnPropertyChanged(e.PropertyName);
        protected override void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
            => this.OnPropertyChanged(propertyExpression.GetPropertyName());
        //protected override void OnPropertyChanged(string propertyNames)
        //=> OnPropertyChanged(propertyNames.Split(';'));
        protected new void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => OnPropertiesChanged(propertyName);
        public virtual void OnPropertiesChanged(params string[] propertyNames)
            => NotifyPropertyChangedBase.DoOnPropertyChanged(this,
                (s, e) => base.OnPropertyChanged(e.PropertyName), OnPropertyChangedLocal, propertyNames);
        #endregion
        #endregion
        #region Logging
        [System.Diagnostics.Conditional("LOG_PROPERTY_CHANGED"), System.Diagnostics.Conditional("LOG_ALL")]
        public void LogPropertyChanged(string propertyName)
            => Log("Δ ", propertyName);
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        public void Log(string prefix = "+", object status=null, object title = null, object text=null, object info=null,  PadDirection textPadDirection = PadDirection.Default,
                        string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default,
                        string titleSuffix = null, int random = 0)
        {
#if DEBUG || TRACE_EXT
            LogUtils.Log(
                        (prefix ?? " ") + status,
                        title?.ToString(), text?.ToString(), info?.ToString(), textPadDirection, textSuffix,
                        titlePadDirection, titleSuffix, random);
#endif
        }

        #endregion
    }
}