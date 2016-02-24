using System;
using Torrent.Extensions;

// ReSharper disable InconsistentNaming

namespace uAL.Properties.Settings.ToggleSettings
{
    using Serialization;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;
    using Torrent.Properties.Settings;
    public partial class ToggleSettingsBase
    {
        [DataContract(Namespace = Namespaces.Default)]
        [DebuggerDisplay("{DebuggerDisplay(1)}")]
        public class ToggleSettingsBaseSubclass : BaseSubSettings
        {
            public ToggleSettingsBaseSubclass() : this($"Initializing.{nameof(ToggleSettingsBaseSubclass)}") { }
            public ToggleSettingsBaseSubclass(string name) : base(name) { }
        }
    }
}
