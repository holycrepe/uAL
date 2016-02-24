using System;
using AddGenericConstraint;
using Torrent.Infrastructure.Enums;

namespace wUAL.WPF.Selectors.Models.Base
{
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
        public TTemplate Template
        {
            get { return this.Value; }
            set { this.Value = value; }
        }
    }
}