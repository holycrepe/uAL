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
    public class ViewModelMultiSelect<T, TValue> : ViewModel<T, TValue>
        where T : class 
        where TValue : class
    {
        public ObservableCollection<T> SelectedItems { get; set; }
            = new ObservableCollection<T>();
        public ViewModelMultiSelect()
        {
            
        }
        #region Value / Value Selector
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
        [SafeForDependencyAnalysis]
        public override T SelectedItem
        {
            get { return this._selectedItem; }
            set
            {
                if (value != null)
                {
                    this.SetSelectedItem(value);
                    this._selectedItem = value;
                    this.Value = this.GetConsolidatedValue();
                }
            }
        }
        #endregion
        #region Properties: Methods: Set Property        
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
        protected override void SetSelection(TValue value)
            => this.SetSelectedItems(value);
        #endregion
    }
}