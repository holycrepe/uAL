using System;
using AddGenericConstraint;

namespace Torrent.Infrastructure.Enums
{
    public interface IEnumKey<[AddGenericConstraint(typeof(Enum))] TKey>
    where TKey : struct
    {
        TKey Key { get; set; }
    }
    public interface IEnumValue<TValue>
    {
        TValue Value { get; set; }
    }
    public interface IEnumKeyValuePair<[AddGenericConstraint(typeof (Enum))] TKey, TValue> : IEnumKey<TKey>, IEnumValue<TValue>
    where TKey : struct
    {
    }
}