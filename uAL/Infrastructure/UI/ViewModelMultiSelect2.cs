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
    public class ViewModelMultiSelect2<T, TValue> : ViewModelBase
        where T : class 
        where TValue : class
    {
        private T _selectedItem;
        private TValue _value;
        public ObservableCollection<T> Items { get; set; }
            = new ObservableCollection<T>();
        public ObservableCollection<T> SelectedItems { get; set; }
            = new ObservableCollection<T>();
        public ViewModelMultiSelect2()
        {
            
        }
        #region Value / Value Selector
        protected virtual TValue GetValueFromItem(T item)
            => item as TValue;
        protected virtual IEnumerable<TValue> GetValueFromSelection(IEnumerable<T> items)
            => items.Select(item => this.GetValueFromItem(item));
        protected virtual object GetDisplayValue(IEnumerable<TValue> values)
            => string.Join(", ", values.Select(item => item.ToString()));
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
        #region Selected Items
        protected virtual T GetSelectedItem(ObservableCollection<T> selectedItems)
            => selectedItems.FirstOrDefault();
        protected virtual void SetSelectedItem(T item)
        {
            if (!this.SelectedItems.Contains(item))
            {
                this.SelectedItems.Add(item);
            }
        }
        public virtual T SelectedItem
        {
            get { return this.GetSelectedItem(this.SelectedItems); }
            set
            {
                if (value != null)
                {
                    this.SetSelectedItem(value);
                    this.Value = this.GetValueFromItem(this.GetSelectedItem(this.SelectedItems));
                }
            }
        }
        #endregion
        #region Properties: Methods: Set Property
        protected bool IsMatchingItem(T item, TValue match)
            => this.IsMatchingItem(this.GetValueFromItem(item), match);
        protected virtual bool IsMatchingItem(TValue item, TValue match)
            => item.Equals(match);

        protected void SetSelection()
            => this.SetSelection(this.Value);
        protected void SetSelection(TValue value)
        {
            foreach (var item in this.Items)
            {
                if (this.IsMatchingItem(item, value))
                {
                    if (!this.SelectedItems.Contains(item))
                    {
                        this.SelectedItems.Add(item);
                    }
                }
                else if (this.SelectedItems.Contains(item))
                {
                    this.SelectedItems.Remove(item);
                }
            }
        }
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