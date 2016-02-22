﻿using PostSharp.Patterns.Model;
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

namespace wUAL.UserControls
{
    public class EnumMemberViewModel : ViewModel<EnumMember, object>
    {
        bool _useCombinedFormat = true;

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
        #region Public Properties
        #region Public Properties: Accessors
        public EnumMember[] Members { get; set; }
        #endregion
        #region Public Properties: Methods        
        #region Public Properties: Methods: Enum Type
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
            Members = EnumMember.GetEnumMembers(Type, UseCombinedFormat).ToArray();
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
        public EnumMemberViewModel(bool setDesignEnum)
        {

        }
        public EnumMemberViewModel()
        {
            if (MainApp.DesignMode)
            {
                // this.SetEnum(GetFileMethod.Enumerator);
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