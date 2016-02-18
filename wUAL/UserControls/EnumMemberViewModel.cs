using PostSharp.Patterns.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.Windows.Controls;
using Torrent.Enums;
using Torrent.Extensions;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;
using Torrent.Infrastructure.Enums;
using Torrent.Properties.Settings;

namespace wUAL.UserControls
{
    [NotifyPropertyChanged]
    public class EnumMemberViewModel : ViewModel<EnumMember, object>
    {
        bool _useCombinedFormat = true;
        #region Public Properties
        #region Public Properties: Accessors
        public Type Type { get; set; }
        [SafeForDependencyAnalysis]
        public bool UseCombinedFormat
        {
            get { return this._useCombinedFormat; }
            set {
                if (SetProperty(ref this._useCombinedFormat, value))
                {
                    this.Items?.SetUseCombined(value);
                }
            }
        }
        public EnumMember EnumMember
        {
            get { return this.Type == null ? null : new EnumMember(this.Type, this.Value); }
            set { this.Value = value.Value; }
        }
        #endregion
        #region Public Properties: Methods        
        #region Public Properties: Methods: Enum Type
        private Type GetEnumType(bool required = true)
        {
            var enumType = this.Value?.GetType();
            if (enumType == null)
            {
                return null;
            }
            enumType = Nullable.GetUnderlyingType(enumType) ?? enumType;
            if (!enumType.IsEnum)
            {
                if (required)
                {
                    throw new ArgumentException($"Specified Type {enumType.GetFriendlyFullName()} is not an Enum.");
                }
                return null;
            }
            return enumType;
        }
        #endregion
        #region Public Properties: Methods: Enum
        public void SetEnum(object value=null, bool requireValue = true)
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
            Type = GetEnumType();
            Items.Clear();
            Items.AddRange(EnumMember.GetEnumMembers(Type, UseCombinedFormat));
        }
        #endregion
        #endregion
        #endregion
        public EnumMemberViewModel()
        {
            if (MainApp.DesignMode)
            {
                this.SetEnum(GetFileMethod.Enumerator);
            }
            else
            {
                this.SetEnum(ProcessQueueMethod.Parallel);
            }
        }
        #region Logging
        [System.Diagnostics.Conditional("DEBUG"), System.Diagnostics.Conditional("TRACE_EXT")]
        public void Log(string prefix = "+", string text = null)
        {
#if DEBUG || TRACE_EXT
            base.Log(prefix, Type.Name, text, Value, EnumMember);
#endif
        }

        #endregion
    }
}