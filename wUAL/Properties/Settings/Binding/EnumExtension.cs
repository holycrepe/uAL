using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace wUAL.Properties.Settings.Binding
{
    using System.Windows.Data;
    using Torrent.Properties.Settings.Binding;
    using Torrent.Converters;
    using System.Windows.Markup;
    using Torrent.Extensions;
    public class EnumExtension : MarkupExtension
    {
        private Type _enumType;
        public Type EnumType
        {
            get { return this._enumType; }
            set
            {
                if (value != this._enumType && value != null)
                {
                    var enumType = Nullable.GetUnderlyingType(value) ?? value;

                    if (!enumType.IsEnum)
                    {
                        throw new ArgumentException($"Type {enumType.GetFriendlyFullName()} must be an Enum.");
                    }
                    this._enumType = value;
                }
            }
        }

        public EnumExtension() : base() { }

        public EnumExtension(Type enumType) : base()
        {
            this.EnumType = enumType;
        }

        public bool UseDescriptions { get; set; } = false;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (EnumType == null)
            {
                throw new InvalidOperationException($"{nameof(EnumType)} must be specified.");
            }

            return UseDescriptions ? EnumType.GetDescriptions() : EnumType.GetValues();

            //Type actualEnumType = Nullable.GetUnderlyingType(this._enumType) ?? this._enumType;
            //Array enumValues = Enum.GetValues(actualEnumType);

            //if (actualEnumType == this._enumType)
            //    return enumValues;

            //Array tempArray = Array.CreateInstance(actualEnumType, enumValues.Length + 1);
            //enumValues.CopyTo(tempArray, 1);
            //return tempArray;
        }
    }
}
