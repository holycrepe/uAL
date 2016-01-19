using System;
using Torrent.Helpers.Utils;
using Torrent.Infrastructure;
// ReSharper disable InconsistentNaming
namespace Torrent.Properties.Settings.MySettings
{
    using Enums;
    using Properties.Settings;

    public partial class MySettingsBase
    {
        #region Subclasses
        #region Subclasses: METHODS
        [Serializable]
        public class MyMethodsSettings : BaseSubSettings
        {
            GetFileMethod _getFiles;
            ProcessQueueMethod _processQueue;
            protected override object[] AllSettings => new object[] { nameof(GET_FILES), GET_FILES, nameof(PROCESS_QUEUE), PROCESS_QUEUE };

            public GetFileMethod GET_FILES {
                get { return _getFiles; }
                set { if (value != _getFiles) { _getFiles = value; OnPropertyChanged(nameof(GET_FILES)); } }
            }

            public ProcessQueueMethod PROCESS_QUEUE
            {
                get { return _processQueue; }
                set { if (value != _processQueue) { _processQueue = value; OnPropertyChanged(nameof(PROCESS_QUEUE)); } }
            }

            public MyMethodsSettings() : this($"Initializing.{nameof(MyMethodsSettings)}") {}
            public MyMethodsSettings(string name) : base(name)
            {
                GET_FILES = GetFileMethod.Plain;
                PROCESS_QUEUE = ProcessQueueMethod.Plain;
            }
        }
        #endregion
        #endregion
    }
}

