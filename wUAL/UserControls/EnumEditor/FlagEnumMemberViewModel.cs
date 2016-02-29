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
using System.Diagnostics;

namespace wUAL.UserControls
{
    using Puchalapalli.Extensions.Collections;
    using static Torrent.Helpers.Utils.DebugUtils;
    [NotifyPropertyChanged]
    public class FlagEnumMemberViewModel : EnumMemberViewModel
    {

        #region Overrides of ViewModel
        protected override bool IS_MULTIPLE_DEFAULT { get; }
            = true;
        protected override bool IsMatchingItem(object item, object match)
        {
            var value = Convert.ToInt64(item);
            var flag = Convert.ToInt64(match);
            if (value == 0 || value == -1)
                return value == flag;
            return (flag & value) == value;
        }
        [DebuggerNonUserCode]
        protected override object GetConsolidatedValue(IEnumerable<object> values)
            => EnumUtils.GetUsedBits(Type, values);
        
        protected override IEnumerable<EnumMember> GetSelectedItems(object value)
        {
            var items = base.GetSelectedItems(value).ToArray();
            if (!items.Any() && Value == Disabled)
                return new[] { Disabled };
            return items;
        }
        protected override void InitializeEnumItems()
        {
            base.InitializeEnumItems();
            All = Items.FirstOrDefault(x => x.IsAll);
            None = Items.FirstOrDefault(x => x.IsDisabled);
            Disabled = None ?? Members.FirstOrDefault(value => value == 0);
            var usedBits = EnumUtils.GetUsedBits(Type, Items, true);
            Main = new EnumMember(Type, usedBits, DisplayFormat);
            SelectedItems.AddRange(GetSelectedItems());
        }
        protected override void SetDisplayFormat()
        {
            base.SetDisplayFormat();
            if (Disabled)
                Disabled.DisplayFormat = DisplayFormat;
        }
        protected override EnumMember GetItemFromValue(object value)
            //=> base.GetItemFromValue(value)
            //?? Disabled == value
            //    ? Disabled
            //    : null;
        {
            var item = base.GetItemFromValue(value);
            if (item)
                return item;
            return Disabled == value 
                ? Disabled 
                : null;
        }
        protected override EnumMember GetDefaultSelectedItem()
            => base.GetDefaultSelectedItem() 
            ?? Value == Disabled ? Disabled : null;
        #endregion
        #region Public Properties
        public EnumMember All { get; private set; }
        public EnumMember None { get; private set; }
        public EnumMember Disabled { get; private set; }
        public EnumMember Main { get; private set; }
        #endregion
        public FlagEnumMemberViewModel()  : base(false)
        {
            if (MainApp.DesignMode)
            {
                // DEBUG.Break();
                this.SetEnum(MonitorTypes.Main);
            }
            else
            {
                // this.SetEnum(ProcessQueueMethod.Parallel);
            }
        }
    }
}