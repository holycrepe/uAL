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

namespace wUAL.UserControls
{
    public class FlagEnumMemberViewModel : ViewModelMultiSelect<EnumMember, object>
    {
        #region EnumMemberViewModel
        bool _useCombinedFormat = true;

        #region Overrides of ViewModel
        protected override object GetValueFromItem(EnumMember item)
            => item.Value;
        //protected override void SetSelection(object value)
        //    => this.SetSelectedItem(value);
        #endregion
        #region Public Properties
        #region Public Properties: Accessors
        public Type Type { get; set; }
        [SafeForDependencyAnalysis]
        public bool UseCombinedFormat
        {
            get { return this._useCombinedFormat; }
            set
            {
                if (this._useCombinedFormat != value)
                {
                    this._useCombinedFormat = value;
                    this.Items?.SetUseCombined(value);
                }
            }
        }
        #endregion
        #region Public Properties: Methods        
        #region Public Properties: Methods: Enum Type
        protected Type GetEnumType(bool required = true, bool requiresFlags = true)
        {

            var enumType = this.Value?.GetType();
            if (enumType == null)
            {
                return null;
            }
            enumType = Nullable.GetUnderlyingType(enumType) ?? enumType;
            var isFlags = !requiresFlags || EnumUtils.IsFlags(enumType);
            if (enumType.IsEnum && isFlags)
                return enumType;

            if (required)
            {
                throw new ArgumentException($"Specified Type {enumType.GetFriendlyFullName()} is not {(enumType.IsEnum ? "a Flags" : "an")} Enum.");
            }
            return null;
        }
        #endregion
        #region Public Properties: Methods: Enum
        public void SetEnum(object value = null, bool requireValue = true)
        {
            if (value != null)
            {
                Value = value;
            }
            if (Value == null)
            {
                if (requireValue)
                {
                    throw new ArgumentNullException(nameof(Value));
                }
                return;
            }
            if (Type == null)
            {
                Type = GetEnumType();
                Items.Clear();
                Items.AddRange(EnumMember.GetBrowsableEnumMembers(Type, UseCombinedFormat));
                SetSelection();
            }
        }
        #endregion
        #endregion
        #endregion
        #region Logging
        public override void Log(string prefix = "+", object status = null, object title = null, object text = null, object info = null, PadDirection textPadDirection = PadDirection.Default,
                        string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default,
                        string titleSuffix = null, int random = 0)
        {
#if DEBUG || TRACE_EXT
            base.Log(prefix, Type?.Name, text, Value, SelectedItem);
#endif
        }

        #endregion
        #endregion
        #region Overrides of ViewModel
        protected override bool IsMatchingItem(object item, object match)
        {
            var value = (long)item;
            return (((long)match) & value) == value;
        }
        protected override void SetSelection(object value)
           => this.SetSelectedItems(value);
        protected override object GetConsolidatedValue(IEnumerable<object> values)
        {
            long value = 0;
            foreach (var flag in values)
            {
                value = value | ((long) flag);
            }
            return value;
        }
        #endregion
        #region Public Properties
        #region Public Properties: Accessors

        #endregion
        #region Public Properties: Methods        
        #region Public Properties: Methods: Enum Type
        //protected Type GetEnumType(bool required = true)
        //    => base.GetEnumType(required, true);
        #endregion
        #endregion
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