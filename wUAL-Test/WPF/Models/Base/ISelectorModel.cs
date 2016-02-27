namespace wUAL.WPF.Models.Base
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using AddGenericConstraint;
    using Torrent.Infrastructure.Enums;

    public interface ISelectorModel<[AddGenericConstraint(typeof(Enum))] TKey, [AddGenericConstraint(typeof(Enum))] TTemplate> : IEnumKeyValuePair<TKey, TTemplate>
    where TKey : struct
    where TTemplate : struct
    {
        TTemplate Template {get; set; }
    }

    public class SelectorModel<[AddGenericConstraint(typeof (Enum))] TKey,
        [AddGenericConstraint(typeof (Enum))] TTemplate> : EnumKeyValuePair<TKey, TTemplate>,  ISelectorModel<TKey, TTemplate>
    where TKey : struct
    where TTemplate : struct
    {
        [DebuggerNonUserCode]
        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override TTemplate Value
        {
            get { return base.Value; }
            set { base.Value = value; }
        }
        public TTemplate Template
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
    }
}