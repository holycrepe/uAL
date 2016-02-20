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
using Torrent.Exceptions;
using System.Collections.Specialized;

namespace wUAL.UserControls
{

    [NotifyPropertyChanged]
    public class ViewModel<T, TValue> : ViewModelBase
        where T : class
        where TValue : class
    {
        protected bool IsMultiple { get; }
        protected virtual bool IS_MULTIPLE_DEFAULT { get; }
            = false;
        protected T _selectedItem;
        protected ObservableCollection<T> _selectedItems;
        protected TValue _value;
        public ObservableCollection<T> Items { get; set; }
            = new ObservableCollection<T>();
        public ObservableCollection<T> SelectedItems
        {
            get
            {
                if (!this.IsMultiple)
                {
                    throw new ViewModelStateException($"Cannot access {nameof(SelectedItems)} when {nameof(IsMultiple)} is false.");
                }
                return _selectedItems ?? (_selectedItems = new ObservableCollection<T>());
            }
        }
        public ViewModel()
        {
            this.IsMultiple = IS_MULTIPLE_DEFAULT;
            Initialize();
        }
        public ViewModel(bool isMultiple)
        {
            this.IsMultiple = isMultiple;
            Initialize();
        }
        protected virtual void Initialize()
        {
            if (IsMultiple)
            {
                this.SelectedItems.CollectionChanged += SelectedItems_OnCollectionChanged;
            }
        }
        protected virtual void OnSelectedItemsChanged(NotifyCollectionChangedEventArgs e) { }
        protected virtual void OnSelectedItemsChangedComplete(NotifyCollectionChangedEventArgs e) { }

        protected virtual void OnSelectedItemRemoved(T item) { }
        protected virtual void OnSelectedItemAdded(T item) { }
        private void SelectedItems_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnSelectedItemsChanged(e);
            foreach (var item in e.OldItems)
            {
                OnSelectedItemRemoved(item as T);
            }
            foreach (var item in e.NewItems)
            {
                OnSelectedItemAdded(item as T);
            }
            OnSelectedItemsChangedComplete(e);
        }
        #region Value
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
        #region Selected Item
        [SafeForDependencyAnalysis]
        public virtual T SelectedItem
        {
            get { return this._selectedItem; }
            set
            {
                if (value != null && value != this._selectedItem)
                {
                    this._selectedItem = value;
                    if (this.IsMultiple)
                    {
                        this.AddSelectedItem(value);
                        this.Value = this.GetConsolidatedValue();
                    }
                    else
                    {
                        this.Value = this.GetValueFromItem(value);
                    }
                }
            }
        }
        #endregion
        #region Properties: Methods
        #region Properties: Methods: Is Matching Item
        protected bool IsMatchingItem(T item, TValue match)
            => this.IsMatchingItem(this.GetValueFromItem(item), match);
        protected virtual bool IsMatchingItem(TValue item, TValue match)
            => item.Equals(match);
        #endregion
        #region Properties: Methods: Value
        protected virtual TValue GetValueFromItem(T item)
            => item as TValue;
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
        #region Properties: Methods: Selection
        protected void SetSelection()
            => SetSelection(this.Value);
        protected virtual void SetSelection(TValue value)
            => SetSelectedItem(value);
        #endregion
        #region Properties: Methods: Selected Item
        protected void SetSelectedItem()
            => this.SetSelectedItem(this.Value);
        protected virtual void SetSelectedItem(TValue value)
        {
            var selectedItem = this.Items.FirstOrDefault(item
                    => this.IsMatchingItem(item, value));
            if (selectedItem != null)
                this._selectedItem = selectedItem;

            if (this.IsMultiple)
            {
                SetSelectedItems(value);
            }
        }
        protected void AddSelectedItem(T item)
        {
            if (!this.SelectedItems.Contains(item))
            {
                this.SelectedItems.Add(item);
            }
        }
        #endregion
        #region Properties: Methods: Selected Items
        protected IEnumerable<T> GetSelectedItems(TValue value)
            => this.Items.Where(item => this.IsMatchingItem(item, value));
        protected void SetSelectedItems()
            => SetSelectedItems(this.Value);
        protected virtual void SetSelectedItems(TValue value)
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
        #endregion
        #region Events
        #region Events: Property Changed
        protected virtual void OnPropertyChangedLocal(string propertyName)
            => LogPropertyChanged(propertyName);
        #endregion
        #endregion
        #region Interfaces
        #region Interfaces: IPropertyChanged
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