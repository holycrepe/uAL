namespace Torrent.Properties.Settings.Binding
{
    using System.Windows.Data;
    using Helpers.Utils;

    #region Base
    public abstract class SettingBindingExtensionBase<T> : Binding
    {
        protected SettingBindingExtensionBase() { Bind(); }

        protected SettingBindingExtensionBase(string path) : base(path) { SetPath(path); }

        protected SettingBindingExtensionBase(string path, bool noBasePath) : base() {  SetPath(path); }

        private void SetPath(string path)
        {
            SettingPath = path;
            Bind();
        }

        protected abstract T Value { get; }
        protected string SettingPath { get; private set; }
        private string _name;
        protected string Assembly { get; private set; }
        protected bool IsMainAssembly { get; private set; }
        protected string Name {
            get { return _name; }
            set {
                _name = value;
                Assembly = Name.Split('.')[0];
                IsMainAssembly = (Assembly == SettingsManager.AppName);
            }
        }
        protected string QualifiedName => Name + "." + SettingPath;
        protected object GetSettingValue() => ClassUtils.GetProperty(Value, Path?.Path);
        protected object SettingValue => GetSettingValue();
        protected string SettingInfo => QualifiedName + ": " + SettingValue;
        protected virtual BindingMode DefaultMode => BindingMode.TwoWay;
        protected virtual void Initialize() {  }
        private void Bind()
        {
            Name = typeof(T).FullName;
            Initialize();
#if DEBUG || TRACE
            if (!IsMainAssembly)
            {
                var currentValue = SettingValue;
            }
#endif
            Source = Value;
            Mode = DefaultMode;
        }
    }
    #endregion
}