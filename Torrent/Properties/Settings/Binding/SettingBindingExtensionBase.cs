namespace Torrent.Properties.Settings.Binding
{
    using System.Windows.Data;
    using Helpers.Utils;
    using static Helpers.Utils.DebugUtils;
    using MySettings;
    using System.Windows;
    #region Base

    public abstract class SettingBindingExtensionBase<T> : Binding
    {
        protected SettingBindingExtensionBase() { Bind(); }

        protected SettingBindingExtensionBase(string path, string format = null) : base(path) { SetPath(path, format); }

        protected SettingBindingExtensionBase(string path, bool noBasePath, string format = null) : base()
        {
            SetPath(path, format);
        }

        private void SetPath(string path, string format)
        {
            Path = new PropertyPath(path, null);
            if (format != null) {
                Format = format;
            }
            Bind();
        }

        public virtual string Format
        {
            get { return base.StringFormat; }
            set
            {
                base.StringFormat = value;
                DEBUG.Break();
            }
        }

        protected abstract T Value { get; }
        //public new string Path { get; set; }
        private string _name;
        protected string Assembly { get; private set; }
        protected bool IsMainAssembly { get; private set; }

        protected string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                Assembly = Name.Split('.')[0];
                IsMainAssembly = (Assembly == MainApp.Name);
            }
        }

        protected string QualifiedName => Name + "." + Path;
        protected object GetSettingValue() => ClassUtils.GetProperty(Value, base.Path?.Path);
        protected object SettingValue => GetSettingValue();
        protected string SettingInfo => QualifiedName + ": " + SettingValue;
        protected virtual BindingMode DefaultMode => BindingMode.TwoWay;
        protected virtual void SetFromName() { }

        private void Bind()
        {
            var t = typeof(T);
            var name = t.Name;
            Name = t.FullName;
            SetFromName();
#if DEBUG || TRACE_EXT
            if (!IsMainAssembly)
            {
                var currentValue = SettingValue;
            }
#endif
            Source = Value;
            if (Value is MySettingsBase || name == "AllSettingsBase")
            {
                DEBUG.Noop();
            }
            Mode = DefaultMode;
            StringFormat = Format;
        }
    }

    #endregion
}
