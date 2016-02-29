using System;
using AddGenericConstraint;

namespace Torrent.Infrastructure.Enums
{
    using Puchalapalli.Infrastructure.Interfaces;

    public interface IEnumKey<[AddGenericConstraint(typeof(Enum))] TKey> : IDebuggerDisplay
    where TKey : struct
    {
        TKey Key { get; set; }
    }
    public interface IEnumValue<TValue> : IDebuggerDisplay
    {
        TValue Value { get; set; }
    }
    public interface IEnumKeyValuePair<[AddGenericConstraint(typeof (Enum))] TKey, TValue> : IEnumKey<TKey>, IEnumValue<TValue>
    where TKey : struct
    {
    }
}