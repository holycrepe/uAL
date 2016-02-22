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
using Torrent.Extensions;
using Torrent.Infrastructure.ContextHandlers;
using wUAL.WPF;

namespace wUAL.WPF
{
    public class GridMultiSelectBehaviorFlagEnumMember
        : GridMultiSelectBehavior<EnumMember, object, FlagEnumMemberViewModel>
    {
        protected EnumMember All
            => ViewModel.All;
        protected EnumMember None
            => ViewModel.None;
        protected EnumMember Disabled
            => ViewModel.Disabled;

        protected bool HasAllMainMembers
            => GridSelectedItems.GetPositiveBitwiseOr() == ViewModel.Main;
        protected bool OverrideGridSelection(SelectionChangingEventArgs e, EnumMember item)
        {
            if (item == null)
                return false;
            if (e != null)
                e.Cancel = true;
            if (Grid.SelectedItems.Count > 0)
                Grid.SelectedItems.Clear();
            //if (SelectedItems.Count > 0)
            //    SelectedItems.Clear();
            if (item.IsAll)
                Grid.SelectedItems.AddRange(Items.Where(x => !x.IsDisabled));
            else
                Grid.SelectedItems.Add(item);
            return true;
        }

        protected bool HandleGridSelectionEvent(ReadOnlyCollection<object> added, ReadOnlyCollection<object> removed, SelectionChangingEventArgs e = null)
        {
            var isDropdownOpen = ComboBox?.IsDropDownOpen ?? false;
            var addedItems = added.Cast<EnumMember>().ToArray();
            var removedItems = removed.Cast<EnumMember>().ToArray();            
            if (e != null)
                return (e.Cancel = (HasAllMainMembers && removedItems.HasAllMember()));

            if (HasAllMainMembers && removedItems.HasAllMember())
                Grid.SelectedItems.Clear();    

            else if (All && !GridSelectedItems.HasAllMember() && HasAllMainMembers)            
                Grid.SelectedItems.Add(All);
            
            else if (added.Count > 0)
            {
                if (!OverrideGridSelection(e, addedItems.GetAllMember()) &&
                    !OverrideGridSelection(e, addedItems.GetDisabledMember()) &&
                    addedItems.HasEnabledMember())
                {
                    Grid.SelectedItems.Remove(None);
                }
            }
            else if (removed.Count > 0)
            {
                Grid.SelectedItems.Remove(All);
                if (Grid.SelectedItems.Count == 0 && None)
                {
                    Grid.SelectedItems.Add(None);
                }
            }

            Transfer(Grid.SelectedItems, SelectedItems);
            ViewModel.SetSelectedItem(added.FirstOrDefault()
                ?? Grid.SelectedItems.FirstOrDefault(item =>
                !removed.Contains(item)) 
                ?? Disabled);
            Transfer(SelectedItems, Grid.SelectedItems);
            if (ComboBox != null)
                ComboBox.IsDropDownOpen = isDropdownOpen;
            return false;
        }

        //protected override bool OnGridSelectionChanging(SelectionChangingEventArgs e)
        //    => HandleGridSelectionEvent(e.AddedItems, e.RemovedItems, e);

        protected override bool OnGridSelectionChanged(SelectionChangeEventArgs e)
            => HandleGridSelectionEvent(e.AddedItems, e.RemovedItems);
    }


    public class GridMultiSelectBehaviorEnumMember
        : GridMultiSelectBehavior<EnumMember, object, EnumMemberViewModel>
    { }
}
