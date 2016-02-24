namespace uAL.Properties.Settings.LibSettings
{
    #region Usings

    using System.ComponentModel;
    using Torrent;
    using Torrent.Helpers.Utils;
    using ToggleSettings;
    using Torrent.Extensions;
    using static ToggleSettings.ToggleSettings;
    using static Torrent.Helpers.Utils.DebugUtils;
    using Torrent.Properties.Settings;
    #endregion

    public sealed partial class LibSettings
    {
        #region Constructor
        static LibSettings _settings;
        public static LibSettings LibSetting
        {
            get { return _settings ?? (_settings = LibSettings.Load()); }
            set
            {
                if (_settings == null)
                {
                    _settings = value;
                }
                else
                {
                    _settings.Load(value);
                }
            }
        }
        static LibSettings()
        {
            if (MainApp.DesignMode)
            {                
                //DEBUG.Break();
                return;
            }
            if (_settings != null)
            {
                DEBUG.Break();
            }
            if (CONSTRUCTOR_ACTION == BaseSettingsConstructor.Load)
            {
                LoadInstance();
            }
            else if (CONSTRUCTOR_ACTION == BaseSettingsConstructor.Default)
            {
                LoadInstance(new LibSettings());
            }
            
            DEBUG.Noop();
        }

        #endregion

        #region Load/Save
        public static LibSettings LoadInstance()
            => LoadInstance(Load());
        public static LibSettings LoadInstance(LibSettings libSetting)
            => BindInstance(LibSetting = libSetting);
        public static LibSettings BindInstance()
            => BindInstance(Load());
        public static LibSettings BindInstance(LibSettings libSetting)
        {
            RegexUtils.GetLabelsWordsToStrip = libSetting.Labels.GetLabelsWordsToStrip;
            TorrentItem.getWordsToStrip = e => libSetting.Labels.TORRENT_NAME_WORDS_TO_STRIP;
            (Toggles.Filters as INotifyPropertyChanged).PropertyChanged +=
                (s, e) => libSetting.Labels.FilterResults?.Clear();
            return libSetting;
        }

        public static void SaveInstance() { LibSetting.Save(); }        

        #endregion
    }
}
