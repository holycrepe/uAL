using PostSharp.Patterns.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Telerik.Windows.Controls;
using Torrent.Enums;
using Torrent.Extensions;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;
using Torrent.Infrastructure.Enums;
using Torrent.Infrastructure.Reflection;
using Torrent.Properties.Settings;
using uAL.Infrastructure.UI;

namespace wUAL.UserControls
{
    using Puchalapalli.Extensions.Collections;

    [NotifyPropertyChanged]
    public class EnumMemberViewModel : ViewModel<EnumMember, object>
    {
        EnumMemberDisplayFormat _displayFormat = EnumMemberDisplayFormat.Combined;
        #region Overrides of ViewModel
        [DebuggerNonUserCode]
        protected override object GetValueFromItem(EnumMember item)
            => item.Value;
        [DebuggerNonUserCode]
        protected override void SetSelection(object value)
            => this.SetSelectedItem(value);
        #endregion
        #region Public Properties
        #region Public Properties: Accessors
        public Type Type { get; set; }
        [SafeForDependencyAnalysis]
        public EnumMemberDisplayFormat DisplayFormat
        {
            get { return this._displayFormat; }
            set
            {
                if (this._displayFormat != value)
                {
                    this._displayFormat = value;
                    this.SetDisplayFormat();
                }
            }
        }
        #endregion
        #region Public Properties
        #region Public Properties: Accessors
        public EnumMember[] Members { get; set; }
        [SafeForDependencyAnalysis]
        public EnumMember Member
            => (this.Type == null || this.Value == null) ? null 
            : this.Members?.FirstOrDefault(m => m == this.Value)
               ?? new EnumMember(this.Type, this.Value, this.DisplayFormat);
        #endregion
        #region Public Properties: Methods        
        #region Public Properties: Methods: Enum Type
        protected virtual void SetDisplayFormat()        
            => this.Items?.SetDisplayFormat(DisplayFormat);
        
        protected Type GetEnumType(bool required = true, bool requiresFlags = false)
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
                throw new ArgumentException($"Specified Type {enumType.GetFriendlyFullName()} is not {(enumType.IsEnum ? "a Flags" : "an")} Enum.");
            
            return null;
        }
        #endregion
        #region Public Properties: Methods: Enum

        protected virtual void InitializeEnumItems()
        {
            Type = GetEnumType();
            Items.Clear();
            Members = EnumMember.GetEnumMembers(Type, DisplayFormat).ToArray();
            Items.AddRange(Members.Where(f => f.Browsable));
        }
        public void SetEnum(object value = null, bool requireValue = true)
        {
            if (value != null)
            {
                _value = value;
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
                InitializeEnumItems();
                SetSelection();
                SetSelectedItemOnly();
            }
            else
            {
                SetSelection();
            }
        }
        #endregion
        #endregion
        #endregion
        #endregion
        public EnumMemberViewModel(bool setDesignEnum) : base() { }
        public EnumMemberViewModel() : this(false)
        {
            if (MainApp.DesignMode)
            {
                this.SetEnum(GetFileMethod.Enumerator);
            }
            else
            {
                // this.SetEnum(ProcessQueueMethod.Parallel);
            }
        }
        #region Logging
        [DebuggerNonUserCode]        
        public override void Log(string prefix = "+", object status = null, object title = null, object text = null, object info = null, PadDirection textPadDirection = PadDirection.Default,
                       string textSuffix = null, PadDirection titlePadDirection = PadDirection.Default,
                       string titleSuffix = null, int random = 0)
        {
#if DEBUG || TRACE_EXT
            base.Log(prefix, status, 
                title ?? Type?.Name, text ?? Value, 
                info ?? (IsMultiple ? $"{SelectedItem?.ToString().Suffix(": ")}[{SelectedItems?.GetDebuggerDisplaySimple()}]" : SelectedItem?.ToString()), 
                textPadDirection, textSuffix, titlePadDirection, titleSuffix, random);
#endif
        }

        #endregion
    }
}