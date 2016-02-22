using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Torrent.Extensions;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure.Reflection;

namespace Torrent.Infrastructure.Enums
{
    using System.ComponentModel;
    using static Helpers.Utils.DebugUtils;
    [DebuggerDisplay("{DebuggerDisplay(1)}")]
    public class EnumMember : IDebuggerDisplay, IConvertible,
        IEquatable<EnumMember>, IEquatable<object>, IEquatable<long>, IEquatable<int>
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
        //public string FieldInfo { get; set; }
        public object Value { get; set; }
        public long Long { get; set; }
        public bool Browsable { get; set; }
        public bool IsMain
            => this.Long > 0;
        public bool IsDisabled
            => this.Long == 0;
        public bool IsAll
            => this.Long == -1;
        public string Description { get; set; }
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
            if (SetFromField(enumType, field) ||
                enumType.IsEnumDefined(value) && SetFromField(enumType.GetPublicFields().FirstOrDefault(fi => value == fi.Value)))
                return;

            var usedBits = Convert.ToInt64(EnumUtils.GetUsedBits(enumType, true));
            if (!usedBits.Has(value))
                throw new ArgumentOutOfRangeException(nameof(enumType),
                                                      $"Could not find Enum Member `{value}` in Enum Type {enumType.GetFriendlyFullName()}");
            Type = enumType;
            Name = value.ToString();
            Value = value;
            Long = Convert.ToInt64(Value);
            Browsable = false;
            Description = string.Join(", ", EnumUtils.GetMatchingFields(Type, Value).Select(f => f.Description));
        }
        public EnumMember(Type enumType, Field<object> field, bool useCombinedFormat = true)
        {
            UseCombinedFormat = useCombinedFormat;
            SetFromField(field);
        }
        public EnumMember(Type enumType, FieldInfo field, bool useCombinedFormat = true)
        {
            UseCombinedFormat = useCombinedFormat;
            SetFromField(enumType, field);
        }

        private bool SetFromField(Type enumType, FieldInfo field)
            => field != null && SetFromField(new Field<object>(field, enumType));
        private bool SetFromField(Field<object> field)
        {
            if (field == null)
                return false;
            Type = field.Type;
            Name = field.Name;
            Value = field.Value;
            Long = Convert.ToInt64(Value);
            Description = field.Description;
            Browsable = field.Browsable != false;
            return true;
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
        protected string GetDisplay(bool useCombinedFormat = true)
            => string.Format(GetStringFormat(useCombinedFormat), this.Value, this.Description);
        #endregion
        #region Interfaces

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        bool IEquatable<EnumMember>.Equals(EnumMember other)
            => (this.Long == other?.Long);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        bool IEquatable<long>.Equals(long other)
            => (this.Long == other);

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        bool IEquatable<int>.Equals(int other)
            => (this.Long == other);

        [DebuggerNonUserCode]
        public override string ToString()
            => DebuggerDisplaySimple();
        public string DebuggerDisplay(int level = 1)
            => $"<{nameof(EnumMember)}{this.Type?.Name?.Prefix(":")}> {this.GetDisplay()}";
        public string DebuggerDisplaySimple(int level = 1)
            => this.Value?.ToString();
        #endregion
        #region Static Array Creators
        public static IEnumerable<EnumMember> GetBrowsableEnumMembers(Type enumType, bool useCombinedFormat = true)
            => GetEnumMembers(enumType, useCombinedFormat, true);
        public static IEnumerable<EnumMember> GetEnumMembers(Type enumType, bool useCombinedFormat = true, bool requireBrowsable = false)
        {
            var fields = enumType.GetPublicFields();
            if (requireBrowsable)
            {
                fields = fields.Where(f => f.Browsable != false);
            }
            return fields.Select(fi => new EnumMember(enumType, fi, useCombinedFormat));
        }
        public static EnumMember[] GetEnumMemberArray(Type enumType, bool useCombinedFormat = true, bool requireBrowsable = false)
            => GetEnumMembers(enumType, useCombinedFormat, requireBrowsable).ToArray();
        #endregion
        #region Operators
        public override int GetHashCode()
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            => Name.GetHashCode() ^ Long.GetHashCode();

        public override bool Equals(object o)
        {
            if (o == null)
                return false;

            var member = o as EnumMember;
            if (!ReferenceEquals(member, null))
                return this.Equals(member);
            var convertible = o as IConvertible;
            if (convertible != null)
                try
                {
                    var cast = Convert.ToInt64(convertible);
                    return Equals(cast);
                }
                catch (InvalidCastException) { }

            var longValue = o as long?;
            if (longValue.HasValue)
                return this.Equals(longValue.Value);

            var value = o as int?;
            return value.HasValue
                && this.Equals(value.Value);
        }

        public bool Equals(EnumMember other)
            => other.Name == this.Name && other.Value == this.Value;

        public bool Equals(long other)
            => other == this.Long;

        public bool Equals(int other)
            => other == this.Long;
        public static bool operator ==(EnumMember a, EnumMember b)
        {
            // If both are null, or both are same instance, return true.
            if (ReferenceEquals(a, b))
            {
                return true;
            }

            // If one is null, but not both, return false.
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return false;
            }

            // Return true if the fields match:
            return a.Equals(b);
        }
        public static bool operator ==(object a, EnumMember b)
            => object.ReferenceEquals(a, b) || (
            !object.ReferenceEquals(a, null) &&
            !object.ReferenceEquals(b, null) && b.Equals(a));

        public static bool operator ==(long a, EnumMember b)
            => !object.ReferenceEquals(b, null) && a.Equals(b.Long);

        public static bool operator ==(int a, EnumMember b)
            => !object.ReferenceEquals(b, null) && a.Equals((int)b.Long);

        public static bool operator ==(EnumMember a, object b)
            => b == a;
        public static bool operator ==(EnumMember a, long b)
            => b == a;
        public static bool operator ==(EnumMember a, int b)
            => b == a;
        public static bool operator !=(EnumMember a, EnumMember b) => !(a == b);
        public static bool operator !=(EnumMember a, object b) => !(a == b);
        public static bool operator !=(EnumMember a, long b) => !(a == b);
        public static bool operator !=(EnumMember a, int b) => !(a == b);
        public static bool operator !=(object a, EnumMember b) => !(a == b);
        public static bool operator !=(long a, EnumMember b) => !(a == b);
        public static bool operator !=(int a, EnumMember b) => !(a == b);

        public static implicit operator long(EnumMember other)
            => other.Long;
        public static implicit operator int(EnumMember other)
            => (int)other.Long;
        public static implicit operator bool(EnumMember other)
            => other != null;
        #endregion

        #region Implementation of IConvertible

        /// <summary>
        /// Returns the <see cref="T:System.TypeCode"/> for this instance.
        /// </summary>
        /// <returns>
        /// The enumerated constant that is the <see cref="T:System.TypeCode"/> of the class or value type that implements this interface.
        /// </returns>
        TypeCode IConvertible.GetTypeCode()
            => TypeCode.Object;

        /// <summary>
        /// Converts the value of this instance to an equivalent Boolean value using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A Boolean value equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        bool IConvertible.ToBoolean(IFormatProvider provider)
            => true;

        /// <summary>
        /// Converts the value of this instance to an equivalent Unicode character using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A Unicode character equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 8-bit signed integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 8-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 8-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 16-bit signed integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        short IConvertible.ToInt16(IFormatProvider provider)
            => Convert.ToInt16(Long);

        /// <summary>
        /// Converts the value of this instance to an equivalent 16-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 16-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        ushort IConvertible.ToUInt16(IFormatProvider provider)
            => Convert.ToUInt16(Long);

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 32-bit signed integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        int IConvertible.ToInt32(IFormatProvider provider)
            => Convert.ToInt32(Long);

        /// <summary>
        /// Converts the value of this instance to an equivalent 32-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 32-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        uint IConvertible.ToUInt32(IFormatProvider provider)
            => Convert.ToUInt32(Long);

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit signed integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 64-bit signed integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        long IConvertible.ToInt64(IFormatProvider provider)
            => Long;

        /// <summary>
        /// Converts the value of this instance to an equivalent 64-bit unsigned integer using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An 64-bit unsigned integer equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        ulong IConvertible.ToUInt64(IFormatProvider provider)
            => Convert.ToUInt32(Long);

        /// <summary>
        /// Converts the value of this instance to an equivalent single-precision floating-point number using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A single-precision floating-point number equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        float IConvertible.ToSingle(IFormatProvider provider)
            => Convert.ToSingle(Long);

        /// <summary>
        /// Converts the value of this instance to an equivalent double-precision floating-point number using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A double-precision floating-point number equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        double IConvertible.ToDouble(IFormatProvider provider)
            => Convert.ToInt32(Long);

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.Decimal"/> number using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Decimal"/> number equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        decimal IConvertible.ToDecimal(IFormatProvider provider)
            => Convert.ToInt32(Long);

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.DateTime"/> using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.DateTime"/> instance equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the value of this instance to an equivalent <see cref="T:System.String"/> using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> instance equivalent to the value of this instance.
        /// </returns>
        /// <param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        string IConvertible.ToString(IFormatProvider provider)
            => Value.ToString();

        /// <summary>
        /// Converts the value of this instance to an <see cref="T:System.Object"/> of the specified <see cref="T:System.Type"/> that has an equivalent value, using the specified culture-specific formatting information.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Object"/> instance of type <paramref name="conversionType"/> whose value is equivalent to the value of this instance.
        /// </returns>
        /// <param name="conversionType">The <see cref="T:System.Type"/> to which the value of this instance is converted. </param><param name="provider">An <see cref="T:System.IFormatProvider"/> interface implementation that supplies culture-specific formatting information. </param>
        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
            => Convert.ChangeType(Value, conversionType);

        #endregion
    }
}
