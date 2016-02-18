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
    using Torrent.Extensions;
    using System.Collections.Generic;
    public class EnumBindingExtension : SettingBindingExtensionBase<IEnumerable>
    {
        public EnumBindingExtension() : base() { }
        public EnumBindingExtension(string path) : base(path) { }
        protected override IEnumerable Value => _value;
        private string _enumName=null;
        private Type _enumType;
        private IEnumerable _value;
    
        //[TypeConverter( typeof( TypeTypeConverter ) )]
        public Type EnumType
        {
            get { return _enumType; }
            set
            {
                if (value != null)
                {
                    _enumType = Nullable.GetUnderlyingType(value) ?? value;
                    if (_enumType != null && !_enumType.IsEnum)
                    {
                        throw new ArgumentException($"Type {_enumType.GetFriendlyFullName()} must be an Enum.");
                    }
                    SetFromType();
                }
            }
        }
        
        public string EnumName {
            get { return this._enumName; }
            set { this._enumName = value; SetFromName(); }
        }
        public bool UseDescriptions { get; set; } = false;
        protected override BindingMode DefaultMode => BindingMode.OneWay;

        protected override void SetFromName()
        {
            if (EnumName == null && Path?.Path != null)
            {
                _enumName = Path?.Path;
            }
            EnumType = TypeTypeConverter.ConvertFromName(EnumName);
            Path = null;
        }
        private void SetFromType()
        {
            if (EnumType == null)
            {
                throw new ArgumentNullException(nameof(EnumType));
            }
            _value = UseDescriptions ? EnumType.GetDescriptions() : EnumType.GetValues();
        }
    }
    
    
}
