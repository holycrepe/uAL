using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interactivity;
using Telerik.Windows.Controls;
using System.Collections.Specialized;
using System.Windows;
using System.Collections;
using Torrent.Infrastructure.Enums;
using wUAL.UserControls;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Torrent.Infrastructure.ContextHandlers;
using uAL.Infrastructure.UI;

namespace wUAL.WPF
{
    using static Torrent.Helpers.Utils.DebugUtils;
    public class GridMultiSelectBehavior<T, TValue, TViewModel> : Behavior<RadGridView>
        where T : class
        where TValue : class
        where TViewModel : ViewModel<T, TValue>
    {
        private RadComboBox _comboBox = null;
        protected RadGridView Grid
            => AssociatedObject as RadGridView;
        protected RadComboBox ComboBox
            => _comboBox ?? Grid.GetVisualParent<RadComboBox>();
        protected TViewModel ViewModel
            => Grid.DataContext as TViewModel;
        public T[] GridSelectedItems
            => Grid.SelectedItems.Cast<T>().ToArray();
        #region Dependency Property: SelectedItems
        public ObservableCollection<T> Items
            => ViewModel.Items;
        public ObservableCollection<T> SelectedItems
            => ViewModel.SelectedItems;
        public T SelectedItem
            => ViewModel.SelectedItem;
        public TValue Value
            => ViewModel.Value;
        public ContextHandlers IgnoreEvents { get; }
            = new ContextHandlers();
        //public INotifyCollectionChanged SelectedItems
        //{
        //    get { return (INotifyCollectionChanged)GetValue(SelectedItemsProperty); }
        //    set { SetValue(SelectedItemsProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for SelectedItemsProperty.  This enables animation, styling, binding, etc...
        //public static readonly DependencyProperty SelectedItemsProperty =
        //    DependencyProperty.Register(nameof(SelectedItems), typeof(INotifyCollectionChanged), typeof(GridMultiSelectBehavior<T, TValue, TViewModel>), new PropertyMetadata(OnSelectedItemsPropertyChanged));


        //private static void OnSelectedItemsPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        //{
        //    var collection = args.NewValue as INotifyCollectionChanged;
        //    if (collection != null)
        //    {
        //        collection.CollectionChanged += ((GridMultiSelectBehavior<T, TValue, TViewModel>)target).ContextSelectedItems_CollectionChanged;
        //    }
        //}
        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            Grid.SelectionChanging += Grid_SelectionChanging;
            Grid.SelectionChanged += Grid_OnSelectionChanged;
            SelectedItems.CollectionChanged += ContextSelectedItems_CollectionChanged;
            Grid.MouseUp += Grid_OnMouseUp;
            //Grid.SelectedItems.CollectionChanged += GridSelectedItems_CollectionChanged;
        }

        private void Grid_OnSelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            if (!ViewModel.IgnoreSelectedItemsChanged && !IgnoreEvents)
                using (IgnoreEvents.On)
                using (ViewModel.IgnoreSelectedItemsChanged.On)
                    OnGridSelectionChanged(e);
        }

        private void Grid_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Middle)
                Debugger.Break();
        }

        protected virtual bool OnGridSelectionChanging(SelectionChangingEventArgs e)
            => false;
            //=> Transfer(Grid.SelectedItems as IList, SelectedItems as IList);
        protected virtual bool OnGridSelectionChanged(SelectionChangeEventArgs e)
            => Transfer(Grid.SelectedItems as IList, SelectedItems as IList);
        protected virtual void OnContextCollectionChanged(NotifyCollectionChangedEventArgs e)
            => Transfer(SelectedItems as IList, Grid.SelectedItems as IList);
        private void Grid_SelectionChanging(object sender, SelectionChangingEventArgs e)
        {
            if (!ViewModel.IgnoreSelectedItemsChanged && !IgnoreEvents)
                using (IgnoreEvents.On)
                using (ViewModel.IgnoreSelectedItemsChanged.On)
                    OnGridSelectionChanging(e);
        }

        void ContextSelectedItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!IgnoreEvents)
                using (IgnoreEvents.On)
                using (ViewModel.IgnoreSelectedItemsChanged.On)
                    OnContextCollectionChanged(e);
        }

        protected static bool Transfer(IList source, IList target)
        {
            if (source == null || target == null)
                return false;

            var itemsToRemove = target.Cast<object>().Where(item => !source.Contains(item));
            var itemsToAdd = source.Cast<object>().Where(item => !target.Contains(item));

            return Modify(target, itemsToAdd, itemsToRemove);
        }

        protected static bool Modify(IList target, IEnumerable<object> itemsToAdd, IEnumerable<object> itemsToRemove)
        {
            foreach (var item in itemsToRemove.ToArray().Where(target.Contains))
                target.Remove(item);

            foreach (var item in itemsToAdd.ToArray().Where(item => !target.Contains(item)))
                target.Add(item);

            return false;
        }
    }
}
