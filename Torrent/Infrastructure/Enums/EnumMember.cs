using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Torrent.Extensions;

namespace Torrent.Infrastructure.Enums
{
    using System.ComponentModel;
    using static Helpers.Utils.DebugUtils;
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class EnumMember : IDebuggerDisplay
    {
        #region Public Properties
        public string DescriptionFormat
            => "{1}";
        public string CombinedFormat
            => "{0}: {1}";
        public string ValueFormat
            => "{0}";
        public bool UseCombinedFormat { get; set; } = true;
        public string Name { get; set; } = null;
        public Type Type { get; set; }
        public string FieldInfo { get; set; }
        public object Value { get; set; }
        public object Description { get; set; }        
        #endregion
        #region Constructor
        public EnumMember(Type enumType, object value, bool useCombinedFormat = true)
        {
            if (enumType == null)
            {
                throw new ArgumentNullException($"EnumType is null");
            }
            if (!enumType.IsEnum)
            {
                throw new ArgumentException($"Type {enumType.GetFriendlyFullName()} must be an Enum");
            }            
            UseCombinedFormat = useCombinedFormat;
            var field = enumType.GetField(value.ToString());
            if (field != null)
            {
                SetFromField(enumType, field);
                return;
            }
            else if (enumType.IsEnumDefined(value))
            {
                foreach (var fi in enumType.GetPublicFields())
                {
                    var memberValue = fi.GetValue(enumType);
                    if (value == memberValue)
                    {
                        SetFromField(enumType, fi);
                        return;
                    }
                }
            }
            throw new ArgumentOutOfRangeException(nameof(enumType), $"Could not find Enum Member `{value}` in Enum Type {enumType.GetFriendlyFullName()}");
        }
        public EnumMember(Type enumType, FieldInfo field, bool useCombinedFormat = true)
        {
            UseCombinedFormat = useCombinedFormat;
            SetFromField(enumType, field);
        }
        private void SetFromField(Type enumType, FieldInfo field)
        {
            Type = enumType;
            Name = field.Name;
            Value = field.GetValue(enumType);
            Description = field.GetDescription(enumType);
            if (Description == null)
            {
                DEBUG.Noop(); 
            }
        }
        #endregion
        #region Display String
        public string Display
            => this.GetDisplay(this.UseCombinedFormat);
        public string QualifiedName
            => $"{this.Type?.Name.Suffix(".")}{this.Name}";
        private string GetStringFormat(bool useCombinedFormat)
            => this.Description == null
            ? this.ValueFormat
            : useCombinedFormat
                ? this.CombinedFormat
                : this.DescriptionFormat;
        protected string GetDisplay(bool useCombinedFormat=true)
            => string.Format(GetStringFormat(useCombinedFormat), this.Value, this.Description);
        #endregion
        #region Interfaces
        public override string ToString()
            => this.Value?.ToString();        
        public string DebuggerDisplay(int level = 1)
            => $"<{nameof(EnumMember)}{this.Type?.Name?.Prefix(":")}> {this.GetDisplay()}";
        public string DebuggerDisplaySimple(int level = 1)
            => this.QualifiedName;
        #endregion
        #region Static Array Creators
        public static IEnumerable<EnumMember> GetBrowsableEnumMembers(Type enumType, bool useCombinedFormat = true)
            => GetEnumMembers(enumType, useCombinedFormat, true);
        public static IEnumerable<EnumMember> GetEnumMembers(Type enumType, bool useCombinedFormat = true, bool requireBrowsable=false)
        {
            var fields = enumType.GetPublicFields();
            if (requireBrowsable)
            {
                fields = fields.Where(f => f.GetCustomAttribute<BrowsableAttribute>()?.Browsable != false);
            }
            return fields.Select(fi => new EnumMember(enumType, fi, useCombinedFormat));
        }
        public static EnumMember[] GetEnumMemberArray(Type enumType, bool useCombinedFormat = true, bool requireBrowsable=false)
            => GetEnumMembers(enumType, useCombinedFormat, requireBrowsable).ToArray();
        #endregion
    }
}
