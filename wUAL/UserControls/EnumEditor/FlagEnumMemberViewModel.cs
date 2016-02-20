using PostSharp.Patterns.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Telerik.Windows.Controls;
using Torrent.Enums;
using Torrent.Extensions;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;
using Torrent.Infrastructure.Enums;
using Torrent.Properties.Settings;
using uAL.Properties.Settings.ToggleSettings;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace wUAL.UserControls
{
    public class FlagEnumMemberViewModel : EnumMemberViewModel
    {

        #region Overrides of ViewModel
        protected override bool IS_MULTIPLE_DEFAULT { get; }
            = true;
        protected override bool IsMatchingItem(object item, object match)
        {
            var value = (long)item;
            return (((long)match) & value) == value;
        }
        protected override object GetConsolidatedValue(IEnumerable<object> values)
        {
            long value = 0;
            foreach (var flag in values)
            {
                value = value | ((long) flag);
            }
            return value;
        }
        protected override void OnSelectedItemsChangedComplete(NotifyCollectionChangedEventArgs e)
        {
            if (!SelectedItems.Contains(SelectedItem))
            {
                SelectedItem = SelectedItems.FirstOrDefault();
            }
            Value = GetConsolidatedValue();
        }

        #endregion
        public FlagEnumMemberViewModel() 
        {
            if (MainApp.DesignMode)
            {
                this.SetEnum(MonitorTypes.Main);
            }
            else
            {
                // this.SetEnum(ProcessQueueMethod.Parallel);
            }
        }
    }
}