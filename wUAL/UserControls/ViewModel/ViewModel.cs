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
using System.Collections.Generic;

namespace wUAL.UserControls
{

    [NotifyPropertyChanged]
    public class ViewModel<T, TValue> : ViewModelBase
        where T : class 
        where TValue : class
    {
        protected T _selectedItem;
        protected ObservableCollection<T> _selectedItems;
        protected TValue _value;
        public ObservableCollection<T> Items { get; set; }
            = new ObservableCollection<T>();
        public ObservableCollection<T> SelectedItems
            => _selectedItems ?? (_selectedItems = new ObservableCollection<T>());
        public ViewModel()
        {
            
        }
        #region Value / Value Selector
        protected virtual TValue GetValueFromItem(T item)
            => item as TValue;
        public virtual TValue Value
        {
            get { return this._value; }
            set
            {
                this._value = value;
                this.SetSelection();
            }
        }
        #endregion
        #region Values
        protected virtual IEnumerable<TValue> GetValuesFromItems(IEnumerable<T> items)
            => items.Select(item => this.GetValueFromItem(item));
        protected virtual TValue GetConsolidatedValue(IEnumerable<TValue> values)
            => values.FirstOrDefault();
        protected TValue GetConsolidatedValue(IEnumerable<T> items)
            => this.GetConsolidatedValue(this.GetValuesFromItems(items));
        protected TValue GetConsolidatedValue()
            => this.GetConsolidatedValue(this.SelectedItems);
        protected virtual object GetDisplayValue(IEnumerable<TValue> values)
            => string.Join(", ", values.Select(item => item.ToString()));
        #endregion
        #region Selected Item
        public virtual T SelectedItem
        {
            get { return this._selectedItem; }
            set
            {
                if (value != null)
                {
                    this.Value = this.GetValueFromItem(value);
                    this._selectedItem = value;
                }
            }
        }


        //public virtual TValue Value { get; set; }

        //private T selectedItem;
        //public virtual T SelectedItem { get; set; }
        //public T SelectedItem
        //{
        //    get
        //    {
        //        return this.selectedItem;
        //    }

        //    set
        //    {
        //        if (this.selectedItem != value)
        //        {
        //            this.selectedItem = value;
        //            //this.OnPropertyChanged();
        //        }
        //    }
        //}
        #endregion
        #region Properties: Methods: Set Property
        //protected bool SetProperty<TProperty>(ref TProperty backingField, TProperty newValue, [CallerMemberName] string propertyName = null)
        //{
        //    if ((backingField == null && newValue == null) ||
        //        (backingField != null && backingField.Equals(newValue)))
        //    {
        //        return false;
        //    }
        //    backingField = newValue;
        //    this.OnPropertyChanged(propertyName);
        //    return true;
        //}
        //protected void SetProperty<TProperty>(ref TProperty backingField, TProperty newValue, Expression<Func<T>> propertyExpression)
        //    => SetProperty(ref backingField, newValue, propertyExpression.GetPropertyName());
        protected bool IsMatchingItem(T item, TValue match)
            => this.IsMatchingItem(this.GetValueFromItem(item), match);
        protected virtual bool IsMatchingItem(TValue item, TValue match)
            => item.Equals(match);
        protected IEnumerable<T> GetSelectedItems(TValue value)
            => this.Items.Where(item => this.IsMatchingItem(item, value));
        protected void SetSelectedItem()
            => this.SetSelectedItem(this.Value);
        protected virtual void SetSelectedItem(TValue value)
        {
            var selectedItem = this.Items.FirstOrDefault(item => this.GetValueFromItem(item).Equals(value));
            if (selectedItem != null)
                this._selectedItem = selectedItem;
        }

        protected void SetSelection()
            => SetSelection(this.Value);
        protected virtual void SetSelection(TValue value)
            => SetSelectedItem(value);
        #endregion
        #region Events
        #region Events: Property Changed
        protected virtual void OnPropertyChangedLocal(string propertyName)
            => LogPropertyChanged(propertyName);
        #endregion
        #endregion
        #region Interfaces
        #region Interfaces: IPropertyChanged
        //public event PropertyChangedEventHandler PropertyChanged;
        //protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        //    => OnPropertyChanged(e.PropertyName);
        //protected virtual void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
        //    => this.OnPropertyChanged(propertyExpression.GetPropertyName());
        //protected override void OnPropertyChanged(string propertyNames)
        //=> OnPropertyChanged(propertyNames.Split(';'));
        protected new virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => OnPropertiesChanged(propertyName);
        public virtual void OnPropertiesChanged(params string[] propertyNames)
            => NotifyPropertyChangedBase.DoOnPropertyChanged(this,
                (s, e) => base.OnPropertyChanged(e.PropertyName), 
                OnPropertyChangedLocal, propertyNames);
        #endregion
        #endregion
        #region Logging
        [System.Diagnostics.Conditional("LOG_PROPERTY_CHANGED"), System.Diagnostics.Conditional("LOG_ALL")]
        public void LogPropertyChanged(string propertyName)
            => Log("Δ ", propertyName);
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        public virtual void Log(string prefix = "+", object status=null, object title = null, object text=null, object info=null,  PadDirection textPadDirection = PadDirection.Default,
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