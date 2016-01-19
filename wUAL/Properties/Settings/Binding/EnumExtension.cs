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
    public class EnumExtension : SettingBindingExtensionBase<IEnumerable>
    {        
        public EnumExtension(string path) : base(path, false) { }
        protected override IEnumerable Value => _value;
        private Type enumType;
        private IEnumerable _value;
	
	    [TypeConverter( typeof( TypeTypeConverter ) )]
	    public Type EnumType
	    {
	        get { return enumType; }
	        set
	        {
	            enumType = value;
	            InitValues();
	        }
	    }

        protected override BindingMode DefaultMode => BindingMode.OneTime;

        protected override void Initialize()
        {
            EnumType = TypeTypeConverter.ConvertFromName(SettingPath);
        }

	    private void InitValues()
	    {
	        _value = EnumType.GetFields( BindingFlags.Public | BindingFlags.Static )
	                        .Select( x => x.GetValue( EnumType ) );
	    }
	}
	
	
}
