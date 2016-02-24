using System;

namespace Torrent.Enums
{
    using Converters;
    public enum OperationType
    {
        And,
        Or
    }

    public static partial class EnumExtensions
    {
        public static bool Evaluate(this OperationType type, object a, object b, bool fallbackValue=false)
        {
            bool first = Converters.ParseBool(a) ?? fallbackValue,
                second = Converters.ParseBool(b) ?? fallbackValue;
            switch (type)
            {
                case OperationType.And:
                    return first && second;
                case OperationType.Or:
                    return first || second;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}